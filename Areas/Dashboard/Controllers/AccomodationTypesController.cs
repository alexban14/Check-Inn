using Check_Inn.Areas.Dashboard.ViewModels;
using Check_Inn.Entities;
using Check_Inn.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Check_Inn.Areas.Dashboard.Controllers
{
    public class AccomodationTypesController : Controller
    {
        AccomodationTypesService accomodationTypesService;

        public AccomodationTypesController()
        {
            accomodationTypesService = new AccomodationTypesService();
        }

        // GET: Dashboard/AccomodationTypes
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Listing()
        {
            AccomodationTypesListingModel model = new AccomodationTypesListingModel();

            model.AccomodationTypes = accomodationTypesService.GetAllAccomodationTypes();

            return PartialView("_Listing", model);
        }

        public ActionResult Action()
        {
            AccomodationTypeActionModel model = new AccomodationTypeActionModel();

            return PartialView("_Action", model);
        }
    }
}