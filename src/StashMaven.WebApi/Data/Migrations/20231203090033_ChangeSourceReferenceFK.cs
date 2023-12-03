using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSourceReferenceFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipment_SourceReferences_SourceReferenceId",
                schema: "inv",
                table: "Shipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SourceReferences",
                table: "SourceReferences");

            migrationBuilder.RenameTable(
                name: "SourceReferences",
                newName: "SourceReference",
                newSchema: "inv");

            migrationBuilder.AlterColumn<int>(
                name: "SourceReferenceId",
                schema: "inv",
                table: "Shipment",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SourceReference",
                schema: "inv",
                table: "SourceReference",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipment_SourceReference_SourceReferenceId",
                schema: "inv",
                table: "Shipment",
                column: "SourceReferenceId",
                principalSchema: "inv",
                principalTable: "SourceReference",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipment_SourceReference_SourceReferenceId",
                schema: "inv",
                table: "Shipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SourceReference",
                schema: "inv",
                table: "SourceReference");

            migrationBuilder.RenameTable(
                name: "SourceReference",
                schema: "inv",
                newName: "SourceReferences");

            migrationBuilder.AlterColumn<int>(
                name: "SourceReferenceId",
                schema: "inv",
                table: "Shipment",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SourceReferences",
                table: "SourceReferences",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipment_SourceReferences_SourceReferenceId",
                schema: "inv",
                table: "Shipment",
                column: "SourceReferenceId",
                principalTable: "SourceReferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
