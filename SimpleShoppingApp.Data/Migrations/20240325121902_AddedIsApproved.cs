using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleShoppingApp.Data.Migrations
{
    public partial class AddedIsApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Products");
        }
    }
}
