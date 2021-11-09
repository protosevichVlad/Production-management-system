using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.DAL.Migrations
{
    public partial class RemaneTablesAndColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComponentsSupplyRequests_AspNetUsers_UserId",
                table: "ComponentsSupplyRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ComponentsSupplyRequests_Components_ComponentId",
                table: "ComponentsSupplyRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ComponentsSupplyRequests_Tasks_TaskId",
                table: "ComponentsSupplyRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignsSupplyRequests_AspNetUsers_UserId",
                table: "DesignsSupplyRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignsSupplyRequests_Designs_DesignId",
                table: "DesignsSupplyRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignsSupplyRequests_Tasks_TaskId",
                table: "DesignsSupplyRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceComponentsTemplates_Components_ComponentId",
                table: "DeviceComponentsTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceComponentsTemplates_Devices_DeviceId",
                table: "DeviceComponentsTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceDesignTemplates_Designs_DesignId",
                table: "DeviceDesignTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceDesignTemplates_Devices_DeviceId",
                table: "DeviceDesignTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_ObtainedComponents_Components_ComponentId",
                table: "ObtainedComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_ObtainedComponents_Tasks_TaskId",
                table: "ObtainedComponents");

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

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DeviceId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_OrderId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_ObtainedDesigns_DesignId",
                table: "ObtainedDesigns");

            migrationBuilder.DropIndex(
                name: "IX_ObtainedDesigns_TaskId",
                table: "ObtainedDesigns");

            migrationBuilder.DropIndex(
                name: "IX_ObtainedComponents_ComponentId",
                table: "ObtainedComponents");

            migrationBuilder.DropIndex(
                name: "IX_ObtainedComponents_TaskId",
                table: "ObtainedComponents");

            migrationBuilder.DropIndex(
                name: "IX_DeviceDesignTemplates_DesignId",
                table: "DeviceDesignTemplates");

            migrationBuilder.DropIndex(
                name: "IX_DeviceDesignTemplates_DeviceId",
                table: "DeviceDesignTemplates");

            migrationBuilder.DropIndex(
                name: "IX_DeviceComponentsTemplates_ComponentId",
                table: "DeviceComponentsTemplates");

            migrationBuilder.DropIndex(
                name: "IX_DeviceComponentsTemplates_DeviceId",
                table: "DeviceComponentsTemplates");

            migrationBuilder.DropIndex(
                name: "IX_DesignsSupplyRequests_DesignId",
                table: "DesignsSupplyRequests");

            migrationBuilder.DropIndex(
                name: "IX_DesignsSupplyRequests_TaskId",
                table: "DesignsSupplyRequests");

            migrationBuilder.DropIndex(
                name: "IX_DesignsSupplyRequests_UserId",
                table: "DesignsSupplyRequests");

            migrationBuilder.DropIndex(
                name: "IX_ComponentsSupplyRequests_ComponentId",
                table: "ComponentsSupplyRequests");

            migrationBuilder.DropIndex(
                name: "IX_ComponentsSupplyRequests_TaskId",
                table: "ComponentsSupplyRequests");

            migrationBuilder.DropIndex(
                name: "IX_ComponentsSupplyRequests_UserId",
                table: "ComponentsSupplyRequests");

            migrationBuilder.RenameColumn(
                name: "DesignId",
                table: "ObtainedDesigns",
                newName: "ComponentId");

            migrationBuilder.RenameColumn(
                name: "UserLogin",
                table: "Logs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ComponentId",
                table: "Logs",
                newName: "MontageSupplyRequestId");

            migrationBuilder.RenameColumn(
                name: "DesignId",
                table: "DeviceDesignTemplates",
                newName: "ComponentId");

            migrationBuilder.RenameColumn(
                name: "DesignId",
                table: "DesignsSupplyRequests",
                newName: "ComponentId");

            migrationBuilder.AddColumn<int>(
                name: "DesignSupplyRequestId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MontageId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DesignsSupplyRequests",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ComponentsSupplyRequests",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesignSupplyRequestId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "MontageId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ComponentId",
                table: "ObtainedDesigns",
                newName: "DesignId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Logs",
                newName: "UserLogin");

            migrationBuilder.RenameColumn(
                name: "MontageSupplyRequestId",
                table: "Logs",
                newName: "ComponentId");

            migrationBuilder.RenameColumn(
                name: "ComponentId",
                table: "DeviceDesignTemplates",
                newName: "DesignId");

            migrationBuilder.RenameColumn(
                name: "ComponentId",
                table: "DesignsSupplyRequests",
                newName: "DesignId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DesignsSupplyRequests",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ComponentsSupplyRequests",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DeviceId",
                table: "Tasks",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OrderId",
                table: "Tasks",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ObtainedDesigns_DesignId",
                table: "ObtainedDesigns",
                column: "DesignId");

            migrationBuilder.CreateIndex(
                name: "IX_ObtainedDesigns_TaskId",
                table: "ObtainedDesigns",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ObtainedComponents_ComponentId",
                table: "ObtainedComponents",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObtainedComponents_TaskId",
                table: "ObtainedComponents",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDesignTemplates_DesignId",
                table: "DeviceDesignTemplates",
                column: "DesignId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDesignTemplates_DeviceId",
                table: "DeviceDesignTemplates",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComponentsTemplates_ComponentId",
                table: "DeviceComponentsTemplates",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComponentsTemplates_DeviceId",
                table: "DeviceComponentsTemplates",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignsSupplyRequests_DesignId",
                table: "DesignsSupplyRequests",
                column: "DesignId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignsSupplyRequests_TaskId",
                table: "DesignsSupplyRequests",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignsSupplyRequests_UserId",
                table: "DesignsSupplyRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentsSupplyRequests_ComponentId",
                table: "ComponentsSupplyRequests",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentsSupplyRequests_TaskId",
                table: "ComponentsSupplyRequests",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentsSupplyRequests_UserId",
                table: "ComponentsSupplyRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentsSupplyRequests_AspNetUsers_UserId",
                table: "ComponentsSupplyRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentsSupplyRequests_Components_ComponentId",
                table: "ComponentsSupplyRequests",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentsSupplyRequests_Tasks_TaskId",
                table: "ComponentsSupplyRequests",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DesignsSupplyRequests_AspNetUsers_UserId",
                table: "DesignsSupplyRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DesignsSupplyRequests_Designs_DesignId",
                table: "DesignsSupplyRequests",
                column: "DesignId",
                principalTable: "Designs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DesignsSupplyRequests_Tasks_TaskId",
                table: "DesignsSupplyRequests",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceComponentsTemplates_Components_ComponentId",
                table: "DeviceComponentsTemplates",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceComponentsTemplates_Devices_DeviceId",
                table: "DeviceComponentsTemplates",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceDesignTemplates_Designs_DesignId",
                table: "DeviceDesignTemplates",
                column: "DesignId",
                principalTable: "Designs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceDesignTemplates_Devices_DeviceId",
                table: "DeviceDesignTemplates",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObtainedComponents_Components_ComponentId",
                table: "ObtainedComponents",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObtainedComponents_Tasks_TaskId",
                table: "ObtainedComponents",
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
    }
}
