using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wodsoft.ComBoost.Forum.Migrations
{
    public partial class localhost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Member",
                nullable: false);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Salt",
                table: "Member",
                nullable: false);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Password",
                table: "Member",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Member",
                nullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Salt",
                table: "Member",
                nullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Password",
                table: "Member",
                nullable: true);
        }
    }
}
