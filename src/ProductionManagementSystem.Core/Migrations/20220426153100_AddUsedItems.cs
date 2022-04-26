using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class AddUsedItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityInPCB_PCB_PcbId",
                table: "EntityInPCB");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityInPCB",
                table: "EntityInPCB");

            migrationBuilder.DropIndex(
                name: "IX_EntityInPCB_PcbId",
                table: "EntityInPCB");

            migrationBuilder.RenameTable(
                name: "EntityInPCB",
                newName: "UsedItems");

            migrationBuilder.RenameColumn(
                name: "PcbId",
                table: "UsedItems",
                newName: "ItemType");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "UsedItems",
                newName: "ItemId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "UsedItems",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "InItemId",
                table: "UsedItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InItemType",
                table: "UsedItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsedItems",
                table: "UsedItems",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CDBTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentTaskId = table.Column<int>(type: "int", nullable: false),
                    TaskItemId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CDBTasks", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CDBObtained",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    ObtainedId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CDBObtained", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CDBObtained_CDBTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "CDBTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CDBObtained_TaskId",
                table: "CDBObtained",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CDBObtained");

            migrationBuilder.DropTable(
                name: "CDBTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsedItems",
                table: "UsedItems");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "UsedItems");

            migrationBuilder.DropColumn(
                name: "InItemId",
                table: "UsedItems");

            migrationBuilder.DropColumn(
                name: "InItemType",
                table: "UsedItems");

            migrationBuilder.RenameTable(
                name: "UsedItems",
                newName: "EntityInPCB");

            migrationBuilder.RenameColumn(
                name: "ItemType",
                table: "EntityInPCB",
                newName: "PcbId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "EntityInPCB",
                newName: "EntityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityInPCB",
                table: "EntityInPCB",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EntityInPCB_PcbId",
                table: "EntityInPCB",
                column: "PcbId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityInPCB_PCB_PcbId",
                table: "EntityInPCB",
                column: "PcbId",
                principalTable: "PCB",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
