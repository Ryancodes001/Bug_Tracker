using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bug_tracker.Models;
using Bug_tracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using System.IO;
using Bug_tracker.Helpers;

namespace Bug_tracker
{
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        //No need for a ticket attachments index

        //// GET: TicketAttachments
        //public ActionResult Index()
        //{
        //    var ticketAttachments = db.TicketAttachments.Include(t => t.Ticket).Include(t => t.User);
        //    return View(ticketAttachments.ToList());
        //}

        //// GET: TicketAttachments/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
        //    if (ticketAttachment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticketAttachment);
        //}

        // GET: TicketAttachments/Create
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Create(int? id)
        {
            UserRolesHelper rolesHelper = new UserRolesHelper(db);
            var user = db.Users.Find(User.Identity.GetUserId());
            var userRoles = rolesHelper.ListUserRoles(user.Id);
            Ticket ticket = db.Tickets.Find(id);

            ViewBag.UserId = user.Id;
            ViewBag.TicketId = id;

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

           
            return View();
        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,UserId,Description,Created")] TicketAttachment ticketAttachment, HttpPostedFileBase fileUpload)
        {


            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(fileUpload.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp" && ext != ".pdf" && ext != ".doc" && ext != ".docx")
                    ModelState.AddModelError("fileUpload", "Invalid Format.");
            }


            {
                if (ModelState.IsValid)
                {
                    
                    if (fileUpload != null)
                    {
                        //relative server path
                        var filePath = "/Uploads/";
                        // path on physical drive on server
                        var absPath = Server.MapPath("~" + filePath);
                        // media url for relative path
                        ticketAttachment.FilePath = filePath + fileUpload.FileName;
                        //save image
                        fileUpload.SaveAs(Path.Combine(absPath, fileUpload.FileName));
                    }

                    ticketAttachment.UserId = User.Identity.GetUserId();
                    ticketAttachment.Created = DateTimeOffset.Now;
                    db.TicketAttachments.Add(ticketAttachment);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
                }
            }
            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
            //ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }
    }
}
    
 
        //// GET: TicketAttachments/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
        //    if (ticketAttachment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
        //    return View(ticketAttachment);
        //}

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [Authorize(Roles = "Admin")]
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,Description,Created,FilePath")] TicketAttachment ticketAttachment)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(ticketAttachment).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
//            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
//            return View(ticketAttachment);
//        }

//        //GET: TicketAttachments/Delete/5
//        [Authorize(Roles ="Admin")]
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
//            if (ticketAttachment == null)
//            {
//                return HttpNotFound();
//            }
//            return View(ticketAttachment);
//        }

//        // POST: TicketAttachments/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
//            db.TicketAttachments.Remove(ticketAttachment);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
