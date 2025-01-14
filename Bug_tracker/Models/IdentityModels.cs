﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using Bug_tracker.Models.CodeFirst;

namespace Bug_tracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get  { return FirstName + " " + LastName; } }
        public ApplicationUser()      
         {
            this.Projects = new HashSet<Project>();
         }
    public virtual ICollection<Project> Projects { get; set; }




    //default code below

    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
            public DbSet<Project> Projects { get; set; }
            public DbSet<Ticket> Tickets { get; set; }
            public DbSet<TicketPriority> TicketPriority { get; set; }
            public DbSet<TicketType> TicketType { get; set; }
            public DbSet<TicketStatus> TicketStatus { get; set; }
            public DbSet<TicketComment> TicketComments { get; set; }
            public DbSet<TicketAttachment> TicketAttachments { get; set; }
            public DbSet<TicketHistory> TicketHistory { get; set; }
            


    }
}