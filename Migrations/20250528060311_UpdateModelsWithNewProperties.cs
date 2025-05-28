using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pampazon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsWithNewProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispatches_Orders_OrderId",
                table: "Dispatches");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispatches_Orders_OrderNumber",
                table: "Dispatches");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Orders_OrderNumber",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Products_ProductId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItem_Products_ProductId",
                table: "ReceiptItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItem_Receipts_ReceiptId",
                table: "ReceiptItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Clients_ClientId",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Orders_OrderId",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Orders_OrderNumber",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_StockPositions_Clients_ClientId",
                table: "StockPositions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_OrderId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_OrderNumber",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Dispatches_OrderId",
                table: "Dispatches");

            migrationBuilder.DropIndex(
                name: "IX_Dispatches_OrderNumber",
                table: "Dispatches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptItem",
                table: "ReceiptItem");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptItem_ReceiptId",
                table: "ReceiptItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "CarrierCUIT",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Dispatches");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "ReceiptItem");

            migrationBuilder.RenameTable(
                name: "ReceiptItem",
                newName: "ReceiptItems");

            migrationBuilder.RenameTable(
                name: "OrderItem",
                newName: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Receipts",
                newName: "ReceiptNumber");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Dispatches",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptItem_ProductId",
                table: "ReceiptItems",
                newName: "IX_ReceiptItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_ProductId",
                table: "OrderItems",
                newName: "IX_OrderItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_OrderNumber",
                table: "OrderItems",
                newName: "IX_OrderItems_OrderNumber");

            migrationBuilder.AlterColumn<string>(
                name: "Shelf",
                table: "StockPositions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Section",
                table: "StockPositions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "StockPositions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "StockPositions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Aisle",
                table: "StockPositions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)");

            migrationBuilder.AddColumn<string>(
                name: "ClientCUIT",
                table: "StockPositions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StockPositions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ReceiptNumber",
                table: "StockPositions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Receipts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DispatchNumber1",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptNumber",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "Dispatches",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "Dispatches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Dispatches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptNumber",
                table: "ReceiptItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts",
                column: "ReceiptNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptItems",
                table: "ReceiptItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StockPositions_ClientCUIT",
                table: "StockPositions",
                column: "ClientCUIT");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DispatchNumber1",
                table: "Orders",
                column: "DispatchNumber1");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ReceiptNumber",
                table: "Orders",
                column: "ReceiptNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Dispatches_OrderNumber",
                table: "Dispatches",
                column: "OrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_ReceiptNumber",
                table: "ReceiptItems",
                column: "ReceiptNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispatches_Orders_OrderNumber",
                table: "Dispatches",
                column: "OrderNumber",
                principalTable: "Orders",
                principalColumn: "OrderNumber",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderNumber",
                table: "OrderItems",
                column: "OrderNumber",
                principalTable: "Orders",
                principalColumn: "OrderNumber",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Dispatches_DispatchNumber1",
                table: "Orders",
                column: "DispatchNumber1",
                principalTable: "Dispatches",
                principalColumn: "DispatchNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Receipts_ReceiptNumber",
                table: "Orders",
                column: "ReceiptNumber",
                principalTable: "Receipts",
                principalColumn: "ReceiptNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_Products_ProductId",
                table: "ReceiptItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_Receipts_ReceiptNumber",
                table: "ReceiptItems",
                column: "ReceiptNumber",
                principalTable: "Receipts",
                principalColumn: "ReceiptNumber",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Clients_ClientId",
                table: "Receipts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "CUIT",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockPositions_Clients_ClientCUIT",
                table: "StockPositions",
                column: "ClientCUIT",
                principalTable: "Clients",
                principalColumn: "CUIT");

            migrationBuilder.AddForeignKey(
                name: "FK_StockPositions_Clients_ClientId",
                table: "StockPositions",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "CUIT",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispatches_Orders_OrderNumber",
                table: "Dispatches");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderNumber",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Dispatches_DispatchNumber1",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Receipts_ReceiptNumber",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_Products_ProductId",
                table: "ReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_Receipts_ReceiptNumber",
                table: "ReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Clients_ClientId",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_StockPositions_Clients_ClientCUIT",
                table: "StockPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockPositions_Clients_ClientId",
                table: "StockPositions");

            migrationBuilder.DropIndex(
                name: "IX_StockPositions_ClientCUIT",
                table: "StockPositions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DispatchNumber1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ReceiptNumber",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Dispatches_OrderNumber",
                table: "Dispatches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptItems",
                table: "ReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptItems_ReceiptNumber",
                table: "ReceiptItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ClientCUIT",
                table: "StockPositions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StockPositions");

            migrationBuilder.DropColumn(
                name: "ReceiptNumber",
                table: "StockPositions");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "DispatchNumber1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReceiptNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "Dispatches");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Dispatches");

            migrationBuilder.DropColumn(
                name: "ReceiptNumber",
                table: "ReceiptItems");

            migrationBuilder.RenameTable(
                name: "ReceiptItems",
                newName: "ReceiptItem");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                newName: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "ReceiptNumber",
                table: "Receipts",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Dispatches",
                newName: "Date");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptItems_ProductId",
                table: "ReceiptItem",
                newName: "IX_ReceiptItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItem",
                newName: "IX_OrderItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_OrderNumber",
                table: "OrderItem",
                newName: "IX_OrderItem_OrderNumber");

            migrationBuilder.AlterColumn<int>(
                name: "Shelf",
                table: "StockPositions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Section",
                table: "StockPositions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Level",
                table: "StockPositions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "StockPositions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Aisle",
                table: "StockPositions",
                type: "nvarchar(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Receipts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "CarrierCUIT",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "Receipts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "Dispatches",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "Dispatches",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ReceiptId",
                table: "ReceiptItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptItem",
                table: "ReceiptItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_OrderId",
                table: "Receipts",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_OrderNumber",
                table: "Receipts",
                column: "OrderNumber",
                unique: true,
                filter: "[OrderNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Dispatches_OrderId",
                table: "Dispatches",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispatches_OrderNumber",
                table: "Dispatches",
                column: "OrderNumber",
                unique: true,
                filter: "[OrderNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItem_ReceiptId",
                table: "ReceiptItem",
                column: "ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispatches_Orders_OrderId",
                table: "Dispatches",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderNumber",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispatches_Orders_OrderNumber",
                table: "Dispatches",
                column: "OrderNumber",
                principalTable: "Orders",
                principalColumn: "OrderNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Orders_OrderNumber",
                table: "OrderItem",
                column: "OrderNumber",
                principalTable: "Orders",
                principalColumn: "OrderNumber",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Products_ProductId",
                table: "OrderItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItem_Products_ProductId",
                table: "ReceiptItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItem_Receipts_ReceiptId",
                table: "ReceiptItem",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Clients_ClientId",
                table: "Receipts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "CUIT",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Orders_OrderId",
                table: "Receipts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderNumber",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Orders_OrderNumber",
                table: "Receipts",
                column: "OrderNumber",
                principalTable: "Orders",
                principalColumn: "OrderNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_StockPositions_Clients_ClientId",
                table: "StockPositions",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "CUIT");
        }
    }
}
