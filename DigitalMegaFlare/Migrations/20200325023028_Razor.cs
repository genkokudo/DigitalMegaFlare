using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalMegaFlare.Migrations
{
    public partial class Razor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExcelInputHistories");

            migrationBuilder.CreateTable(
                name: "ExcelFiles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IsLocked = table.Column<bool>(nullable: false),
                    RawFileName = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    InputDate = table.Column<DateTime>(nullable: false),
                    Host = table.Column<string>(nullable: true),
                    Ip = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Xlsx = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcelFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RazorFiles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Razor = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazorFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RazorFiles_RazorFiles_ParentId",
                        column: x => x.ParentId,
                        principalTable: "RazorFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RazorFiles_ParentId",
                table: "RazorFiles",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExcelFiles");

            migrationBuilder.DropTable(
                name: "RazorFiles");

            migrationBuilder.CreateTable(
                name: "ExcelInputHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(type: "longtext", nullable: true),
                    Host = table.Column<string>(type: "longtext", nullable: true),
                    InputDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Ip = table.Column<string>(type: "longtext", nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    RawFileName = table.Column<string>(type: "longtext", nullable: true),
                    Url = table.Column<string>(type: "longtext", nullable: true),
                    Xlsx = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcelInputHistories", x => x.Id);
                });
        }
    }
}
