using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxInfoToRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaxRate",
                schema: "inv",
                table: "ShipmentRecord",
                newName: "Tax_Rate");

            migrationBuilder.AddColumn<string>(
                name: "Tax_Name",
                schema: "inv",
                table: "ShipmentRecord",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tax_TaxDefinitionId",
                schema: "inv",
                table: "ShipmentRecord",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tax_Name",
                schema: "inv",
                table: "ShipmentRecord");

            migrationBuilder.DropColumn(
                name: "Tax_TaxDefinitionId",
                schema: "inv",
                table: "ShipmentRecord");

            migrationBuilder.RenameColumn(
                name: "Tax_Rate",
                schema: "inv",
                table: "ShipmentRecord",
                newName: "TaxRate");
        }
    }
}
