using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Wodsoft.ComBoost.Forum;

namespace Wodsoft.ComBoost.Forum.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Wodsoft.ComBoost.Forum.Entity.Forum", b =>
                {
                    b.Property<Guid>("Index")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EditDate");

                    b.Property<string>("Name");

                    b.HasKey("Index");

                    b.ToTable("Forum");
                });

            modelBuilder.Entity("Wodsoft.ComBoost.Forum.Entity.Member", b =>
                {
                    b.Property<Guid>("Index");

                    b.Property<DateTime>("CreateDate");

                    b.Property<DateTime>("EditDate");

                    b.Property<byte[]>("Password");

                    b.Property<byte[]>("Salt");

                    b.Property<string>("Username");

                    b.HasKey("Index");

                    b.HasIndex("Index")
                        .IsUnique();

                    b.ToTable("Member");
                });

            modelBuilder.Entity("Wodsoft.ComBoost.Forum.Entity.Thread", b =>
                {
                    b.Property<Guid>("Index")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate");

                    b.Property<DateTime>("EditDate");

                    b.Property<Guid?>("ForumIndex");

                    b.Property<string>("Title");

                    b.HasKey("Index");

                    b.HasIndex("ForumIndex");

                    b.ToTable("Thread");
                });

            modelBuilder.Entity("Wodsoft.ComBoost.Forum.Entity.Member", b =>
                {
                    b.HasOne("Wodsoft.ComBoost.Forum.Entity.Thread")
                        .WithOne("Member")
                        .HasForeignKey("Wodsoft.ComBoost.Forum.Entity.Member", "Index")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Wodsoft.ComBoost.Forum.Entity.Thread", b =>
                {
                    b.HasOne("Wodsoft.ComBoost.Forum.Entity.Forum", "Forum")
                        .WithMany("Threads")
                        .HasForeignKey("ForumIndex");
                });
        }
    }
}
