using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "prt");

            migrationBuilder.EnsureSchema(
                name: "cat");

            migrationBuilder.EnsureSchema(
                name: "inv");

            migrationBuilder.CreateTable(
                name: "Brand",
                schema: "cat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BrandId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Partner",
                schema: "prt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartnerId = table.Column<string>(type: "text", nullable: false),
                    LegalName = table.Column<string>(type: "text", nullable: false),
                    CustomIdentifier = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SequenceGenerator",
                schema: "inv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SequenceGeneratorId = table.Column<string>(type: "text", nullable: false),
                    NextValue = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SequenceGenerator", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentKind",
                schema: "inv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShipmentKindId = table.Column<string>(type: "text", nullable: false),
                    SequenceGeneratorId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortCode = table.Column<string>(type: "text", nullable: false),
                    ShipmentDirection = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentKind", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SourceReference",
                schema: "inv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identifier = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceReference", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stockpile",
                schema: "inv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StockpileId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stockpile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxDefinition",
                schema: "cat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaxDefinitionId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                schema: "prt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartnerId = table.Column<int>(type: "integer", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    StreetAdditional = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    CountryCode = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalSchema: "prt",
                        principalTable: "Partner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxIdentifier",
                schema: "prt",
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
                    table.PrimaryKey("PK_TaxIdentifier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxIdentifier_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalSchema: "prt",
                        principalTable: "Partner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItem",
                schema: "inv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryItemId = table.Column<string>(type: "text", nullable: false),
                    Sku = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxDefinitionId = table.Column<string>(type: "text", nullable: false),
                    StockpileId = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItem_Stockpile_StockpileId",
                        column: x => x.StockpileId,
                        principalSchema: "inv",
                        principalTable: "Stockpile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shipment",
                schema: "inv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShipmentId = table.Column<string>(type: "text", nullable: false),
                    SupplierId = table.Column<string>(type: "text", nullable: true),
                    ShipmentSeqId = table.Column<string>(type: "text", nullable: true),
                    ShipmentAcceptance = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    KindId = table.Column<int>(type: "integer", nullable: false),
                    SourceReferenceId = table.Column<int>(type: "integer", nullable: true),
                    StockpileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipment_ShipmentKind_KindId",
                        column: x => x.KindId,
                        principalSchema: "inv",
                        principalTable: "ShipmentKind",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shipment_SourceReference_SourceReferenceId",
                        column: x => x.SourceReferenceId,
                        principalSchema: "inv",
                        principalTable: "SourceReference",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipment_Stockpile_StockpileId",
                        column: x => x.StockpileId,
                        principalSchema: "inv",
                        principalTable: "Stockpile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogItem",
                schema: "cat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CatalogItemId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Sku = table.Column<string>(type: "text", nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "integer", nullable: false),
                    TaxDefinitionId = table.Column<int>(type: "integer", nullable: true),
                    BarCode = table.Column<string>(type: "text", nullable: true),
                    BrandId = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItem_Brand_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "cat",
                        principalTable: "Brand",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CatalogItem_TaxDefinition_TaxDefinitionId",
                        column: x => x.TaxDefinitionId,
                        principalSchema: "cat",
                        principalTable: "TaxDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShipmentRecord",
                schema: "inv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric", nullable: false),
                    InventoryItemId = table.Column<int>(type: "integer", nullable: false),
                    ShipmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentRecord_InventoryItem_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalSchema: "inv",
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipmentRecord_Shipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "inv",
                        principalTable: "Shipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_PartnerId",
                schema: "prt",
                table: "Address",
                column: "PartnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItem_BrandId",
                schema: "cat",
                table: "CatalogItem",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItem_TaxDefinitionId",
                schema: "cat",
                table: "CatalogItem",
                column: "TaxDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_StockpileId",
                schema: "inv",
                table: "InventoryItem",
                column: "StockpileId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_KindId",
                schema: "inv",
                table: "Shipment",
                column: "KindId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_SourceReferenceId",
                schema: "inv",
                table: "Shipment",
                column: "SourceReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_StockpileId",
                schema: "inv",
                table: "Shipment",
                column: "StockpileId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentKind_ShortCode",
                schema: "inv",
                table: "ShipmentKind",
                column: "ShortCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentRecord_InventoryItemId",
                schema: "inv",
                table: "ShipmentRecord",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentRecord_ShipmentId",
                schema: "inv",
                table: "ShipmentRecord",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxIdentifier_PartnerId",
                schema: "prt",
                table: "TaxIdentifier",
                column: "PartnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address",
                schema: "prt");

            migrationBuilder.DropTable(
                name: "CatalogItem",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "SequenceGenerator",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "ShipmentRecord",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "TaxIdentifier",
                schema: "prt");

            migrationBuilder.DropTable(
                name: "Brand",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "TaxDefinition",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "InventoryItem",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "Shipment",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "Partner",
                schema: "prt");

            migrationBuilder.DropTable(
                name: "ShipmentKind",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "SourceReference",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "Stockpile",
                schema: "inv");
        }
    }
}
