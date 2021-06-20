using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenHomeEnergyManager.Infrastructure.EntityFramework.Migrations
{
    public partial class RemovedImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "ChargePoints");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Vehicles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "ChargePoints",
                type: "TEXT",
                nullable: true);
        }
    }
}
