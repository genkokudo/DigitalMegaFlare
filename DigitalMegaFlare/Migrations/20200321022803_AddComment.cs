using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalMegaFlare.Migrations
{
    public partial class AddComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "ExcelInputHistories",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "ExcelInputHistories",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "ExcelInputHistories");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "ExcelInputHistories");
        }
    }
}
