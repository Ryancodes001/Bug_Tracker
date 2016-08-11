using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bug_tracker.Models
{
    public class ProjectUserViewModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Projects { get; set; }
       
    }
}