using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RabbitMQ.Client;
using StashMaven.Common;

namespace StashMaven.WebApi.Controllers;

public record ShipmentItem(
    string InventoryItemId,
    decimal Quantity);

public class AcceptShipmentRequest
{
    public bool Inbound { get; set; }
    public List<ShipmentItem> ShipmentItems { get; set; }
}

[ApiController]
[Route("api/v1/[controller]")]
public class ShipmentController : ControllerBase
{
    [HttpPost]
    [Route("accept-shipment")]
    public async Task<IActionResult> AcceptShipment(
        AcceptShipmentRequest request)
    {
        ConnectionFactory factory = new() { HostName = "localhost" };
        using IConnection connection = factory.CreateConnection();
        using IModel channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "accept-shipment",
            exclusive: false,
            autoDelete: false,
            arguments: null);

        foreach (ShipmentItem shipmentItem in request.ShipmentItems)
        {
            InventoryItemQuantityChanged quantityChanged = new()
            {
                InventoryItemId = shipmentItem.InventoryItemId,
                Quantity = request.Inbound
                    ? shipmentItem.Quantity
                    : -shipmentItem.Quantity
            };

            string requestJson = JsonSerializer.Serialize(quantityChanged);
            byte[] messageBody = Encoding.UTF8.GetBytes(requestJson);

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: "accept-shipment",
                basicProperties: null,
                body: messageBody);
        }

        return Ok();
    }
}
