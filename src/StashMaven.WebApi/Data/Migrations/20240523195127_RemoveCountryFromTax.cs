using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCountryFromTax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                schema: "com",
                table: "TaxDefinition");

            migrationBuilder.EnsureSchema(
                name: "common");

            migrationBuilder.RenameTable(
                name: "TaxDefinition",
                schema: "com",
                newName: "TaxDefinition",
                newSchema: "common");

            migrationBuilder.RenameTable(
                name: "StashMavenOption",
                schema: "com",
                newName: "StashMavenOption",
                newSchema: "common");

            migrationBuilder.RenameTable(
                name: "SourceReference",
                schema: "com",
                newName: "SourceReference",
                newSchema: "common");

            migrationBuilder.RenameTable(
                name: "CompanyOption",
                schema: "com",
                newName: "CompanyOption",
                newSchema: "common");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "com");

            migrationBuilder.RenameTable(
                name: "TaxDefinition",
                schema: "common",
                newName: "TaxDefinition",
                newSchema: "com");

            migrationBuilder.RenameTable(
                name: "StashMavenOption",
                schema: "common",
                newName: "StashMavenOption",
                newSchema: "com");

            migrationBuilder.RenameTable(
                name: "SourceReference",
                schema: "common",
                newName: "SourceReference",
                newSchema: "com");

            migrationBuilder.RenameTable(
                name: "CompanyOption",
                schema: "common",
                newName: "CompanyOption",
                newSchema: "com");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                schema: "com",
                table: "TaxDefinition",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");
        }
    }
}
