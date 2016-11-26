using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wodsoft.ComBoost.Forum.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forum",
                columns: table => new
                {
                    Index = table.Column<Guid>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EditDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forum", x => x.Index);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    Index = table.Column<Guid>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    EditDate = table.Column<DateTime>(nullable: false),
                    Password = table.Column<byte[]>(nullable: false),
                    Salt = table.Column<byte[]>(nullable: false),
                    Username = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.Index);
                });

            migrationBuilder.CreateTable(
                name: "Thread",
                columns: table => new
                {
                    Index = table.Column<Guid>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    EditDate = table.Column<DateTime>(nullable: false),
                    ForumIndex = table.Column<Guid>(nullable: false),
                    MemberIndex = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thread", x => x.Index);
                    table.ForeignKey(
                        name: "FK_Thread_Forum_ForumIndex",
                        column: x => x.ForumIndex,
                        principalTable: "Forum",
                        principalColumn: "Index",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Thread_Member_MemberIndex",
                        column: x => x.MemberIndex,
                        principalTable: "Member",
                        principalColumn: "Index",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Index = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    EditDate = table.Column<DateTime>(nullable: false),
                    MemberIndex = table.Column<Guid>(nullable: false),
                    ThreadIndex = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Index);
                    table.ForeignKey(
                        name: "FK_Post_Member_MemberIndex",
                        column: x => x.MemberIndex,
                        principalTable: "Member",
                        principalColumn: "Index",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Post_Thread_ThreadIndex",
                        column: x => x.ThreadIndex,
                        principalTable: "Thread",
                        principalColumn: "Index",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_MemberIndex",
                table: "Post",
                column: "MemberIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ThreadIndex",
                table: "Post",
                column: "ThreadIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Thread_ForumIndex",
                table: "Thread",
                column: "ForumIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Thread_MemberIndex",
                table: "Thread",
                column: "MemberIndex");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Thread");

            migrationBuilder.DropTable(
                name: "Forum");

            migrationBuilder.DropTable(
                name: "Member");
        }
    }
}
