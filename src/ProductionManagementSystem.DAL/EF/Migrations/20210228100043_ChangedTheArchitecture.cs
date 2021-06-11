using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.DAL.EF.Migrations
{
    public partial class ChangedTheArchitecture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObtainedСomponents_Components_ComponentId",
                table: "ObtainedСomponents");

            migrationBuilder.DropForeignKey(
                name: "FK_ObtainedСomponents_Tasks_TaskId",
                table: "ObtainedСomponents");

            migrationBuilder.DropForeignKey(
                name: "FK_ObtainedDesigns_Designs_DesignId",
                table: "ObtainedDesigns");

            migrationBuilder.DropForeignKey(
                name: "FK_ObtainedDesigns_Tasks_TaskId",
                table: "ObtainedDesigns");

            migrationBuilder.DropTable(
                name: "DeviceComponentsTemplate");

            migrationBuilder.DropTable(
                name: "DeviceDesignTemplate");

            migrationBuilder.DropTable(
                name: "DeviceInTask");

            migrationBuilder.DropTable(
                name: "LogsComponent");

            migrationBuilder.DropTable(
                name: "LogsDesign");

            migrationBuilder.DropTable(
                name: "OrderComponents");

            migrationBuilder.DropTable(
                name: "OrderDesign");

            migrationBuilder.RenameColumn(
                name: "Customer",
                table: "Tasks",
                newName: "Description");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "ObtainedDesigns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DesignId",
                table: "ObtainedDesigns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "ObtainedСomponents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ComponentId",
                table: "ObtainedСomponents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Designs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Designs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Components",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "DeviceComponentsTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceComponentsTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceComponentsTemplates_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceComponentsTemplates_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceDesignTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DesignId = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDesignTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceDesignTemplates_Designs_DesignId",
                        column: x => x.DesignId,
                        principalTable: "Designs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceDesignTemplates_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DeviceId",
                table: "Tasks",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OrderId",
                table: "Tasks",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComponentsTemplates_ComponentId",
                table: "DeviceComponentsTemplates",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComponentsTemplates_DeviceId",
                table: "DeviceComponentsTemplates",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDesignTemplates_DesignId",
                table: "DeviceDesignTemplates",
                column: "DesignId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDesignTemplates_DeviceId",
                table: "DeviceDesignTemplates",
                column: "DeviceId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ObtainedСomponents_Components_ComponentId",
                table: "ObtainedСomponents",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObtainedСomponents_Tasks_TaskId",
                table: "ObtainedСomponents",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObtainedDesigns_Designs_DesignId",
                table: "ObtainedDesigns",
                column: "DesignId",
                principalTable: "Designs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObtainedDesigns_Tasks_TaskId",
                table: "ObtainedDesigns",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Devices_DeviceId",
                table: "Tasks",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Orders_OrderId",
                table: "Tasks",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObtainedСomponents_Components_ComponentId",
                table: "ObtainedСomponents");

            migrationBuilder.DropForeignKey(
                name: "FK_ObtainedСomponents_Tasks_TaskId",
                table: "ObtainedСomponents");

            migrationBuilder.DropForeignKey(
                name: "FK_ObtainedDesigns_Designs_DesignId",
                table: "ObtainedDesigns");

            migrationBuilder.DropForeignKey(
                name: "FK_ObtainedDesigns_Tasks_TaskId",
                table: "ObtainedDesigns");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Devices_DeviceId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Orders_OrderId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "DeviceComponentsTemplates");

            migrationBuilder.DropTable(
                name: "DeviceDesignTemplates");

            migrationBuilder.DropTable(
                name: "LogComponents");

            migrationBuilder.DropTable(
                name: "LogDesigns");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DeviceId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_OrderId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Tasks",
                newName: "Customer");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "ObtainedDesigns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DesignId",
                table: "ObtainedDesigns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "ObtainedСomponents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ComponentId",
                table: "ObtainedСomponents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Designs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Designs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Components",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DeviceComponentsTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComponentId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceComponentsTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceComponentsTemplate_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeviceComponentsTemplate_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceDesignTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesignId = table.Column<int>(type: "int", nullable: true),
                    DeviceId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDesignTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceDesignTemplate_Designs_DesignId",
                        column: x => x.DesignId,
                        principalTable: "Designs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeviceDesignTemplate_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceInTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceInTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceInTask_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeviceInTask_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogsComponent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComponentId = table.Column<int>(type: "int", nullable: true),
                    LogId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogsComponent_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogsComponent_Logs_LogId",
                        column: x => x.LogId,
                        principalTable: "Logs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogsDesign",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignId = table.Column<int>(type: "int", nullable: true),
                    LogId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsDesign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogsDesign_Designs_DesignId",
                        column: x => x.DesignId,
                        principalTable: "Designs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogsDesign_Logs_LogId",
                        column: x => x.LogId,
                        principalTable: "Logs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComponentId = table.Column<int>(type: "int", nullable: true),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderComponents_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderComponents_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDesign",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesignId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDesign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDesign_Designs_DesignId",
                        column: x => x.DesignId,
                        principalTable: "Designs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDesign_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComponentsTemplate_ComponentId",
                table: "DeviceComponentsTemplate",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComponentsTemplate_DeviceId",
                table: "DeviceComponentsTemplate",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDesignTemplate_DesignId",
                table: "DeviceDesignTemplate",
                column: "DesignId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDesignTemplate_DeviceId",
                table: "DeviceDesignTemplate",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceInTask_DeviceId",
                table: "DeviceInTask",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceInTask_TaskId",
                table: "DeviceInTask",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsComponent_ComponentId",
                table: "LogsComponent",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsComponent_LogId",
                table: "LogsComponent",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsDesign_DesignId",
                table: "LogsDesign",
                column: "DesignId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsDesign_LogId",
                table: "LogsDesign",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderComponents_ComponentId",
                table: "OrderComponents",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderComponents_TaskId",
                table: "OrderComponents",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDesign_DesignId",
                table: "OrderDesign",
                column: "DesignId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDesign_TaskId",
                table: "OrderDesign",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObtainedСomponents_Components_ComponentId",
                table: "ObtainedСomponents",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ObtainedСomponents_Tasks_TaskId",
                table: "ObtainedСomponents",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ObtainedDesigns_Designs_DesignId",
                table: "ObtainedDesigns",
                column: "DesignId",
                principalTable: "Designs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ObtainedDesigns_Tasks_TaskId",
                table: "ObtainedDesigns",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
