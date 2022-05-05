using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class AddUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO master.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('505af43d-749d-4d0b-976c-cf14b11f1bbb', 'Инженер', 'ИНЖЕНЕР', '14c5f774-b32e-424c-92d6-0089aaf52d4c');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
