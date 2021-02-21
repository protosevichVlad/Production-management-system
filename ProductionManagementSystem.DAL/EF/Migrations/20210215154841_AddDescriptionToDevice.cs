using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.DAL.EF.Migrations
{
    public partial class AddDescriptionToDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Devices");
        }
    }
}
