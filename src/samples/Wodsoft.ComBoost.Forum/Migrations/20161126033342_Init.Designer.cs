using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Wodsoft.ComBoost.Forum;

namespace Wodsoft.ComBoost.Forum.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20161126033342_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Wodsoft.ComBoost.Forum.Entity.Forum", b =>
                {
                    b.Property<Guid>("Index")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EditDate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Index");

                    b.ToTable("Forum");
                });

            modelBuilder.Entity("Wodsoft.ComBoost.Forum.Entity.Member", b =>
                {
                    b.Property<Guid>("Index")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate");

                    b.Property<DateTime>("EditDate");

                    b.Property<byte[]>("Password")
                        .IsRequired();

                    b.Property<byte[]>("Salt")
                        .IsRequired();

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Index");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("Wodsoft.ComBoost.Forum.Entity.Post", b =>
                {
                    b.Property<Guid>("Index")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<DateTime>("CreateDate");

                    b.Property<DateTime>("EditDate");

                    b.Property<Guid>("MemberIndex");

                    b.Property<Guid>("ThreadIndex");

                    b.HasKey("Index");

                    b.HasIndex("MemberIndex");

                    b.HasIndex("ThreadIndex");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("Wodsoft.ComBoost.Forum.Entity.Thread", b =>
                {
                    b.Property<Guid>("Index")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate");

                    b.Property<DateTime>("EditDate");

                    b.Property<Guid>("ForumIndex");

                    b.Property<Guid>("MemberIndex");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Index");

                    b.HasIndex("ForumIndex");

                    b.HasIndex("MemberIndex");

                    b.ToTable("Thread");
                });

            modelBuilder.Entity("Wodsoft.ComBoost.Forum.Entity.Post", b =>
                {
                    b.HasOne("Wodsoft.ComBoost.Forum.Entity.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberIndex")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Wodsoft.ComBoost.Forum.Entity.Thread", "Thread")
                        .WithMany("Replies")
                        .HasForeignKey("ThreadIndex")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Wodsoft.ComBoost.Forum.Entity.Thread", b =>
                {
                    b.HasOne("Wodsoft.ComBoost.Forum.Entity.Forum", "Forum")
                        .WithMany("Threads")
                        .HasForeignKey("ForumIndex")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Wodsoft.ComBoost.Forum.Entity.Member", "Member")
                        .WithMany("Threads")
                        .HasForeignKey("MemberIndex")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
