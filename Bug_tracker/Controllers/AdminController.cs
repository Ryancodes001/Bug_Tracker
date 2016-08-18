using Bug_tracker.Helpers;
using Bug_tracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bug_tracker.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AdminDashboard
        //[Authorize]
        public ActionResult AdminDashboard()
        {
            return View(db.Users.ToList());
        }



        //GET Admin/EditUser
        //[Authorize(Roles = "Admin")]
        public ActionResult EditUser(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            UserRolesHelper helper = new UserRolesHelper(db);
            var currentRoles = helper.ListUserRoles(id);
            var absentRoles = helper.ListAbsentUserRoles(id);
            AdminModel.AbsentRoles = new MultiSelectList(absentRoles);
            AdminModel.Roles = new MultiSelectList(currentRoles);
            AdminModel.User = user;

            return View(AdminModel);
        }

        // POST: Add User Role
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddRole(string AddId, List<string> SelectedAbsentRoles)
        {
            if (ModelState.IsValid)
            {
                UserRolesHelper helper = new UserRolesHelper(db);
                var user = db.Users.Find(AddId);
                foreach (var role in SelectedAbsentRoles)
                {
                    helper.AddUserToRole(AddId, role);
                }

                db.Entry(user).State = EntityState.Modified;
                db.Users.Attach(user);
                db.SaveChanges();
                return RedirectToAction("AdminDashboard");
            }
            return View(AddId);
        }

        // POST: Remove User Role
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult RemoveRole(string RemoveId, List<string> SelectedCurrentRoles)
        {
            if (ModelState.IsValid)
            {
                UserRolesHelper helper = new UserRolesHelper(db);
                var user = db.Users.Find(RemoveId);
                foreach (var role in SelectedCurrentRoles)
                {
                    helper.RemoveUserFromRole(RemoveId, role);
                }
                db.Entry(user).State = EntityState.Modified;
                db.Users.Attach(user);
                db.SaveChanges();
                return RedirectToAction("AdminDashboard");
            }
            return View(RemoveId);
        }

    }
}