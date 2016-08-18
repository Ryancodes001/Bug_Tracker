using Bug_tracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_tracker.Models.CodeFirst
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public string FileUrl { get; set; }
        public string FilePath { get; set; }

        public virtual Ticket Tickets { get; set; }
    }
}