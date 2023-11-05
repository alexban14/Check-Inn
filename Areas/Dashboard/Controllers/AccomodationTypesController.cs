using Check_Inn.Areas.Dashboard.ViewModels;
using Check_Inn.Entities;
using Check_Inn.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

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

        [HttpGet]
        public ActionResult Action()
        {
            AccomodationTypeActionModel model = new AccomodationTypeActionModel();

            return View("Create", model);
        }

        [HttpPost]
        public JsonResult Action(AccomodationType model)
        {
            JsonResult json = new JsonResult();

            AccomodationType accomodationType = new AccomodationType();

            accomodationType.Name = model.Name;
            accomodationType.Description = model.Description;

            bool result = accomodationTypesService.SaveAccomodationType(accomodationType);

            if (result)
            {
                json.Data = new { Success = true };

            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to add Accomodation Type" };
            }

            return json;
        }
    }
}