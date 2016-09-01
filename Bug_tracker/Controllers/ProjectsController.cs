using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bug_tracker.Models;
using Bug_tracker.Helpers;
using Microsoft.AspNet.Identity;

namespace Bug_tracker.Models.CodeFirst
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Index()
        {
            UserRolesHelper rolesHelper = new UserRolesHelper(db);
            var user = db.Users.Find(User.Identity.GetUserId());
            var userRoles = rolesHelper.ListUserRoles(user.Id);

            if (userRoles.Contains("Admin"))
            {
                return View(db.Projects.ToList());
            }
            if (userRoles.Contains("Project Manager") || (userRoles.Contains("Developer")) || (userRoles.Contains("Submitter")))
            {
                return View(user.Projects.ToList());
            }
            return RedirectToAction("Index");
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
<<<<<<< HEAD
        [Authorize(Roles = "Admin, Project Manager")]
=======
        [Authorize(Roles = "Admin")]
>>>>>>> refs/heads/newTemplate
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Created")] Project project)
        {
            if (ModelState.IsValid)
            {
                
                project.Created = DateTimeOffset.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Updated")] Project project)
        {
            if (ModelState.IsValid)
            {
                
                project.Updated = DateTimeOffset.Now;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

      
        //GET Change User
        [Authorize(Roles = "Admin, Project Manager")]

        public ActionResult ChangeUser(int? id)
        {
            var project = db.Projects.Find(id);
            ProjectUserViewModel ProjectModel = new ProjectUserViewModel();
            ProjectsHelper helper = new ProjectsHelper(db);
            var assigned = helper.AssignedUser(id);
            var unassigned = helper.UnassignedUsers(id);

          
            ProjectModel.AssignedUserList = new MultiSelectList(assigned, "id", "DisplayName");
            ProjectModel.UnassignedUserList = new MultiSelectList(unassigned, "id", "DisplayName");
            ProjectModel.Project = project;

            return View(ProjectModel);

        }


        // POST: Projects/ChangeUsers (ADD USER) ---c
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(int AddUserProjectID, List<string> SelectedUnassignedUsers)
        {
            if (ModelState.IsValid)
            {
                //Instantiate the helper and find the user in the DB
                ProjectsHelper helper = new ProjectsHelper(db);
                var project = db.Projects.Find(AddUserProjectID);

                //ADD NEW USER(S)


                if(SelectedUnassignedUsers != null)
                {
                    foreach (var u in SelectedUnassignedUsers)
                    {
                        //If the user isn't assigned to this project
                        if (!helper.HasProject(u, AddUserProjectID))
                        {
                            //Add the user to the project
                            helper.AssignUser(u, AddUserProjectID);
                        }
                    }

                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                
            }

            return RedirectToAction("Index");
        }


        //POST: Projects/ChangeUsers(REMOVE USER)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveUser(int RemoveUserProjectID, List<string> SelectedAssignedUsers)
        {
            if (ModelState.IsValid)
            {
                //Instantiate the helper and find the user in the DB
                ProjectsHelper helper = new ProjectsHelper(db);
                var project = db.Projects.Find(RemoveUserProjectID);

                //ADD NEW USER(S)


                if(SelectedAssignedUsers != null)
                {
                    foreach (var u in SelectedAssignedUsers)
                    {
                        //If the user isn't assigned to this project
                        if (helper.HasProject(u, RemoveUserProjectID))
                        {
                            //Add the user to the project
                            helper.RemoveUser(u, RemoveUserProjectID);
                        }
                    }

                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

             
            }

            return RedirectToAction("Index");
        }
            }
        }




//// GET: Projects/Delete/5
//public ActionResult Delete(int? id)
//{
//    if (id == null)
//    {
//        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//    }
//    Project project = db.Projects.Find(id);
//    if (project == null)
//    {
//        return HttpNotFound();
//    }
//    return View(project);
//}

//// POST: Projects/Delete/5
//[HttpPost, ActionName("Delete")]
//[ValidateAntiForgeryToken]
//public ActionResult DeleteConfirmed(int id)
//{
//    Project project = db.Projects.Find(id);
//    db.Projects.Remove(project);
//    db.SaveChanges();
//    return RedirectToAction("Index");
//}

//protected override void Dispose(bool disposing)
//{
//    if (disposing)
//    {
//        db.Dispose();
//    }
//    base.Dispose(disposing);
//}
