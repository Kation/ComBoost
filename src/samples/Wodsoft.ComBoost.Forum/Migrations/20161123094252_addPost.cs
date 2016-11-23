using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wodsoft.ComBoost.Forum.Migrations
{
    public partial class addPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Index = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    EditDate = table.Column<DateTime>(nullable: false),
                    MemberIndex = table.Column<Guid>(nullable: true),
                    ThreadIndex = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Index);
                    table.ForeignKey(
                        name: "FK_Post_Member_MemberIndex",
                        column: x => x.MemberIndex,
                        principalTable: "Member",
                        principalColumn: "Index",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Post_Thread_ThreadIndex",
                        column: x => x.ThreadIndex,
                        principalTable: "Thread",
                        principalColumn: "Index",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_MemberIndex",
                table: "Post",
                column: "MemberIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ThreadIndex",
                table: "Post",
                column: "ThreadIndex");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Post");
        }
    }
}
