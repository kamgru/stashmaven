using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShippedOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ShippedOn",
                schema: "inv",
                table: "Shipment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippedOn",
                schema: "inv",
                table: "Shipment");
        }
    }
}
