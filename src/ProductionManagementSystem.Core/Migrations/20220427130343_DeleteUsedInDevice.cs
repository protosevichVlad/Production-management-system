using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class DeleteUsedInDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsedInDevice");

            migrationBuilder.AddColumn<int>(
                name: "CompDbDeviceId",
                table: "UsedItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsedItems_CompDbDeviceId",
                table: "UsedItems",
                column: "CompDbDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedItems_CompDB_Devices_CompDbDeviceId",
                table: "UsedItems",
                column: "CompDbDeviceId",
                principalTable: "CompDB_Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedItems_CompDB_Devices_CompDbDeviceId",
                table: "UsedItems");

            migrationBuilder.DropIndex(
                name: "IX_UsedItems_CompDbDeviceId",
                table: "UsedItems");

            migrationBuilder.DropColumn(
                name: "CompDbDeviceId",
                table: "UsedItems");

            migrationBuilder.CreateTable(
                name: "UsedInDevice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CompDbDeviceId = table.Column<int>(type: "int", nullable: false),
                    ComponentType = table.Column<int>(type: "int", nullable: false),
                    Designator = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UsedComponentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedInDevice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsedInDevice_CompDB_Devices_CompDbDeviceId",
                        column: x => x.CompDbDeviceId,
                        principalTable: "CompDB_Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UsedInDevice_CompDbDeviceId",
                table: "UsedInDevice",
                column: "CompDbDeviceId");
        }
    }
}
