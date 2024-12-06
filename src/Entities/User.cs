using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Check_Inn.Entities
{
    public class User : IdentityUser
    {
        [Column(TypeName = "TEXT")]
        public string FullName { get; set; }
        [Column(TypeName = "TEXT")]
        public string Country { get; set; }
        [Column(TypeName = "TEXT")]
        public string City { get; set; }

        [Column(TypeName = "TEXT")]
        public string Address { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

}