using Check_Inn.Entities;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Check_Inn.Services
{
    public class CheckInnSignInManager : SignInManager<User, string>
    { 
        public CheckInnSignInManager(CheckInnUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((CheckInnUserManager)UserManager);
        }

        public static CheckInnSignInManager Create(IdentityFactoryOptions<CheckInnUserManager> options, IOwinContext context)
        {
            return new CheckInnSignInManager(context.GetUserManager<CheckInnUserManager>(), context.Authentication);
        }
    }
}