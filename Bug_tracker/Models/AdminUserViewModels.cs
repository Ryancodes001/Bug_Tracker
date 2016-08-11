using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bug_tracker.Models
{
    public class AdminUserViewModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Roles { get; set; }
        public MultiSelectList AbsentRoles { get; set; }
        public string[] SelectedCurrentRoles { get; set; }
        public string[] SelectedAbsentRoles { get; set; }
    
    }

    

}