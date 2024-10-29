using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.Areas.Dashboard.Middleware
{
    public class Dashboard
    {
        public static void RegisterIndexMiddleware(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (IsDashboardRequest(context) && !IsUserAuthorized(context))
                {
                    return;
                }

                await next.Invoke();
            });
        }

        private static bool IsDashboardRequest(IOwinContext context)
        {
            return context.Request.Path.Value.StartsWith("/Dashboard");
        }

        private static bool IsUserAuthorized(IOwinContext context)
        {
            var user = context.Authentication.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Response.Redirect("/Account/Login");
                return false;
            }
            else if (!user.IsInRole("Admin"))
            {
                // TODO: implement route & page not found
                context.Response.Redirect("/Home/PageNotFound");
                return false;
            }

            return true;
        }
    }
}