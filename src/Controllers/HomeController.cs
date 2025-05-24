using Check_Inn.Services;
using Check_Inn.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Check_Inn.Controllers
{
    public class HomeController : Controller
    {
        private AccomodationTypesService _accomodationTypesService;
        public HomeController(AccomodationTypesService accomodationTypesService)
        {
            _accomodationTypesService = accomodationTypesService;
        }

        public ActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();

            model.AccomodationTypes = _accomodationTypesService.GetAllAccomodationTypes();

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Rooms()
        {
            ViewBag.Message = "All the rooms we host.";

            return View();
        }
    }
}