using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopGeocoder.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                schema: "dbo",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Email = table.Column<string>(nullable: false),
                    Key = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                schema: "dbo",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    AddressExt = table.Column<string>(nullable: true),
                    Longitude = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Guid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Shops",
                schema: "dbo");
        }
    }
}
