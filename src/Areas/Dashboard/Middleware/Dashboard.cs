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
        public static void RegisterMiddleware(IAppBuilder app)
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
            if (!IsAuthenticated(context))
            {
                RedirectToLogin(context);
                return false;
            }

            var requestPath = context.Request.Path.Value;

            if (IsAdminOnlyRoute(requestPath) && !IsAdmin(context))
            {
                RedirectToPageNotFound(context);
                return false;
            }

            if (IsManagerOrAdminRoute(requestPath) && !IsManagerOrAdmin(context))
            {
                RedirectToPageNotFound(context);
                return false;
            }

            return true;
        }

        private static bool IsAuthenticated(IOwinContext context)
        {
            return context.Authentication.User.Identity.IsAuthenticated;
        }

        private static void RedirectToLogin(IOwinContext context)
        {
            context.Response.Redirect("/Account/Login");
        }

        private static bool IsAdminOnlyRoute(string requestPath)
        {
            return requestPath.StartsWith("/Dashboard/Roles") || requestPath.StartsWith("/Dashboard/Users");
        }

        private static bool IsAdmin(IOwinContext context)
        {
            return context.Authentication.User.IsInRole("Admin");
        }

        private static bool IsManagerOrAdminRoute(string requestPath)
        {
            return requestPath.StartsWith("/Dashboard/AccomodationTypes") ||
                   requestPath.StartsWith("/Dashboard/AccomodationPackages") ||
                   requestPath.StartsWith("/Dashboard/Accomodations") ||
                   requestPath.StartsWith("/Dashboard/Bookings");
        }

        public static bool IsManagerOrAdmin(IOwinContext context)
        {
            var user = context.Authentication.User;
            return user.IsInRole("Admin") || user.IsInRole("HotelManager") || user.IsInRole("AccomodationManager");
        }

        private static void RedirectToPageNotFound(IOwinContext context)
        {
            if (IsManagerOrAdmin(context))
            {
                context.Response.Redirect("/Dashboard/AdminPageNotFound");
            } else
            {
                context.Response.Redirect("/PageNotFound");
            }
        }
    }
}