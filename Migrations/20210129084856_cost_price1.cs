using Microsoft.EntityFrameworkCore.Migrations;

namespace SHOP_DECOMPILED.Migrations
{
    public partial class cost_price1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Cost_cash",
                table: "sold_items",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Profit",
                table: "sold_items",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Total_Cost_cash",
                table: "sold_items",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost_cash",
                table: "sold_items");

            migrationBuilder.DropColumn(
                name: "Profit",
                table: "sold_items");

            migrationBuilder.DropColumn(
                name: "Total_Cost_cash",
                table: "sold_items");
        }
    }
}
