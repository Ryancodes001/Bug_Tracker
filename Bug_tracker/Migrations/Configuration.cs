namespace Bug_tracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using Models.CodeFirst;
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
                    //DisplayName = "RRousseau"
                }, "E@gles1988");
            }

                var userId = userManager.FindByEmail("rrousseau1988@gmail.com").Id;
                userManager.AddToRole(userId, "Admin");

            context.TicketType.AddOrUpdate(x => x.Id,
            new Models.CodeFirst.TicketType() { Id = 1, Name = "Defect (Bug)" },
            new Models.CodeFirst.TicketType() { Id = 2, Name = "Enhancement" },
            new Models.CodeFirst.TicketType() { Id = 3, Name = "Feature Request" },
            new Models.CodeFirst.TicketType() { Id = 4, Name = "Task" });

            context.TicketPriority.AddOrUpdate(x => x.Id,
            new TicketPriority() { Id = 1, Name = "High" },
            new TicketPriority() { Id = 2, Name = "Medium" },
            new TicketPriority() { Id = 3, Name = "Low" });

            context.TicketStatus.AddOrUpdate(x => x.Id,
            new TicketStatus() { Id = 1, Name = "Unassigned" },
            new TicketStatus() { Id = 2, Name = "Assigned" },
            new TicketStatus() { Id = 3, Name = "Resolved" });
        } 

    }
}
