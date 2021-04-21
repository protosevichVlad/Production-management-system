using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.DAL.EF.Migrations
{
    public partial class NewLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogComponents");

            migrationBuilder.DropTable(
                name: "LogDesigns");

            migrationBuilder.AddColumn<int>(
                name: "ComponentId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DesignId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "Logs",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComponentId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "DesignId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Logs");

            migrationBuilder.CreateTable(
                name: "LogComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComponentId = table.Column<int>(type: "int", nullable: true),
                    LogId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogComponents_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogComponents_Logs_LogId",
                        column: x => x.LogId,
                        principalTable: "Logs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogDesigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignId = table.Column<int>(type: "int", nullable: true),
                    LogId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogDesigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogDesigns_Designs_DesignId",
                        column: x => x.DesignId,
                        principalTable: "Designs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogDesigns_Logs_LogId",
                        column: x => x.LogId,
                        principalTable: "Logs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogComponents_ComponentId",
                table: "LogComponents",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_LogComponents_LogId",
                table: "LogComponents",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_LogDesigns_DesignId",
                table: "LogDesigns",
                column: "DesignId");

            migrationBuilder.CreateIndex(
                name: "IX_LogDesigns_LogId",
                table: "LogDesigns",
                column: "LogId");
        }
    }
}
