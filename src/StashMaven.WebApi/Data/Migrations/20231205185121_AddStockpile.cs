using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStockpile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockpileId",
                schema: "inv",
                table: "Shipment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StockpileId",
                schema: "inv",
                table: "InventoryItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Stockpile",
                schema: "inv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StockpileId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stockpile", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_StockpileId",
                schema: "inv",
                table: "Shipment",
                column: "StockpileId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_StockpileId",
                schema: "inv",
                table: "InventoryItem",
                column: "StockpileId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_Stockpile_StockpileId",
                schema: "inv",
                table: "InventoryItem",
                column: "StockpileId",
                principalSchema: "inv",
                principalTable: "Stockpile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipment_Stockpile_StockpileId",
                schema: "inv",
                table: "Shipment",
                column: "StockpileId",
                principalSchema: "inv",
                principalTable: "Stockpile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_Stockpile_StockpileId",
                schema: "inv",
                table: "InventoryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipment_Stockpile_StockpileId",
                schema: "inv",
                table: "Shipment");

            migrationBuilder.DropTable(
                name: "Stockpile",
                schema: "inv");

            migrationBuilder.DropIndex(
                name: "IX_Shipment_StockpileId",
                schema: "inv",
                table: "Shipment");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItem_StockpileId",
                schema: "inv",
                table: "InventoryItem");

            migrationBuilder.DropColumn(
                name: "StockpileId",
                schema: "inv",
                table: "Shipment");

            migrationBuilder.DropColumn(
                name: "StockpileId",
                schema: "inv",
                table: "InventoryItem");
        }
    }
}
