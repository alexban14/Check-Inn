﻿using Check_Inn.Areas.Dashboard.ViewModels;
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
        public ActionResult Index(string searchTerm)
        {
            AccomodationTypesListingModel model = new AccomodationTypesListingModel();

            model.SearchTerm = searchTerm;

            model.AccomodationTypes = accomodationTypesService.SearchAccomodationType(searchTerm);

            return View(model);
        }
        
        [HttpGet]
        public ActionResult Action(int? ID)
        {
            AccomodationTypeActionModel model = new AccomodationTypeActionModel();

            if (ID.HasValue)
            {
                AccomodationType accomodationType = accomodationTypesService.GetAccomodationTypeByID(ID.Value);

                model.ID = accomodationType.ID;
                model.Name = accomodationType.Name;
                model.Description = accomodationType.Description;
            }

            return View("Action", model);
        }

        [HttpPost]
        public JsonResult Action(AccomodationType model)
        {
            JsonResult json = new JsonResult();
            bool result = false;

            if(model.ID > 0)
            {
                AccomodationType accomodationType = accomodationTypesService.GetAccomodationTypeByID(model.ID);

                accomodationType.Name = model.Name;
                accomodationType.Description = model.Description;

                result = accomodationTypesService.UpdateAccomodationType(accomodationType);
            }
            else
            {
                AccomodationType accomodationType = new AccomodationType();

                accomodationType.Name = model.Name;
                accomodationType.Description = model.Description;

                result = accomodationTypesService.SaveAccomodationType(accomodationType);
            }


            if (result)
            {
                json.Data = new { Success = true };

            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Accomodation Type" };
            }

            return json;
        }

        [HttpGet]
        public ActionResult Delete(int ID)
        {
            AccomodationTypeActionModel model = new AccomodationTypeActionModel();

            AccomodationType accomodationType = accomodationTypesService.GetAccomodationTypeByID(ID);

            model.ID = accomodationType.ID;
            model.Name = accomodationType.Name;

            return View("Delete", model);
        }

        [HttpPost]
        public JsonResult Delete(AccomodationType model)
        {
            JsonResult json = new JsonResult();
            bool result = false;

            AccomodationType accomodationType = accomodationTypesService.GetAccomodationTypeByID(model.ID);

            result = accomodationTypesService.DeleteAccomodationType(accomodationType);


            if (result)
            {
                json.Data = new { Success = true };

            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Accomodation Type" };
            }

            return json;
        }
    }
}