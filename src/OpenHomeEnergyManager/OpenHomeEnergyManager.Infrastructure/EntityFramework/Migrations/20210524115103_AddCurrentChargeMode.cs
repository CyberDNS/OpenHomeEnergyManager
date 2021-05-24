using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenHomeEnergyManager.Infrastructure.EntityFramework.Migrations
{
    public partial class AddCurrentChargeMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentChargeMode",
                table: "ChargePoints",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentChargeMode",
                table: "ChargePoints");
        }
    }
}
