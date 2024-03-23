using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class InvItem2CatItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatalogItemId",
                schema: "inv",
                table: "InventoryItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_CatalogItemId",
                schema: "inv",
                table: "InventoryItem",
                column: "CatalogItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_CatalogItem_CatalogItemId",
                schema: "inv",
                table: "InventoryItem",
                column: "CatalogItemId",
                principalSchema: "cat",
                principalTable: "CatalogItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_CatalogItem_CatalogItemId",
                schema: "inv",
                table: "InventoryItem");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItem_CatalogItemId",
                schema: "inv",
                table: "InventoryItem");

            migrationBuilder.DropColumn(
                name: "CatalogItemId",
                schema: "inv",
                table: "InventoryItem");
        }
    }
}
