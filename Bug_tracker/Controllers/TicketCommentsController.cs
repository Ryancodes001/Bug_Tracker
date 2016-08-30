using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bug_tracker.Models;
using Microsoft.AspNet.Identity;
using Bug_tracker.Helpers;

namespace Bug_tracker.Models.CodeFirst
{
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

       //public Ticket ticket = new Ticket();


        //// GET: TicketComments
        //public ActionResult Index()
        //{
        //    var ticketComments = db.TicketComments.Include(t => t.Ticket);
        //    return View(ticketComments.ToList());
        //}

        //// GET: TicketComments/Details/5
        //public ActionResult Details(int? id)
        //{
            
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketComment ticketComment = db.TicketComments.Find(id);
        //    if (ticketComment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticketComment);
        //}

        // GET: TicketComments/Create
        [Authorize (Roles="Admin, Project Manager, Developer, Submitter")]
        public ActionResult Create(int? id)
        {
            ViewBag.TicketId = id;
            UserRolesHelper rolesHelper = new UserRolesHelper(db);
            var user = db.Users.Find(User.Identity.GetUserId());
            var userRoles = rolesHelper.ListUserRoles(user.Id);
            Ticket ticket = db.Tickets.Find(id);
            ViewBag.Author = user.DisplayName;

            if (userRoles.Contains("Admin"))
            {
                return View();
            }
            if (userRoles.Contains("Project Manager"))
            {
                if (ticket.Project.ApplicationUsers.Contains(user))
                {
                    return View();
                }
            }
            if (userRoles.Contains("Developer"))
            {
                if (ticket.AssignedToUserId == user.Id)
                {
                    return View();
                }
            }
            if (userRoles.Contains("Submitter"))
            {
                if (ticket.OwnerUserId == user.Id)
                {
                    return View();
                }
            }

            return RedirectToAction("Login", "Account");
        }

        //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
       

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,UserId,Body,Created")] TicketComment ticketComment)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.Author = user.DisplayName;


            if (ModelState.IsValid)
            {
                
                ticketComment.UserId = User.Identity.GetUserId();
                ticketComment.Created = DateTimeOffset.Now;
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
            }

            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
           return RedirectToAction("Ticket");
        }

        //// GET: TicketComments/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketComment ticketComment = db.TicketComments.Find(id);
        //    if (ticketComment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
        //    return View(ticketComment);
        //}

        //// POST: TicketComments/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,Body,Created")] TicketComment ticketComment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(ticketComment).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
        //    return View(ticketComment);
        //}

        //// GET: TicketComments/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketComment ticketComment = db.TicketComments.Find(id);
        //    if (ticketComment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticketComment);
        //}

        //// POST: TicketComments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    TicketComment ticketComment = db.TicketComments.Find(id);
        //    db.TicketComments.Remove(ticketComment);
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
    }
}
