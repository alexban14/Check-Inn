using Check_Inn.Areas.Dashboard.ViewModels;
using Check_Inn.Entities;
using Check_Inn.Services;
using Check_Inn.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Check_Inn.Areas.Dashboard.Controllers
{
    public class AccomodationPackagesController : Controller
    {
        AccomodationPackagesService accomodationPackagesService;
        AccomodationTypesService accomodationTypesService;
        public AccomodationPackagesController()
        {
            accomodationPackagesService = new AccomodationPackagesService();
            accomodationTypesService = new AccomodationTypesService();
        }

        // GET: Dashboard/AccomodationPackages
        public ActionResult Index(string searchTerm, int? AccomodationTypeID, int? page)
        {
            int recordSize = 3;
            page = page ?? 1;

            AccomodationPackagesListingModel model = new AccomodationPackagesListingModel();

            model.SearchTerm = searchTerm;
            model.AccomodationTypeID = AccomodationTypeID;

            model.AccomodationPackages = accomodationPackagesService.SearchAccomodationPackage(searchTerm, AccomodationTypeID, page, recordSize);
            model.AccomodationTypes = accomodationTypesService.GetAllAccomodationTypes();

            var totalRecords = accomodationPackagesService.SearchAccomodationPackageCount(searchTerm, AccomodationTypeID);

            model.Pager = new Pager(totalRecords, page, recordSize);

            return View(model);
        }
        

        // GET: Dashboard/AccomodationPackages/Create
        public ActionResult Action(int? ID)
        {
            AccomodationPackageActionModel model = new AccomodationPackageActionModel();

            if(ID > 0)
            {
                AccomodationPackage accomodationPackage = accomodationPackagesService.GetAccomodationPackageByID(ID.Value);

                model.ID = accomodationPackage.ID;
                model.AccomodationTypeID = accomodationPackage.AccomodationTypeID;
                model.Name = accomodationPackage.Name;
                model.NoOfRoom = accomodationPackage.NoOfRoom;
                model.FeePerNight = accomodationPackage.FeePerNight;
            }

            model.AccomodationTypes = accomodationTypesService.GetAllAccomodationTypes();

            return View("Action", model);
        }

        // POST: Dashboard/AccomodationPackages/Create
        [HttpPost]
        public ActionResult Action(AccomodationPackage model)
        {
            bool result;

            if (ModelState.IsValid)
            {
                if (model.ID > 0)
                {
                    AccomodationPackage accomodationPackage = accomodationPackagesService.GetAccomodationPackageByID(model.ID);

                    accomodationPackage.AccomodationTypeID = model.AccomodationTypeID;
                    accomodationPackage.Name = model.Name;
                    accomodationPackage.NoOfRoom = model.NoOfRoom;
                    accomodationPackage.FeePerNight = model.FeePerNight;

                    result = accomodationPackagesService.UpdateAccomodationPackage(accomodationPackage);
                }
                else
                {
                    AccomodationPackage accomodationPackage = new AccomodationPackage();

                    accomodationPackage.AccomodationTypeID = model.AccomodationTypeID;
                    accomodationPackage.Name = model.Name;
                    accomodationPackage.NoOfRoom = model.NoOfRoom;
                    accomodationPackage.FeePerNight = model.FeePerNight;

                    result = accomodationPackagesService.SaveAccomodationPackage(accomodationPackage);
                }

                if (result)
                {
                    return RedirectToAction("Index", new { AccomodationTypeID = model.AccomodationTypeID });
                }
                else
                {
                    ModelState.AddModelError("", "Unable to perform action on Accomodation Package");
                }

            }

            return View("Action", model);

        }

        // GET: Dashboard/AccomodationPackages/Delete/5
        public ActionResult Delete(int ID)
        {
            AccomodationPackageActionModel model = new AccomodationPackageActionModel();

            AccomodationPackage accomodationPackage = accomodationPackagesService.GetAccomodationPackageByID(ID);   

            model.ID = accomodationPackage.ID;
            model.Name = accomodationPackage.Name;

            return View("Delete", model);
        }

        // POST: Dashboard/AccomodationPackages/Delete/5
        [HttpPost]
        public ActionResult Delete(AccomodationPackage model)
        {
            JsonResult json = new JsonResult();
            bool result;

            AccomodationPackage accomodationPackage = accomodationPackagesService.GetAccomodationPackageByID(model.ID);

            result = accomodationPackagesService.DeleteAccomodationPackage(accomodationPackage);

            if (result)
            {
                json.Data = new { Success = true };
            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Accomodation Package" };
            }

            return json;
        }
    }
}
