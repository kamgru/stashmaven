using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentRecord_Shipment_ShipmentId",
                schema: "inv",
                table: "ShipmentRecord");

            migrationBuilder.AlterColumn<int>(
                name: "ShipmentId",
                schema: "inv",
                table: "ShipmentRecord",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceReferenceId",
                schema: "inv",
                table: "Shipment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SourceReferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identifier = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceReferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_SourceReferenceId",
                schema: "inv",
                table: "Shipment",
                column: "SourceReferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipment_SourceReferences_SourceReferenceId",
                schema: "inv",
                table: "Shipment",
                column: "SourceReferenceId",
                principalTable: "SourceReferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentRecord_Shipment_ShipmentId",
                schema: "inv",
                table: "ShipmentRecord",
                column: "ShipmentId",
                principalSchema: "inv",
                principalTable: "Shipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipment_SourceReferences_SourceReferenceId",
                schema: "inv",
                table: "Shipment");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentRecord_Shipment_ShipmentId",
                schema: "inv",
                table: "ShipmentRecord");

            migrationBuilder.DropTable(
                name: "SourceReferences");

            migrationBuilder.DropIndex(
                name: "IX_Shipment_SourceReferenceId",
                schema: "inv",
                table: "Shipment");

            migrationBuilder.DropColumn(
                name: "SourceReferenceId",
                schema: "inv",
                table: "Shipment");

            migrationBuilder.AlterColumn<int>(
                name: "ShipmentId",
                schema: "inv",
                table: "ShipmentRecord",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentRecord_Shipment_ShipmentId",
                schema: "inv",
                table: "ShipmentRecord",
                column: "ShipmentId",
                principalSchema: "inv",
                principalTable: "Shipment",
                principalColumn: "Id");
        }
    }
}
