using Bug_tracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Bug_tracker.Models.CodeFirst;
namespace Bug_tracker.Helpers
{
    public class ProjectsHelper
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
    
        public ProjectsHelper (ApplicationDbContext context)
        {
            this.userManager = new UserManager<ApplicationUser>(
                 new UserStore<ApplicationUser>(context));
            this.db = context;
        }
       
        //check to see if a User has a project 
        public bool HasProject(string userId, int? projectId)
        {
            var user = db.Users.Find(userId);
            var projects = user.Projects.Where(n => n.Id == projectId).ToList();
            if (!projects.Any())
            {
                return false;
            }
            return true;
        }
        //Get a list of projects assigned to a given user
        public List<Project> AssignedProjects(string userId)
        {
            var user = db.Users.Find(userId);
            var projects = user.Projects.ToList();
            return projects; 
        }

        //Assign a user to a project if not already assigned
        public void AssignUser(string userId, int? projectId)
        {
            if (!HasProject(userId, projectId))
            {
                var user = db.Users.Find(userId);
                var project = db.Projects.Find(projectId);
                project.ApplicationUsers.Add(user);
            }
        }
        //List of users for a given project
        public List<ApplicationUser> AssignedUser(int? projectId)
        {
            var project = db.Projects.Find(projectId);
            var users = project.ApplicationUsers.ToList();
            return users;
            
        }
        //remove user from a project
        public void RemoveUser(string userId, int? projectId)
        {
            if (HasProject(userId, projectId))
            {
                var user = db.Users.Find(userId);
                var project = db.Projects.Find(projectId);
                project.ApplicationUsers.Remove(user);
            }
        }
        //Get a list of users NOT assigned to a given project
        public List<ApplicationUser> UnassignedUsers(int? projectId)
        {
            var users = db.Users.ToList();
            var AbsentUserList = new List<ApplicationUser>();
            foreach (var user in users)
            {
                if (!HasProject(user.Id, projectId))
                {
                    AbsentUserList.Add(user);
                }
            }
            return AbsentUserList;
        }

    }

}