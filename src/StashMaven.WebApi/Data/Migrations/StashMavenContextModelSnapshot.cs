﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StashMaven.WebApi.Data;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    [DbContext(typeof(StashMavenContext))]
    partial class StashMavenContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("StashMaven.WebApi.Data.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PartnerId")
                        .HasColumnType("integer");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("State")
                        .HasColumnType("text");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StreetAdditional")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PartnerId")
                        .IsUnique();

                    b.ToTable("Address", "prt");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Brand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShortCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Brand", "cat");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.CatalogItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BarCode")
                        .HasColumnType("text");

                    b.Property<int?>("BrandId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Sku")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<int>("UnitOfMeasure")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.ComplexProperty<Dictionary<string, object>>("BuyTax", "StashMaven.WebApi.Data.CatalogItem.BuyTax#CatalogItemTaxReference", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<decimal>("Rate")
                                .HasColumnType("numeric");

                            b1.Property<string>("TaxDefinitionIdValue")
                                .IsRequired()
                                .HasColumnType("text");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("SellTax", "StashMaven.WebApi.Data.CatalogItem.SellTax#CatalogItemTaxReference", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<decimal>("Rate")
                                .HasColumnType("numeric");

                            b1.Property<string>("TaxDefinitionIdValue")
                                .IsRequired()
                                .HasColumnType("text");
                        });

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("Sku")
                        .IsUnique();

                    b.ToTable("CatalogItem", "cat");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.CompanyOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CompanyOption", "com");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.InventoryItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("LastPurchasePrice")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("numeric");

                    b.Property<string>("Sku")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("StockpileId")
                        .HasColumnType("integer");

                    b.Property<int>("UnitOfMeasure")
                        .HasColumnType("integer");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.ComplexProperty<Dictionary<string, object>>("BuyTax", "StashMaven.WebApi.Data.InventoryItem.BuyTax#InventoryItemTaxReference", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<decimal>("Rate")
                                .HasColumnType("numeric");

                            b1.Property<string>("TaxDefintionIdValue")
                                .IsRequired()
                                .HasColumnType("text");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("SellTax", "StashMaven.WebApi.Data.InventoryItem.SellTax#InventoryItemTaxReference", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<decimal>("Rate")
                                .HasColumnType("numeric");

                            b1.Property<string>("TaxDefintionIdValue")
                                .IsRequired()
                                .HasColumnType("text");
                        });

                    b.HasKey("Id");

                    b.HasIndex("StockpileId");

                    b.ToTable("InventoryItem", "inv");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Partner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomIdentifier")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("LegalName")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Partner", "prt");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.PartnerRefSnapshot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LegalName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ShipmentPartnerReference", "inv");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.SequenceEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Delimiter")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NextValue")
                        .HasColumnType("integer");

                    b.Property<int>("SequenceGeneratorId")
                        .HasColumnType("integer");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.HasIndex("SequenceGeneratorId");

                    b.HasIndex("Group", "Delimiter")
                        .IsUnique();

                    b.ToTable("SequenceEntry", "inv");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.SequenceGenerator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.ToTable("SequenceGenerator", "inv");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Shipment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Acceptance")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Currency")
                        .HasColumnType("integer");

                    b.Property<DateTime>("IssuedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("KindId")
                        .HasColumnType("integer");

                    b.Property<int?>("PartnerId")
                        .HasColumnType("integer");

                    b.Property<int?>("PartnerRefSnapshotId")
                        .HasColumnType("integer");

                    b.Property<int?>("SourceReferenceId")
                        .HasColumnType("integer");

                    b.Property<int>("StockpileId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("KindId");

                    b.HasIndex("PartnerId");

                    b.HasIndex("PartnerRefSnapshotId");

                    b.HasIndex("SourceReferenceId");

                    b.HasIndex("StockpileId");

                    b.ToTable("Shipment", "inv");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.ShipmentKind", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Direction")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShortCode")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)");

                    b.HasKey("Id");

                    b.HasIndex("ShortCode")
                        .IsUnique();

                    b.ToTable("ShipmentKind", "inv");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.ShipmentRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("InventoryItemId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("numeric");

                    b.Property<int>("ShipmentId")
                        .HasColumnType("integer");

                    b.Property<decimal>("TaxRate")
                        .HasColumnType("numeric");

                    b.Property<int>("UnitOfMeasure")
                        .HasColumnType("integer");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("InventoryItemId");

                    b.HasIndex("ShipmentId");

                    b.ToTable("ShipmentRecord", "inv");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.SourceReference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SourceReference", "inv");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.StashMavenOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("StashMavenOption", "com");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Stockpile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShortCode")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)");

                    b.HasKey("Id");

                    b.HasIndex("ShortCode")
                        .IsUnique();

                    b.ToTable("Stockpile", "inv");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.TaxDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Rate")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("TaxDefinition", "com");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.TaxIdentifier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsPrimary")
                        .HasColumnType("boolean");

                    b.Property<int>("PartnerId")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PartnerId");

                    b.ToTable("TaxIdentifier", "prt");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Address", b =>
                {
                    b.HasOne("StashMaven.WebApi.Data.Partner", "Partner")
                        .WithOne("Address")
                        .HasForeignKey("StashMaven.WebApi.Data.Address", "PartnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Partner");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Brand", b =>
                {
                    b.OwnsOne("StashMaven.WebApi.Data.BrandId", "BrandId", b1 =>
                        {
                            b1.Property<int>("BrandId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("BrandId");

                            b1.HasKey("BrandId");

                            b1.ToTable("Brand", "cat");

                            b1.WithOwner()
                                .HasForeignKey("BrandId");
                        });

                    b.Navigation("BrandId")
                        .IsRequired();
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.CatalogItem", b =>
                {
                    b.HasOne("StashMaven.WebApi.Data.Brand", "Brand")
                        .WithMany("CatalogItems")
                        .HasForeignKey("BrandId");

                    b.OwnsOne("StashMaven.WebApi.Data.CatalogItemId", "CatalogItemId", b1 =>
                        {
                            b1.Property<int>("CatalogItemId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("CatalogItemId");

                            b1.HasKey("CatalogItemId");

                            b1.ToTable("CatalogItem", "cat");

                            b1.WithOwner()
                                .HasForeignKey("CatalogItemId");
                        });

                    b.Navigation("Brand");

                    b.Navigation("CatalogItemId")
                        .IsRequired();
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.InventoryItem", b =>
                {
                    b.HasOne("StashMaven.WebApi.Data.Stockpile", "Stockpile")
                        .WithMany("InventoryItems")
                        .HasForeignKey("StockpileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("StashMaven.WebApi.Data.InventoryItemId", "InventoryItemId", b1 =>
                        {
                            b1.Property<int>("InventoryItemId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("InventoryItemId");

                            b1.HasKey("InventoryItemId");

                            b1.ToTable("InventoryItem", "inv");

                            b1.WithOwner()
                                .HasForeignKey("InventoryItemId");
                        });

                    b.Navigation("InventoryItemId")
                        .IsRequired();

                    b.Navigation("Stockpile");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Partner", b =>
                {
                    b.OwnsOne("StashMaven.WebApi.Data.PartnerId", "PartnerId", b1 =>
                        {
                            b1.Property<int>("PartnerId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("PartnerId");

                            b1.HasKey("PartnerId");

                            b1.ToTable("Partner", "prt");

                            b1.WithOwner()
                                .HasForeignKey("PartnerId");
                        });

                    b.Navigation("PartnerId")
                        .IsRequired();
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.SequenceEntry", b =>
                {
                    b.HasOne("StashMaven.WebApi.Data.SequenceGenerator", "SequenceGenerator")
                        .WithMany("Entries")
                        .HasForeignKey("SequenceGeneratorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SequenceGenerator");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.SequenceGenerator", b =>
                {
                    b.OwnsOne("StashMaven.WebApi.Data.SequenceGeneratorId", "SequenceGeneratorId", b1 =>
                        {
                            b1.Property<int>("SequenceGeneratorId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("SequenceGeneratorId");

                            b1.HasKey("SequenceGeneratorId");

                            b1.ToTable("SequenceGenerator", "inv");

                            b1.WithOwner()
                                .HasForeignKey("SequenceGeneratorId");
                        });

                    b.Navigation("SequenceGeneratorId")
                        .IsRequired();
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Shipment", b =>
                {
                    b.HasOne("StashMaven.WebApi.Data.ShipmentKind", "Kind")
                        .WithMany("Shipments")
                        .HasForeignKey("KindId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StashMaven.WebApi.Data.Partner", "Partner")
                        .WithMany("Shipments")
                        .HasForeignKey("PartnerId");

                    b.HasOne("StashMaven.WebApi.Data.PartnerRefSnapshot", "PartnerRefSnapshot")
                        .WithMany()
                        .HasForeignKey("PartnerRefSnapshotId");

                    b.HasOne("StashMaven.WebApi.Data.SourceReference", "SourceReference")
                        .WithMany()
                        .HasForeignKey("SourceReferenceId");

                    b.HasOne("StashMaven.WebApi.Data.Stockpile", "Stockpile")
                        .WithMany("Shipments")
                        .HasForeignKey("StockpileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("StashMaven.WebApi.Data.ShipmentId", "ShipmentId", b1 =>
                        {
                            b1.Property<int>("ShipmentId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("ShipmentId");

                            b1.HasKey("ShipmentId");

                            b1.ToTable("Shipment", "inv");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId");
                        });

                    b.OwnsOne("StashMaven.WebApi.Data.ShipmentSequenceNumber", "SequenceNumber", b1 =>
                        {
                            b1.Property<int>("ShipmentId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("SequenceNumber");

                            b1.HasKey("ShipmentId");

                            b1.ToTable("Shipment", "inv");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId");
                        });

                    b.Navigation("Kind");

                    b.Navigation("Partner");

                    b.Navigation("PartnerRefSnapshot");

                    b.Navigation("SequenceNumber");

                    b.Navigation("ShipmentId")
                        .IsRequired();

                    b.Navigation("SourceReference");

                    b.Navigation("Stockpile");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.ShipmentKind", b =>
                {
                    b.OwnsOne("StashMaven.WebApi.Data.SequenceGeneratorId", "SequenceGeneratorId", b1 =>
                        {
                            b1.Property<int>("ShipmentKindId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("SequenceGeneratorId");

                            b1.HasKey("ShipmentKindId");

                            b1.ToTable("ShipmentKind", "inv");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentKindId");
                        });

                    b.OwnsOne("StashMaven.WebApi.Data.ShipmentKindId", "ShipmentKindId", b1 =>
                        {
                            b1.Property<int>("ShipmentKindId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("ShipmentKindId");

                            b1.HasKey("ShipmentKindId");

                            b1.ToTable("ShipmentKind", "inv");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentKindId");
                        });

                    b.Navigation("SequenceGeneratorId")
                        .IsRequired();

                    b.Navigation("ShipmentKindId")
                        .IsRequired();
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.ShipmentRecord", b =>
                {
                    b.HasOne("StashMaven.WebApi.Data.InventoryItem", "InventoryItem")
                        .WithMany("ShipmentRecords")
                        .HasForeignKey("InventoryItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StashMaven.WebApi.Data.Shipment", "Shipment")
                        .WithMany("Records")
                        .HasForeignKey("ShipmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InventoryItem");

                    b.Navigation("Shipment");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Stockpile", b =>
                {
                    b.OwnsOne("StashMaven.WebApi.Data.StockpileId", "StockpileId", b1 =>
                        {
                            b1.Property<int>("StockpileId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("StockpileId");

                            b1.HasKey("StockpileId");

                            b1.ToTable("Stockpile", "inv");

                            b1.WithOwner()
                                .HasForeignKey("StockpileId");
                        });

                    b.Navigation("StockpileId")
                        .IsRequired();
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.TaxDefinition", b =>
                {
                    b.OwnsOne("StashMaven.WebApi.Data.TaxDefinitionId", "TaxDefinitionId", b1 =>
                        {
                            b1.Property<int>("TaxDefinitionId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("TaxDefinitionId");

                            b1.HasKey("TaxDefinitionId");

                            b1.ToTable("TaxDefinition", "com");

                            b1.WithOwner()
                                .HasForeignKey("TaxDefinitionId");
                        });

                    b.Navigation("TaxDefinitionId")
                        .IsRequired();
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.TaxIdentifier", b =>
                {
                    b.HasOne("StashMaven.WebApi.Data.Partner", "Partner")
                        .WithMany("TaxIdentifiers")
                        .HasForeignKey("PartnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Partner");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Brand", b =>
                {
                    b.Navigation("CatalogItems");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.InventoryItem", b =>
                {
                    b.Navigation("ShipmentRecords");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Partner", b =>
                {
                    b.Navigation("Address");

                    b.Navigation("Shipments");

                    b.Navigation("TaxIdentifiers");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.SequenceGenerator", b =>
                {
                    b.Navigation("Entries");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Shipment", b =>
                {
                    b.Navigation("Records");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.ShipmentKind", b =>
                {
                    b.Navigation("Shipments");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Stockpile", b =>
                {
                    b.Navigation("InventoryItems");

                    b.Navigation("Shipments");
                });
#pragma warning restore 612, 618
        }
    }
}
