using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mystore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class editorderheader4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_OrderHeaders_OrdereHaderId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "OrderHaderId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "OrdereHaderId",
                table: "OrderDetails",
                newName: "OrderHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrdereHaderId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrderHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_OrderHeaders_OrderHeaderId",
                table: "OrderDetails",
                column: "OrderHeaderId",
                principalTable: "OrderHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_OrderHeaders_OrderHeaderId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "OrderHeaderId",
                table: "OrderDetails",
                newName: "OrdereHaderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrderHeaderId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrdereHaderId");

            migrationBuilder.AddColumn<int>(
                name: "OrderHaderId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_OrderHeaders_OrdereHaderId",
                table: "OrderDetails",
                column: "OrdereHaderId",
                principalTable: "OrderHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
