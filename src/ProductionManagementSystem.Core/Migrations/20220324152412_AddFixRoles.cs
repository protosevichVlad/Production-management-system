using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class AddFixRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_AspNetUsers_UserId",
                table: "Logs");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.Sql(@"INSERT INTO master.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('40cbc9fa-538c-4215-bbb9-bf1e14809415', 'AltiumBD Tables Admin', 'ALTIUMBD TABLES ADMIN', '1d5922fe-eece-451d-8143-e01c0fe13243');
                                    INSERT INTO master.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('42aa18ed-d5e1-4bba-afa8-b7efd8cc81f9', 'AltiumBD Entities Admin', 'ALTIUMBD ENTITIES ADMIN', '514b3ef3-4648-45a0-a96f-f2dd3d5181f3');
                                    INSERT INTO master.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('bbb84ef7-ebb4-4ad0-8937-4f71464c0a26', 'Супер админ', 'СУПЕР АДМИН', 'f2b88d05-5815-4015-b9aa-9da0c1a2bf12');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_AspNetUsers_UserId",
                table: "Logs");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
