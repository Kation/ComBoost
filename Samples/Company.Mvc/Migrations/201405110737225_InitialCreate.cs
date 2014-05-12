namespace Company.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Index = c.Guid(nullable: false),
                        Name = c.String(),
                        Sex = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Group_Index = c.Guid(),
                    })
                .PrimaryKey(t => t.Index)
                .ForeignKey("dbo.EmployeeGroups", t => t.Group_Index)
                .Index(t => t.Group_Index);
            
            CreateTable(
                "dbo.EmployeeGroups",
                c => new
                    {
                        Index = c.Guid(nullable: false),
                        Name = c.String(),
                        CreateDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Index);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "Group_Index", "dbo.EmployeeGroups");
            DropIndex("dbo.Employees", new[] { "Group_Index" });
            DropTable("dbo.EmployeeGroups");
            DropTable("dbo.Employees");
        }
    }
}
