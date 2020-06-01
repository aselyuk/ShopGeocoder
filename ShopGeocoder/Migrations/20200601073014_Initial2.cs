using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopGeocoder.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Default",
                schema: "dbo",
                table: "ApiKeys",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Default",
                schema: "dbo",
                table: "ApiKeys");
        }
    }
}
