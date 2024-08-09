using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mystore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class editorderheader3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderDetails",
                newName: "OrderHaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderHaderId",
                table: "OrderDetails",
                newName: "OrderId");
        }
    }
}
