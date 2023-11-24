using Check_Inn.Services;
using Check_Inn.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Check_Inn.Controllers
{
    public class AccomodationsController : Controller
    {
        private AccomodationTypesService _accomodationTypesService;
        private AccomodationPackagesService _accomodationPackagesService;
        private AccomodationsService _accomodationsService;

        public AccomodationsController()
        {
            _accomodationTypesService = new AccomodationTypesService();
            _accomodationPackagesService = new AccomodationPackagesService();
            _accomodationsService = new AccomodationsService();
        }

        // GET: Accomodations
        public ActionResult Index(int accomodationTypeID, int? accomodationPackageID)
        {
            AccomodationViewModel model = new AccomodationViewModel();

            model.AccomodationType = _accomodationTypesService.GetAccomodationTypeByID(accomodationTypeID);
            model.AccomodationPackages = _accomodationPackagesService.GetAllAccomodationPackagesByAccomodationType(accomodationTypeID);

            model.SelectedAccomodationPackageID = accomodationPackageID.HasValue ? accomodationPackageID.Value : model.AccomodationPackages.First().ID;
            model.Accomodations = _accomodationsService.GetAllAccomodationsByAccomodationType(model.SelectedAccomodationPackageID);

            return View(model);
        }
    }
}