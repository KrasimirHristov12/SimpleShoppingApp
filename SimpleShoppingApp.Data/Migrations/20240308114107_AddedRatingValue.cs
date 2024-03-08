using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleShoppingApp.Data.Migrations
{
    public partial class AddedRatingValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "UsersRatings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "UsersRatings");
        }
    }
}
