using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxIdToPartnerSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaxIdType",
                schema: "inv",
                table: "ShipmentPartnerReference",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxIdValue",
                schema: "inv",
                table: "ShipmentPartnerReference",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxIdType",
                schema: "inv",
                table: "ShipmentPartnerReference");

            migrationBuilder.DropColumn(
                name: "TaxIdValue",
                schema: "inv",
                table: "ShipmentPartnerReference");
        }
    }
}
