using Bug_tracker.Helpers;
using Bug_tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bug_tracker.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AdminDashboard
        public ActionResult AdminDashboard()
        {
            return View(db.Users.ToList());
        }
    


    //GET Admin/EditUser

    public ActionResult EditUser(string id)
    {
        var user = db.Users.Find(id);
        AdminUserViewModel AdminModel = new AdminUserViewModel();
        UserRolesHelper helper = new UserRolesHelper(db);
        var selected = helper.ListUserRoles(id);
        AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
        AdminModel.User = user;
        return View(AdminModel);
     }
   }
}


