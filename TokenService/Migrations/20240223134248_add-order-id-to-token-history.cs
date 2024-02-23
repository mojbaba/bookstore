using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TokenService.Migrations
{
    /// <inheritdoc />
    public partial class addorderidtotokenhistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "History",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_History_OrderId",
                table: "History",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_History_OrderId",
                table: "History");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "History");
        }
    }
}
