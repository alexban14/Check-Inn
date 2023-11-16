using Check_Inn.Entities;
using Check_Inn.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.Areas.Dashboard.ViewModels
{
    public class UserListingModel
    {
        public IEnumerable<User> Users { get; set; }
        public string RoleID { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }
        public string SearchTerm { get; set; }
        public Pager Pager { get; set; }
    }

    public class UserActionModel
    {
        public string ID { get; set; } 
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }
}