using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wodsoft.ComBoost.Forum.Migrations
{
    public partial class board : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BoardIndex",
                table: "Forum",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Forum",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Board",
                columns: table => new
                {
                    Index = table.Column<Guid>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    EditDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Board", x => x.Index);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Forum_BoardIndex",
                table: "Forum",
                column: "BoardIndex");

            migrationBuilder.AddForeignKey(
                name: "FK_Forum_Board_BoardIndex",
                table: "Forum",
                column: "BoardIndex",
                principalTable: "Board",
                principalColumn: "Index",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forum_Board_BoardIndex",
                table: "Forum");

            migrationBuilder.DropTable(
                name: "Board");

            migrationBuilder.DropIndex(
                name: "IX_Forum_BoardIndex",
                table: "Forum");

            migrationBuilder.DropColumn(
                name: "BoardIndex",
                table: "Forum");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Forum");
        }
    }
}
