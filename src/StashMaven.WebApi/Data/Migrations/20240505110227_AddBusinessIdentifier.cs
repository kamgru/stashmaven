using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "partnership");

            migrationBuilder.RenameTable(
                name: "TaxIdentifier",
                schema: "prt",
                newName: "TaxIdentifier",
                newSchema: "partnership");

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
                    BusinessIdentifierId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ShortCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessIdentifier", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessIdentifier_ShortCode",
                schema: "partnership",
                table: "BusinessIdentifier",
                column: "ShortCode",
                unique: true);
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
                name: "TaxIdentifier",
                schema: "partnership",
                newName: "TaxIdentifier",
                newSchema: "prt");

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
        }
    }
}
