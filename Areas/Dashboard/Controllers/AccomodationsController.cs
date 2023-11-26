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
    public class AccomodationsController : Controller
    {
        AccomodationsService accomodationsService;
        AccomodationPackagesService accomodationPackagesService;

        public AccomodationsController()
        {
            accomodationsService = new AccomodationsService();
            accomodationPackagesService = new AccomodationPackagesService();
        }

        // GET: Dashboard/Accomodations
        public ActionResult Index(string searchTerm, int? AccomodationPackageID, int? page)
        {
            int recordSize = 3;
            page = page ?? 1;

            AccomodationsListingModel model = new AccomodationsListingModel();

            model.SearchTerm = searchTerm;
            model.AccomodationPackageID = AccomodationPackageID;

            model.Accomodations = accomodationsService.SearchAccomodation(searchTerm, AccomodationPackageID, page, recordSize);
            model.AccomodationPackages = accomodationPackagesService.GetAllAcomodationPackages();

            int totalRecords = accomodationsService.SearchAccomodationCount(searchTerm, AccomodationPackageID);

            model.Pager = new Check_Inn.ViewModels.Pager(totalRecords, page, recordSize);

            return View(model);
        }

        // GET: Dashboard/Accomodations/Create
        public ActionResult Action(int? ID)
        {
            AccomodationActionModel model = new AccomodationActionModel();

            model.AccomodationPackages = accomodationPackagesService.GetAllAcomodationPackages();

            if(ID > 0)
            {
                Accomodation accomodation = accomodationsService.GetAccomodationByID(ID.Value);

                model.ID = accomodation.ID;
                model.AccomodationPackageID = accomodation.AccomodationPackageID;
                model.Name = accomodation.Name;
                model.Image = accomodation.Image;
                model.Description = accomodation.Description;
            }

            return View("Action", model);
        }

        // POST: Dashboard/Accomodations/Create
        [HttpPost]
        public ActionResult Action(Accomodation model)
        {
            bool result;


            if(ModelState.IsValid)
            {
                if (model.ID > 0)
                {
                    Accomodation accomodation = accomodationsService.GetAccomodationByID(model.ID);
                    accomodation.AccomodationPackageID = model.AccomodationPackageID;
                    accomodation.Name = model.Name;
                    accomodation.Image = model.Image;
                    accomodation.Description = model.Description;

                    result = accomodationsService.UpdateAccomodation(accomodation);
                }
                else
                {
                    Accomodation accomodation = new Accomodation();

                    accomodation.AccomodationPackageID = model.AccomodationPackageID;
                    accomodation.Name = model.Name;
                    accomodation.Image = model.Image;
                    accomodation.Description = model.Description;

                    result = accomodationsService.SaveAccomodation(accomodation);
                }

                if (result)
                {
                    return RedirectToAction("Index", new { AccomodationPackageID = model.AccomodationPackageID });
                }
                else
                {
                    ModelState.AddModelError("", "Unable to perform action on Accomodation");
                }

            }

            return View("Action", model);
        }

        // GET: Dashboard/Accomodations/Delete/5
        public ActionResult Delete(int ID)
        {
            AccomodationActionModel model = new AccomodationActionModel();

            Accomodation accomodation = accomodationsService.GetAccomodationByID(ID);   

            model.ID = accomodation.ID;
            model.Name = accomodation.Name;

            return View("Delete", model);
        }

        // POST: Dashboard/Accomodations/Delete/5
        [HttpPost]
        public ActionResult Delete(Accomodation model)
        {
            JsonResult json = new JsonResult();
            bool result;

            Accomodation accomodation = accomodationsService.GetAccomodationByID(model.ID);

            result = accomodationsService.DeleteAccomodation(accomodation);

            if (result)
            {
                json.Data = new { Success = true };
            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Accomodation" };
            }

            return json;
        }
    }
}
