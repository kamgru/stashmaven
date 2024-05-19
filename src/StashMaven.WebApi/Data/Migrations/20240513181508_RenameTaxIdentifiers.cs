using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTaxIdentifiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxIdentifier",
                schema: "prt");

            migrationBuilder.EnsureSchema(
                name: "partnership");

            migrationBuilder.RenameTable(
                name: "Partner",
                schema: "prt",
                newName: "Partner",
                newSchema: "partnership");

            migrationBuilder.RenameTable(
                name: "Address",
                schema: "prt",
                newName: "Address",
                newSchema: "partnership");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "inv",
                table: "ShipmentKind",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "BusinessIdentifier",
                schema: "partnership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartnerId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessIdentifier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessIdentifier_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalSchema: "partnership",
                        principalTable: "Partner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessIdentifier_PartnerId",
                schema: "partnership",
                table: "BusinessIdentifier",
                column: "PartnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessIdentifier",
                schema: "partnership");

            migrationBuilder.EnsureSchema(
                name: "prt");

            migrationBuilder.RenameTable(
                name: "Partner",
                schema: "partnership",
                newName: "Partner",
                newSchema: "prt");

            migrationBuilder.RenameTable(
                name: "Address",
                schema: "partnership",
                newName: "Address",
                newSchema: "prt");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "inv",
                table: "ShipmentKind",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateTable(
                name: "TaxIdentifier",
                schema: "prt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartnerId = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxIdentifier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxIdentifier_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalSchema: "prt",
                        principalTable: "Partner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxIdentifier_PartnerId",
                schema: "prt",
                table: "TaxIdentifier",
                column: "PartnerId");
        }
    }
}
