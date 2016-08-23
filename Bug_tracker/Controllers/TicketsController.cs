using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Bug_tracker.Models;
using Bug_tracker.Models.CodeFirst;
using Bug_tracker.Helpers;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Index()
        {
            // Find the user and a list of roles
            UserRolesHelper rolesHelper = new UserRolesHelper(db);
            var user = db.Users.Find(User.Identity.GetUserId());
            var userRoles = rolesHelper.ListUserRoles(user.Id);
            var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);

            if (userRoles.Contains("Admin"))
            {
                return View(tickets.ToList());
            }
            if (userRoles.Contains("Project Manager"))
            {
                return View(user.Projects.SelectMany(t => t.Tickets).ToList());
            }
            if (userRoles.Contains("Developer") && userRoles.Contains("Submitter"))
            {
                return View(tickets.Where(t => (t.AssignedToUserId == user.Id) || (t.OwnerUserId == user.Id)).ToList());
            }
            if (userRoles.Contains("Developer"))
            {
                return View(tickets.Where(t => t.AssignedToUserId == user.Id).ToList());
            }
            if (userRoles.Contains("Submitter"))
            {
                return View(tickets.Where(t => t.OwnerUserId == user.Id).ToList());
            }

            return RedirectToAction("Login", "Account");
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Details(int? id)
          {
            //Find the user, roles, and ticket Id
            UserRolesHelper rolesHelper = new UserRolesHelper(db);
            var user = db.Users.Find(User.Identity.GetUserId());
            var userRoles = rolesHelper.ListUserRoles(user.Id);
            Ticket ticket = db.Tickets.Find(id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            //If user is submitter or developer related to ticket, return that ticket
            if (user.Id == ticket.AssignedToUserId || user.Id == ticket.OwnerUserId)
            {
                return View(ticket);
            }
            //if user is Admin, return the view
            if (userRoles.Contains("Admin"))
            {
                return View(ticket);
            }
            //if user is project manager for this ticket, return the view 
            if (ticket.Project.ApplicationUsers.Contains(user))
            {
                return View(ticket);
            }
                

            return RedirectToAction("Login", "Account");

        }
        

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectsHelper helper = new ProjectsHelper(db);

            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName");

            //if User is assigned to project, then he can make a ticket for that project;
            ViewBag.ProjectId = new SelectList(helper.AssignedProjects(user.Id), "Id", "Title");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId")] Ticket ticket)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectsHelper helper = new ProjectsHelper(db);

            if (ModelState.IsValid)
            {
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.TicketStatusId = 1;
                ticket.Created = DateTimeOffset.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);
            //if User is assigned to project, then he can make a ticket for that project;
            ViewBag.ProjectId = new SelectList(helper.AssignedProjects(user.Id), "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles ="Project Manager, Developer")]
        public ActionResult Edit(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectsHelper helper = new ProjectsHelper(db);
            UserRolesHelper userRoles = new UserRolesHelper(db);
            //var devUsers = userRoles.UsersInRole("Developer");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            //This line specifies the users that are in the role of Developer- only developers can be assigned to a ticket
            ViewBag.AssignedToUserId = new SelectList(userRoles.UsersInRole("Developer"), "Id", "DisplayName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);
            //if User is assigned to project, then he can make a ticket for that project;
            ViewBag.ProjectId = new SelectList(helper.AssignedProjects(user.Id), "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Project Manager, Developer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,AssignedToUserId,OwnerUserId")] Ticket ticket)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectsHelper helper = new ProjectsHelper(db);
            UserRolesHelper userRoles = new UserRolesHelper(db);

            if (ModelState.IsValid)
            {
                ticket.Updated = DateTimeOffset.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedToUserId = new SelectList(userRoles.UsersInRole("Developer"), "Id", "DisplayName", ticket.AssignedToUserId);
            //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);
            //if User is assigned to project, then he can make a ticket for that project;
            ViewBag.ProjectId = new SelectList(helper.AssignedProjects(user.Id), "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
