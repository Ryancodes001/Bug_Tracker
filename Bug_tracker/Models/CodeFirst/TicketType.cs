using Bug_tracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_tracker.Models.CodeFirst
{
    public class TicketType
    {
        public TicketType()
        {
            this.Tickets = new HashSet<Ticket>();

        }
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Ticket>
            Tickets { get; set; }

    }
}