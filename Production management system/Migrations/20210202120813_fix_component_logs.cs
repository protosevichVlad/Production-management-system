using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Migrations
{
    public partial class fix_component_logs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogsDesign_Designs_DesignId",
                table: "LogsDesign");

            migrationBuilder.AlterColumn<int>(
                name: "DesignId",
                table: "LogsDesign",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_LogsDesign_Designs_DesignId",
                table: "LogsDesign",
                column: "DesignId",
                principalTable: "Designs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogsDesign_Designs_DesignId",
                table: "LogsDesign");

            migrationBuilder.AlterColumn<int>(
                name: "DesignId",
                table: "LogsDesign",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LogsDesign_Designs_DesignId",
                table: "LogsDesign",
                column: "DesignId",
                principalTable: "Designs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
