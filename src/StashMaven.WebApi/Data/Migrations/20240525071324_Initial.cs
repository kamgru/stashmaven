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
                name: "partnership");

            migrationBuilder.EnsureSchema(
                name: "cat");

            migrationBuilder.EnsureSchema(
                name: "common");

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
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ShortCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyOption",
                schema: "common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Partner",
                schema: "partnership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartnerId = table.Column<string>(type: "text", nullable: false),
                    LegalName = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    CustomIdentifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ShortCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentKind", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentPartnerReference",
                schema: "inv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LegalName = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    TaxIdType = table.Column<string>(type: "text", nullable: true),
                    TaxIdValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentPartnerReference", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SourceReference",
                schema: "common",
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
                name: "StashMavenOption",
                schema: "common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StashMavenOption", x => x.Id);
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
                    ShortCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stockpile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxDefinition",
                schema: "common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaxDefinitionId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                schema: "partnership",
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
                        principalSchema: "partnership",
                        principalTable: "Partner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessIdentifier",
                schema: "partnership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartnerId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Value = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "SequenceEntry",
                schema: "inv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SequenceGeneratorId = table.Column<int>(type: "integer", nullable: false),
                    Delimiter = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false),
                    NextValue = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SequenceEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SequenceEntry_SequenceGenerator_SequenceGeneratorId",
                        column: x => x.SequenceGeneratorId,
                        principalSchema: "inv",
                        principalTable: "SequenceGenerator",
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
                    SequenceNumber = table.Column<string>(type: "text", nullable: true),
                    Acceptance = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    KindId = table.Column<int>(type: "integer", nullable: false),
                    SourceReferenceId = table.Column<int>(type: "integer", nullable: true),
                    StockpileId = table.Column<int>(type: "integer", nullable: false),
                    PartnerRefSnapshotId = table.Column<int>(type: "integer", nullable: true),
                    PartnerId = table.Column<int>(type: "integer", nullable: true),
                    IssuedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShippedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipment_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalSchema: "partnership",
                        principalTable: "Partner",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipment_ShipmentKind_KindId",
                        column: x => x.KindId,
                        principalSchema: "inv",
                        principalTable: "ShipmentKind",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shipment_ShipmentPartnerReference_PartnerRefSnapshotId",
                        column: x => x.PartnerRefSnapshotId,
                        principalSchema: "inv",
                        principalTable: "ShipmentPartnerReference",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipment_SourceReference_SourceReferenceId",
                        column: x => x.SourceReferenceId,
                        principalSchema: "common",
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
                name: "Product",
                schema: "cat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Sku = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "integer", nullable: false),
                    BarCode = table.Column<string>(type: "text", nullable: true),
                    BrandId = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DefaultTaxDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    TaxDefinitionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Brand_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "cat",
                        principalTable: "Brand",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_TaxDefinition_DefaultTaxDefinitionId",
                        column: x => x.DefaultTaxDefinitionId,
                        principalSchema: "common",
                        principalTable: "TaxDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_TaxDefinition_TaxDefinitionId",
                        column: x => x.TaxDefinitionId,
                        principalSchema: "common",
                        principalTable: "TaxDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryItem",
                schema: "inv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryItemId = table.Column<string>(type: "text", nullable: false),
                    Sku = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "integer", nullable: false),
                    LastPurchasePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    StockpileId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "cat",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItem_Stockpile_StockpileId",
                        column: x => x.StockpileId,
                        principalSchema: "inv",
                        principalTable: "Stockpile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    InventoryItemId = table.Column<int>(type: "integer", nullable: false),
                    ShipmentId = table.Column<int>(type: "integer", nullable: false),
                    TaxDefinitionId = table.Column<int>(type: "integer", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_ShipmentRecord_TaxDefinition_TaxDefinitionId",
                        column: x => x.TaxDefinitionId,
                        principalSchema: "common",
                        principalTable: "TaxDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_PartnerId",
                schema: "partnership",
                table: "Address",
                column: "PartnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brand_ShortCode",
                schema: "cat",
                table: "Brand",
                column: "ShortCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessIdentifier_PartnerId",
                schema: "partnership",
                table: "BusinessIdentifier",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessIdentifier_Type_Value",
                schema: "partnership",
                table: "BusinessIdentifier",
                columns: new[] { "Type", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_ProductId",
                schema: "inv",
                table: "InventoryItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_StockpileId",
                schema: "inv",
                table: "InventoryItem",
                column: "StockpileId");

            migrationBuilder.CreateIndex(
                name: "IX_Partner_CustomIdentifier",
                schema: "partnership",
                table: "Partner",
                column: "CustomIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_BrandId",
                schema: "cat",
                table: "Product",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_DefaultTaxDefinitionId",
                schema: "cat",
                table: "Product",
                column: "DefaultTaxDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Sku",
                schema: "cat",
                table: "Product",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_TaxDefinitionId",
                schema: "cat",
                table: "Product",
                column: "TaxDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_SequenceEntry_Group_Delimiter",
                schema: "inv",
                table: "SequenceEntry",
                columns: new[] { "Group", "Delimiter" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SequenceEntry_SequenceGeneratorId",
                schema: "inv",
                table: "SequenceEntry",
                column: "SequenceGeneratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_KindId",
                schema: "inv",
                table: "Shipment",
                column: "KindId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_PartnerId",
                schema: "inv",
                table: "Shipment",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_PartnerRefSnapshotId",
                schema: "inv",
                table: "Shipment",
                column: "PartnerRefSnapshotId");

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
                name: "IX_ShipmentRecord_TaxDefinitionId",
                schema: "inv",
                table: "ShipmentRecord",
                column: "TaxDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Stockpile_ShortCode",
                schema: "inv",
                table: "Stockpile",
                column: "ShortCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address",
                schema: "partnership");

            migrationBuilder.DropTable(
                name: "BusinessIdentifier",
                schema: "partnership");

            migrationBuilder.DropTable(
                name: "CompanyOption",
                schema: "common");

            migrationBuilder.DropTable(
                name: "SequenceEntry",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "ShipmentRecord",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "StashMavenOption",
                schema: "common");

            migrationBuilder.DropTable(
                name: "SequenceGenerator",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "InventoryItem",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "Shipment",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "Product",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "Partner",
                schema: "partnership");

            migrationBuilder.DropTable(
                name: "ShipmentKind",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "ShipmentPartnerReference",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "SourceReference",
                schema: "common");

            migrationBuilder.DropTable(
                name: "Stockpile",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "Brand",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "TaxDefinition",
                schema: "common");
        }
    }
}
