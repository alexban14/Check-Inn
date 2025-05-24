using Check_Inn.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Check_Inn
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var container = ContainerConfig.RegisterDependencies();
            DependencyResolver.SetResolver(
                new Autofac.Integration.Mvc.AutofacDependencyResolver(container)
            );

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
