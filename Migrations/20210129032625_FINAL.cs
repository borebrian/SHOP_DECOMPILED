﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SHOP_DECOMPILED.Migrations
{
    public partial class FINAL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log_in",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Full_name = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Shop_name = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    strRole = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log_in", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Restock_history",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Item_id = table.Column<int>(nullable: false),
                    Date_restock = table.Column<string>(nullable: false),
                    new_quanity = table.Column<float>(nullable: false),
                    Prev_quantity = table.Column<float>(nullable: false),
                    quantity = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restock_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Shop_items",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Item_name = table.Column<string>(nullable: false),
                    Item_price = table.Column<float>(nullable: false),
                    Quantity = table.Column<float>(nullable: false),
                    DateTime = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shop_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sold_items",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Item_id = table.Column<int>(nullable: false),
                    quantity_sold = table.Column<float>(nullable: false),
                    Total_cash_made = table.Column<float>(nullable: false),
                    DateTime = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sold_items", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log_in");

            migrationBuilder.DropTable(
                name: "Restock_history");

            migrationBuilder.DropTable(
                name: "Shop_items");

            migrationBuilder.DropTable(
                name: "sold_items");
        }
    }
}
