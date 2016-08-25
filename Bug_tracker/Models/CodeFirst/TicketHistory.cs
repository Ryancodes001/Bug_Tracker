using Bug_tracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bug_tracker.Models.CodeFirst
{
    public class TicketHistory
    {
       
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Body { get; set; }
        [AllowHtml]
        public DateTimeOffset? Updated { get; set; }
        

        public virtual Ticket Ticket { get; set; }
    }
}