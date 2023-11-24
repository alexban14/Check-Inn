using System.Web.Mvc;

namespace Check_Inn.Areas.Dashboard
{
    public class DashboardAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Dashboard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Dashboard_default",
                "Dashboard/{controller}/{action}/{id}",
                new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "AccomodationsAdmin",
                "Dashboard/Accomodations/{action}/{id}",
                new { controller = "Accomodations", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Check_Inn.Areas.Dashboard.Controllers"}
            );
        }
    }
}