﻿// <auto-generated />
using Lubes.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SHOP_DECOMPILED.Migrations.ApplicationDBContext_onlineMigrations
{
    [DbContext(typeof(ApplicationDBContext_online))]
    partial class ApplicationDBContext_onlineModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SHOP.Models.Creditors_account", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Credit")
                        .HasColumnType("float");

                    b.Property<string>("Customer_name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Date_created")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Phone_number")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("Creditors");
                });

            modelBuilder.Entity("SHOP.Models.Credits", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Client_id")
                        .HasColumnType("int");

                    b.Property<string>("Date_created")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Item")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Quantity")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<float>("Total")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.ToTable("Credits");
                });

            modelBuilder.Entity("SHOP.Models.Expiries", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Date_created")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Expiry_date")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Item_name")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("Expiries");
                });

            modelBuilder.Entity("SHOP.Models.Payment_history", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Ammount_paid")
                        .HasColumnType("float");

                    b.Property<float>("Balance")
                        .HasColumnType("float");

                    b.Property<int>("Client_id")
                        .HasColumnType("int");

                    b.Property<string>("Date_created")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("id");

                    b.ToTable("Payment_history");
                });

            modelBuilder.Entity("SHOP.Models.Restock_history", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Date_restock")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Item_id")
                        .HasColumnType("int");

                    b.Property<float>("Prev_quantity")
                        .HasColumnType("float");

                    b.Property<float>("new_quanity")
                        .HasColumnType("float");

                    b.Property<float>("quantity")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.ToTable("Restock_history");
                });

            modelBuilder.Entity("SHOP.Models.log_in", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Full_name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Shop_name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("strRole")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("Log_in");
                });

            modelBuilder.Entity("SHOP.Models.shop_items", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Cost_price")
                        .HasColumnType("float");

                    b.Property<string>("DateTime")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Item_name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<float>("Item_price")
                        .HasColumnType("float");

                    b.Property<float>("Quantity")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.ToTable("Shop_items");
                });

            modelBuilder.Entity("SHOP.Models.sold_items", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Cost_cash")
                        .HasColumnType("float");

                    b.Property<string>("DateTime")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Item_id")
                        .HasColumnType("int");

                    b.Property<float>("Profit")
                        .HasColumnType("float");

                    b.Property<float>("Total_Cost_cash")
                        .HasColumnType("float");

                    b.Property<float>("Total_cash_made")
                        .HasColumnType("float");

                    b.Property<float>("quantity_sold")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.ToTable("sold_items");
                });
#pragma warning restore 612, 618
        }
    }
}