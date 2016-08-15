using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_tracker.Models.CodeFirst
{
    public class Ticket
    {
        public Ticket()
        {
            this.Projects = new HashSet<Project>();

        }
        public int AuthorId { get; set; }
        public int AssignerId { get; set; }
        public int TypeId { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }


        public virtual ICollection<Project>
            Projects { get; set; }
        
       
    }
}