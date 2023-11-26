using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Check_Inn.Areas.Dashboard.Controllers
{
    public class DashboardController : Controller
    {

        // GET: Dashboard/Dashboard
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View();

            }
            else
            {
                return Redirect("/");
            }
        }
    }
}