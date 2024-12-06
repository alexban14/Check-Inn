using Check_Inn.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.Services
{
    public class CheckInnRoleManager: RoleManager<IdentityRole>
    {
        public CheckInnRoleManager(IRoleStore<IdentityRole, string> roleStore) : base(roleStore) { }

        public static CheckInnRoleManager Create(IdentityFactoryOptions<CheckInnRoleManager> options, IOwinContext context)
        {
            return new CheckInnRoleManager(new RoleStore<IdentityRole>(context.Get<CheckInnMySqlContext>()));
        }
    }
}