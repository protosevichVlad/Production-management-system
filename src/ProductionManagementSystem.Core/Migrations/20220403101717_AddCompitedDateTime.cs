using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class AddCompitedDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created",
                table: "AltiumDB_ToDoNotes",
                newName: "CreatedDateTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDateTime",
                table: "AltiumDB_ToDoNotes",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDateTime",
                table: "AltiumDB_ToDoNotes");

            migrationBuilder.RenameColumn(
                name: "CreatedDateTime",
                table: "AltiumDB_ToDoNotes",
                newName: "Created");
        }
    }
}
