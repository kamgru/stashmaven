﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StashMaven.WebApi.Data;

#nullable disable

namespace StashMaven.WebApi.Data.Migrations
{
    [DbContext(typeof(StashMavenContext))]
    [Migration("20231202191016_AddShipmentKindUnique")]
    partial class AddShipmentKindUnique
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
                        .HasColumnType("text");

                    b.Property<string>("Sku")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("TaxDefinitionId")
                        .HasColumnType("integer");

                    b.Property<int>("UnitOfMeasure")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("TaxDefinitionId");

                    b.ToTable("CatalogItem", "cat");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.InventoryItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("numeric");

                    b.Property<string>("Sku")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TaxDefinitionId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("TaxDefinitionId");

                    b.Property<int>("UnitOfMeasure")
                        .HasColumnType("integer");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("numeric");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

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
                        .HasColumnType("text");

                    b.Property<string>("LegalName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Partner", "prt");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Shipment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Currency")
                        .HasColumnType("integer");

                    b.Property<int>("ShipmentAcceptance")
                        .HasColumnType("integer");

                    b.Property<int>("ShipmentDirection")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Shipment", "inv");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.ShipmentKind", b =>
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

                    b.Property<decimal>("Quantity")
                        .HasColumnType("numeric");

                    b.Property<int?>("ShipmentId")
                        .HasColumnType("integer");

                    b.Property<decimal>("TaxRate")
                        .HasColumnType("numeric");

                    b.Property<int>("UnitOfMeasure")
                        .HasColumnType("integer");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("ShipmentId");

                    b.ToTable("ShipmentRecord", "inv");
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

                    b.ToTable("TaxDefinition", "cat");
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

                    b.HasOne("StashMaven.WebApi.Data.TaxDefinition", "TaxDefinition")
                        .WithMany()
                        .HasForeignKey("TaxDefinitionId");

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

                    b.Navigation("TaxDefinition");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.InventoryItem", b =>
                {
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

            modelBuilder.Entity("StashMaven.WebApi.Data.Shipment", b =>
                {
                    b.OwnsOne("StashMaven.WebApi.Data.ShipmentKindId", "ShipmentKindId", b1 =>
                        {
                            b1.Property<int>("ShipmentId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("ShipmentKindId");

                            b1.HasKey("ShipmentId");

                            b1.ToTable("Shipment", "inv");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId");
                        });

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

                    b.OwnsOne("StashMaven.WebApi.Data.SupplierId", "SupplierId", b1 =>
                        {
                            b1.Property<int>("ShipmentId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("SupplierId");

                            b1.HasKey("ShipmentId");

                            b1.ToTable("Shipment", "inv");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId");
                        });

                    b.Navigation("ShipmentId")
                        .IsRequired();

                    b.Navigation("ShipmentKindId")
                        .IsRequired();

                    b.Navigation("SupplierId");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.ShipmentKind", b =>
                {
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

                    b.Navigation("ShipmentKindId")
                        .IsRequired();
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.ShipmentRecord", b =>
                {
                    b.HasOne("StashMaven.WebApi.Data.Shipment", null)
                        .WithMany("ShipmentRecords")
                        .HasForeignKey("ShipmentId");

                    b.OwnsOne("StashMaven.WebApi.Data.InventoryItemId", "InventoryItemId", b1 =>
                        {
                            b1.Property<int>("ShipmentRecordId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("InventoryItemId");

                            b1.HasKey("ShipmentRecordId");

                            b1.ToTable("ShipmentRecord", "inv");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentRecordId");
                        });

                    b.Navigation("InventoryItemId")
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

                            b1.ToTable("TaxDefinition", "cat");

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

            modelBuilder.Entity("StashMaven.WebApi.Data.Partner", b =>
                {
                    b.Navigation("Address");

                    b.Navigation("TaxIdentifiers");
                });

            modelBuilder.Entity("StashMaven.WebApi.Data.Shipment", b =>
                {
                    b.Navigation("ShipmentRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
