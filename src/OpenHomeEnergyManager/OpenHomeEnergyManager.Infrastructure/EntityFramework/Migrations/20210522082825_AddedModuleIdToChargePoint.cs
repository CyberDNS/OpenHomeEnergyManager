using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenHomeEnergyManager.Infrastructure.EntityFramework.Migrations
{
    public partial class AddedModuleIdToChargePoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Host",
                table: "ChargePoints");

            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "ChargePoints",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "ChargePoints");

            migrationBuilder.AddColumn<string>(
                name: "Host",
                table: "ChargePoints",
                type: "TEXT",
                nullable: true);
        }
    }
}
