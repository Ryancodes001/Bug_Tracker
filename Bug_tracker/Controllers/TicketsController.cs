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
using System.Threading.Tasks;
using Bug_tracker;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Text;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public TicketsController()
        {

        }
        public TicketsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

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
            TicketHistory ticketHistory = new TicketHistory();
           


            if (ModelState.IsValid)
            {
                //Ticket Info
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.TicketStatusId = 1;
                ticket.Created = DateTimeOffset.Now;
                db.Tickets.Add(ticket);
                ////Ticket History info
                ticketHistory.TicketId = ticket.Id;
                db.TicketHistory.Add(ticketHistory);
                //Saving above to database
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
            ProjectsHelper projectHelper = new ProjectsHelper(db);
            UserRolesHelper rolesHelper = new UserRolesHelper(db);
           

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
            ViewBag.AssignedToUserId = new SelectList(rolesHelper.UsersInRole("Developer"), "Id", "DisplayName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);
            //if User is assigned to project, then he can make a ticket for that project;
            ViewBag.ProjectId = new SelectList(projectHelper.AssignedProjects(user.Id), "Id", "Title", ticket.ProjectId);
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,AssignedToUserId,OwnerUserId")] Ticket ticket)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            UserRolesHelper rolesHelper = new UserRolesHelper(db);
            ProjectsHelper helper = new ProjectsHelper(db);
            StringBuilder sb = new StringBuilder();


            //var ticketHistory = db.TicketHistory.Where(t => t.TicketId == ticket.Id).ToList();
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

            if (ModelState.IsValid)
            {
                ticket.Updated = DateTimeOffset.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                var newTicket = db.Tickets.Find(ticket.Id);

                if (oldTicket != ticket)
                {
                    sb.AppendLine("Changes on " + DateTimeOffset.Now + ":");
                    sb.Append("<br />");

                    if (oldTicket.Title != ticket.Title)
                    {
                        sb.AppendLine("Title changed from " + oldTicket.Title + " to " + ticket.Title + ".");
                        sb.Append("<br />");
                    }
                    if (oldTicket.Description != ticket.Description)
                    {
                        sb.AppendLine("Description changed from " + oldTicket.Description + " to " + ticket.Description + ".");
                        sb.Append("<br />");
                    }
                    if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                    {
                        var newTicketStatus = db.TicketStatus.Where(s => s.Id == newTicket.TicketStatusId).Select(q => q.Name).FirstOrDefault();
                        sb.AppendLine("Status changed from " + oldTicket.TicketStatus.Name + " to " + newTicketStatus + ".");
                        sb.Append("<br />");
                    }
                    if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                    {
                        var newTicketPriority = db.TicketPriority.Where(s => s.Id == newTicket.TicketPriorityId).Select(q => q.Name).FirstOrDefault();
                        sb.AppendLine("Priority changed from " + oldTicket.TicketPriority.Name + " to " + newTicketPriority + ".");
                        sb.Append("<br />");
                    }
                    if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                    {
                        var newTicketType = db.TicketType.Where(s => s.Id == newTicket.TicketTypeId).Select(q => q.Name).FirstOrDefault();
                        sb.AppendLine("Type changed from " + oldTicket.TicketType.Name + " to " + newTicketType + ".");
                        sb.Append("<br />");
                    }
                    if (oldTicket.AssignedToUserId != ticket.AssignedToUserId)
                    {
                        var newTicketUser = db.Users.Where(s => s.Id == newTicket.AssignedToUserId).Select(q => q.UserName).FirstOrDefault();
                        sb.AppendLine("Assigned User changed from " + oldTicket.AssignedToUser.UserName + " to " + newTicketUser + ".");
                        sb.Append("<br />");
                    }
                }

                var tHistory = new TicketHistory();
                tHistory.TicketId = ticket.Id;
                tHistory.Body = sb.ToString();

                db.TicketHistory.Add(tHistory);
                db.SaveChanges();

                await UserManager.SendEmailAsync(ticket.AssignedToUserId, "Ticket Assigned/Modified", "You have been assigned a new ticket, or a ticket you are currently assigned to has been modified.");
                return RedirectToAction("Index");
            }

            ViewBag.AssignedToUserId = new SelectList(rolesHelper.UsersInRole("Developer"), "Id", "DisplayName", ticket.AssignedToUserId);
            //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(helper.AssignedProjects(user.Id), "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        //// POST: Tickets/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Project Manager, Developer")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,AssignedToUserId,OwnerUserId")] Ticket ticket)
        //{
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    ProjectsHelper projectHelper = new ProjectsHelper(db);
        //    UserRolesHelper rolesHelper = new UserRolesHelper(db);

        //    var ticketHistory = db.TicketHistory.Where(t => t.TicketId == ticket.Id).FirstOrDefault();
        //    var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
           
        //    var sb1 = new StringBuilder();
        //    var sb2 = new StringBuilder();
        //    var sb3 = new StringBuilder();
            
        //    //These are the lines that print when a change takes place
        //    sb1.AppendFormat("{0} - {1} - {2}", DateTimeOffset.Now.ToString(), "Status has changed.", oldTicket.TicketStatusId = oldTicket.TicketStatus.Name();
        //    sb2.AppendFormat("{0} - {1}", DateTimeOffset.Now.ToString(), "Status has changed.");
        //    sb3.AppendFormat("{0} - {1}", DateTimeOffset.Now.ToString(), "Status has changed.");

        //    if (ModelState.IsValid)
        //    {
        //        ticket.Updated = DateTimeOffset.Now;
        //        db.Entry(ticket).State = EntityState.Modified;
        //        db.SaveChanges();
               
                
        //        //this if logs the change with my string
        //        if (oldTicket.TicketStatus != ticket.TicketStatus)
        //        {
        //            ticketHistory.Body = sb1.ToString();
        //        }


        //        if (oldTicket.TicketPriority != ticket.TicketPriority)
        //        {
        //            ticketHistory.Body = sb2.ToString();
        //        }


        //        if (oldTicket.AssignedToUserId != ticket.AssignedToUserId)
        //        {
        //            ticketHistory.Body = sb3.ToString();
        //        }

        //        db.Entry(ticketHistory).State = EntityState.Modified;//This line will note the changes that takes place in the ticket

        //        db.SaveChanges();
        //        await UserManager.SendEmailAsync(ticket.AssignedToUserId, "Ticket Assigned/Modified", "Your ticket has been updated. Please see your ticket details page.");
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.AssignedToUserId = new SelectList(rolesHelper.UsersInRole("Developer"), "Id", "DisplayName", ticket.AssignedToUserId);
        //    //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);

        //    //if User is assigned to project, then he can make a ticket for that project;
        //    ViewBag.ProjectId = new SelectList(projectHelper.AssignedProjects(user.Id), "Id", "Title", ticket.ProjectId);
        //    ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
        //    ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
        //    ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
        //    return View(ticket);
        //}
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



//// POST: Tickets/Edit/5
//// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//[Authorize(Roles = "Project Manager, Developer")]
//[HttpPost]
//[ValidateAntiForgeryToken]
//public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,AssignedToUserId,OwnerUserId")] Ticket ticket)
//{
//    var user = db.Users.Find(User.Identity.GetUserId());
//    ProjectsHelper helper = new ProjectsHelper(db);
//    UserRolesHelper userRoles = new UserRolesHelper(db);

//    if (ModelState.IsValid)
//    {
//        ticket.Updated = DateTimeOffset.Now;
//        db.Entry(ticket).State = EntityState.Modified;
//        db.SaveChanges();
//        return RedirectToAction("Index");
//    }

//    ViewBag.AssignedToUserId = new SelectList(userRoles.UsersInRole("Developer"), "Id", "DisplayName", ticket.AssignedToUserId);
//    //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);
//    //if User is assigned to project, then he can make a ticket for that project;
//    ViewBag.ProjectId = new SelectList(helper.AssignedProjects(user.Id), "Id", "Title", ticket.ProjectId);
//    ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
//    ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
//    ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
//    return View(ticket);
//}
