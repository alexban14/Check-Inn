using Check_Inn.Entities;
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
        private BookingsService _bookingsService;

        public AccomodationsController()
        {
            _accomodationTypesService = new AccomodationTypesService();
            _accomodationPackagesService = new AccomodationPackagesService();
            _accomodationsService = new AccomodationsService();
            _bookingsService = new BookingsService();
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

        [HttpGet]
        public ActionResult BookAccomodation(int accomodationPackageID, int accomodationID)
        {
            BookingActionModel model = new BookingActionModel();
            AccomodationPackage accomodationPackage = _accomodationPackagesService.GetAccomodationPackageByID(accomodationPackageID);
            Accomodation accomodation = _accomodationsService.GetAccomodationByID(accomodationID);

            model.AccomodationPackageName = accomodationPackage.Name;
            model.AccomodationName = accomodation.Name;
            model.AccomodationPackageID = accomodationPackageID;
            model.AccomodationImage = accomodation.Image;
            model.AccomodationFeePerNight = accomodationPackage.FeePerNight;
            model.AccomodationDescription = accomodation.Description;
            model.AccomodationID = accomodationID;
            model.DurationList = new List<int>();
            for (int i = 1; i <= 10; i ++)
            {
                model.DurationList.Add(i);
            }

            return View("BookAccomodation", model);
        }

        [HttpPost]
        public ActionResult BookAccomodation(BookingActionModel model)
        {
            Booking booking = new Booking();
            bool isAccomodationAvailable = _bookingsService.IsAccomodationAvailable(model.AccomodationID, model.FromDate, model.Duration);

            if (ModelState.IsValid && isAccomodationAvailable)
            {
                booking.AccomodationID = model.AccomodationID;
                booking.FromDate = model.FromDate;
                booking.Duration = model.Duration;
                booking.NoOfAdults = model.NoOfAdults;
                booking.NoOfChildren = model.NoOfChildren;
                booking.GuestName = model.GuestName;
                booking.Email = model.Email;
                booking.AdditionalInfo = model.AdditionalInfo;

                bool saved =  _bookingsService.SaveBooking(booking);

                TempData["BookingSucceded"] = saved;

                return RedirectToAction("BookAccomodation", "Accomodations", new { accomodationPackageID = model.AccomodationPackageID, accomodationID = model.AccomodationID });
            }
            else
            {
                TempData["BookingFailed"] = true;
                return RedirectToAction("BookAccomodation", "Accomodations", new { accomodationPackageID = model.AccomodationPackageID, accomodationID = model.AccomodationID });
            }
        }

    }
}