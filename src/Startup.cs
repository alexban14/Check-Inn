using Check_Inn.Areas.Dashboard.Middleware;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using WebGrease.Css.Ast.Selectors;

[assembly: OwinStartupAttribute(typeof(Check_Inn.Startup))]
namespace Check_Inn
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            Dashboard.RegisterMiddleware(app);
        }
    }
}
