using Bug_tracker.Models.CodeFirst;
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
        public Project Project { get; set; }
        public MultiSelectList AssignedUserList { get; set; }
        public MultiSelectList UnassignedUserList { get; set; }
        public string[] SelectedAssignedUsers { get; set; }
        public string[] SelectedUnassignedUsers { get; set; }
    }
}