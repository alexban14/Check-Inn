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
            return View();
        }

        // GET: Dashboard/Accomodations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Accomodations/Create
        [HttpPost]
        public ActionResult Create(Accomodation model)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Dashboard/Accomodations/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Dashboard/Accomodations/Edit/5
        [HttpPost]
        public ActionResult Edit(Accomodation model)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Dashboard/Accomodations/Delete/5
        public ActionResult Delete(int ID)
        {
            return View();
        }

        // POST: Dashboard/Accomodations/Delete/5
        [HttpPost]
        public ActionResult Delete(Accomodation model)
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
