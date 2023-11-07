using System.Text;
using System.Text.Json;
using Npgsql;
using NpgsqlTypes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StashMaven.Common;

ConnectionFactory factory = new() { HostName = "localhost" };
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "accept-shipment",
    exclusive: false,
    autoDelete: false,
    arguments: null);

EventingBasicConsumer consumer = new(channel);
consumer.Received += async (
    model,
    eventArgs) =>
{
    byte[] messageBody = eventArgs.Body.ToArray();
    string messageJson = Encoding.UTF8.GetString(messageBody);
    InventoryItemQuantityChanged quantityChanged = JsonSerializer.Deserialize<InventoryItemQuantityChanged>(messageJson)
                                                   ?? throw new InvalidOperationException(
                                                       "Could not deserialize message.");

    Console.WriteLine($"Received message: {messageJson}");

    string connectionString = "Host=localhost;Username=postgres;Password=P@ssw0rd!;Database=stashmaven";
    await using NpgsqlDataSource dataSource = new NpgsqlDataSourceBuilder(connectionString)
        .Build();

    await using NpgsqlConnection dbConnection = await dataSource.OpenConnectionAsync();
    await using NpgsqlTransaction transaction = await dbConnection.BeginTransactionAsync();
    await using NpgsqlCommand insertEventCmd = dataSource.CreateCommand(
        """
        INSERT INTO inventory_ledger (inventory_item_id, event_type, payload)
        VALUES (@inventoryItemId, @eventType, @payload);
        """);

    insertEventCmd.Parameters.AddWithValue("inventoryItemId", NpgsqlDbType.Uuid, Guid.Parse(quantityChanged.InventoryItemId));
    insertEventCmd.Parameters.AddWithValue("eventType", 1);
    insertEventCmd.Parameters.AddWithValue("payload", NpgsqlDbType.Json, messageJson);
    await insertEventCmd.ExecuteNonQueryAsync();

    await using NpgsqlCommand updateQtyCmd = dataSource.CreateCommand(
        """
        INSERT INTO inventory_item_view (inventory_item_id, quantity)
        VALUES (@inventoryItemId, @quantity)
        ON CONFLICT (inventory_item_id) DO UPDATE
        SET quantity = inventory_item_view.quantity + @quantity;
        """);

    updateQtyCmd.Parameters.AddWithValue("inventoryItemId", NpgsqlDbType.Uuid, Guid.Parse(quantityChanged.InventoryItemId));
    updateQtyCmd.Parameters.AddWithValue("quantity", NpgsqlDbType.Numeric, quantityChanged.Quantity);
    await updateQtyCmd.ExecuteNonQueryAsync();

    await transaction.CommitAsync();
    await dbConnection.CloseAsync();
};

channel.BasicConsume(
        queue: "accept-shipment",
        autoAck: true,
        consumer: consumer);

Console.WriteLine("Listening for messages. Press any key to exit.");
Console.ReadKey();

