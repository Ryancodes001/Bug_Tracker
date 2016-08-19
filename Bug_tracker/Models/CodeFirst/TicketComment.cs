using Bug_tracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_tracker.Models.CodeFirst
{
     public class TicketComment
    { 
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        


        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}