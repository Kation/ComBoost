namespace Company.Mvc.Migrations
{
    using Company.Entity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Company.Mvc.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Company.Mvc.DataContext";
        }

        protected override void Seed(Company.Mvc.DataContext context)
        {
            //EmployeeGroup group = context.EmployeeGroup.Create();
            //group.Index = Guid.NewGuid();
            //group.CreateDate = DateTime.Now;
            //group.Name = "Admins";
            //group.Power = EmployeePower.Admin;
            //context.EmployeeGroup.Add(group);

            //Employee admin = context.Employee.Create();
            //admin.Index = Guid.NewGuid();
            //admin.CreateDate = DateTime.Now;
            //admin.Name = "admin";
            //admin.Group = group;
            //admin.SetPassword("admin");
            //context.Employee.Add(admin);
            //context.SaveChanges();
        }
    }
}
