namespace Bug_tracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Bug_tracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Bug_tracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }
            var userManager = new UserManager<ApplicationUser>(
        
                new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "rrousseau1988@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "rrousseau1988@gmail.com",
                    Email = "rrousseau1988@gmail.com",
                    FirstName = "Ryan",
                    LastName = "Rousseau",
                    DisplayName = "RRousseau"
                }, "E@gles1988");
            }
        }
    }
}