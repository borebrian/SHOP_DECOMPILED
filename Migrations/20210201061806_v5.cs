using Microsoft.EntityFrameworkCore.Migrations;

namespace SHOP_DECOMPILED.Migrations
{
    public partial class v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Creditors_account",
                table: "Creditors_account");

            migrationBuilder.RenameTable(
                name: "Creditors_account",
                newName: "Creditors");

            migrationBuilder.AlterColumn<int>(
                name: "Phone_number",
                table: "Creditors",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Creditors",
                table: "Creditors",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Creditors",
                table: "Creditors");

            migrationBuilder.RenameTable(
                name: "Creditors",
                newName: "Creditors_account");

            migrationBuilder.AlterColumn<float>(
                name: "Phone_number",
                table: "Creditors_account",
                type: "float",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Creditors_account",
                table: "Creditors_account",
                column: "id");
        }
    }
}
