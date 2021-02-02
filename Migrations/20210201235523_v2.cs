using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SHOP_DECOMPILED.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Creditors",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Customer_name = table.Column<string>(nullable: false),
                    Phone_number = table.Column<int>(nullable: false),
                    Credit = table.Column<float>(nullable: false),
                    Date_created = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creditors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Credits",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Client_id = table.Column<int>(nullable: false),
                    Item = table.Column<string>(nullable: true),
                    Quantity = table.Column<string>(nullable: true),
                    Total = table.Column<float>(nullable: false),
                    Date_created = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credits", x => x.id);
                });

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
                name: "Payment_history",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Client_id = table.Column<int>(nullable: false),
                    Ammount_paid = table.Column<float>(nullable: false),
                    Balance = table.Column<float>(nullable: false),
                    Date_created = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment_history", x => x.id);
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
                    DateTime = table.Column<string>(nullable: false),
                    Cost_price = table.Column<float>(nullable: false)
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
                    Cost_cash = table.Column<float>(nullable: false),
                    Total_Cost_cash = table.Column<float>(nullable: false),
                    Profit = table.Column<float>(nullable: false),
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
                name: "Creditors");

            migrationBuilder.DropTable(
                name: "Credits");

            migrationBuilder.DropTable(
                name: "Log_in");

            migrationBuilder.DropTable(
                name: "Payment_history");

            migrationBuilder.DropTable(
                name: "Restock_history");

            migrationBuilder.DropTable(
                name: "Shop_items");

            migrationBuilder.DropTable(
                name: "sold_items");
        }
    }
}
