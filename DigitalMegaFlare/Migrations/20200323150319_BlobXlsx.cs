using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalMegaFlare.Migrations
{
    public partial class BlobXlsx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "ExcelInputHistories");

            migrationBuilder.AddColumn<byte[]>(
                name: "Xlsx",
                table: "ExcelInputHistories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Xlsx",
                table: "ExcelInputHistories");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "ExcelInputHistories",
                type: "longtext",
                nullable: true);
        }
    }
}
