using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class AddCreatedByAndCompitedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletedById",
                table: "AltiumDB_ToDoNotes",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "AltiumDB_ToDoNotes",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AltiumDB_ToDoNotes_CompletedById",
                table: "AltiumDB_ToDoNotes",
                column: "CompletedById");

            migrationBuilder.CreateIndex(
                name: "IX_AltiumDB_ToDoNotes_CreatedById",
                table: "AltiumDB_ToDoNotes",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AltiumDB_ToDoNotes_AspNetUsers_CompletedById",
                table: "AltiumDB_ToDoNotes",
                column: "CompletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AltiumDB_ToDoNotes_AspNetUsers_CreatedById",
                table: "AltiumDB_ToDoNotes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AltiumDB_ToDoNotes_AspNetUsers_CompletedById",
                table: "AltiumDB_ToDoNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_AltiumDB_ToDoNotes_AspNetUsers_CreatedById",
                table: "AltiumDB_ToDoNotes");

            migrationBuilder.DropIndex(
                name: "IX_AltiumDB_ToDoNotes_CompletedById",
                table: "AltiumDB_ToDoNotes");

            migrationBuilder.DropIndex(
                name: "IX_AltiumDB_ToDoNotes_CreatedById",
                table: "AltiumDB_ToDoNotes");

            migrationBuilder.DropColumn(
                name: "CompletedById",
                table: "AltiumDB_ToDoNotes");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "AltiumDB_ToDoNotes");
        }
    }
}
