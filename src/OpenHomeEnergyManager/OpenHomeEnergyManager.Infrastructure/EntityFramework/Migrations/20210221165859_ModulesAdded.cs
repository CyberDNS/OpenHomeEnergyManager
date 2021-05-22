using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenHomeEnergyManager.Infrastructure.EntityFramework.Migrations
{
    public partial class ModulesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ChargePoints",
                schema: "ChargePointManagement",
                newName: "ChargePoints");

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ModuleServiceDefinitionKey = table.Column<string>(type: "TEXT", nullable: true),
                    Settings = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.EnsureSchema(
                name: "ChargePointManagement");

            migrationBuilder.RenameTable(
                name: "ChargePoints",
                newName: "ChargePoints",
                newSchema: "ChargePointManagement");
        }
    }
}
