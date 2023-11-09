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
    public class AccomodationPackagesController : Controller
    {
        AccomodationPackagesService accomodationPackagesService;
        public AccomodationPackagesController()
        {
            accomodationPackagesService = new AccomodationPackagesService();
        }

        // GET: Dashboard/AccomodationPackages
        public ActionResult Index(string searchTerm)
        {
            AccomodationPackagesListingModel model = new AccomodationPackagesListingModel();

            model.SearchTerm = searchTerm;

            model.AccomodationPackages = accomodationPackagesService.SearchAccomodationPackage(searchTerm);

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
                model.AccomodationType = accomodationPackage.AccomodationType;
                model.NoOfRoom = accomodationPackage.NoOfRoom;
                model.FeePerNight = accomodationPackage.FeePerNight;
            }

            return View("Action", model);
        }

        // POST: Dashboard/AccomodationPackages/Create
        [HttpPost]
        public ActionResult Action(AccomodationPackage model)
        {
            JsonResult json = new JsonResult();
            bool result;

            if (model.ID > 0)
            {
                AccomodationPackage accomodationPackage = accomodationPackagesService.GetAccomodationPackageByID(model.ID);

                accomodationPackage.Name = model.Name;
                accomodationPackage.AccomodationTypeID = model.AccomodationTypeID;
                accomodationPackage.AccomodationType = model.AccomodationType;
                accomodationPackage.NoOfRoom = model.NoOfRoom;
                accomodationPackage.FeePerNight = model.FeePerNight;

                result = accomodationPackagesService.UpdateAccomodationPackage(accomodationPackage);
            }
            else
            {
                AccomodationPackage accomodationPackage = new AccomodationPackage();

                accomodationPackage.Name = model.Name;
                accomodationPackage.AccomodationTypeID = model.AccomodationTypeID;
                accomodationPackage.AccomodationType = model.AccomodationType;
                accomodationPackage.NoOfRoom = model.NoOfRoom;
                accomodationPackage.FeePerNight = model.FeePerNight;

                result = accomodationPackagesService.SaveAccomodationPackage(accomodationPackage);
            }

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

        // GET: Dashboard/AccomodationPackages/Delete/5
        public ActionResult Delete(int ID)
        {
            return View();
        }

        // POST: Dashboard/AccomodationPackages/Delete/5
        [HttpPost]
        public ActionResult Delete(AccomodationPackage accomodationPackage)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
