using Check_Inn.Areas.Dashboard.ViewModels;
using Check_Inn.Entities;
using Check_Inn.Services;
using Check_Inn.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Check_Inn.Areas.Dashboard.Controllers
{
    public class BookingsController: Controller
    {
        private readonly BookingsService _bookingsService;
        private readonly AccomodationsService _accomodationsService;
        public BookingsController(
            BookingsService bookingsService,
            AccomodationsService accomodationsService
        )
        {
            _bookingsService = bookingsService;
            _accomodationsService = accomodationsService;
        }

        // GET: Dashboard/Bookings
        public ActionResult Index(string searchTerm, int? AccomodationID, int? page)
        {
            int recordSize = 3;
            page = page ?? 1;

            BookingsListingModel model = new BookingsListingModel();

            model.SearchTerm = searchTerm;
            model.AccomodationID = AccomodationID;

            model.Bookings = _bookingsService.SearchBooking(searchTerm, AccomodationID, page, recordSize);
            model.Accomodations = _accomodationsService.GetAllAcomodation();

            var totalRecords = _bookingsService.SearchBookingCount(searchTerm, AccomodationID);

            model.Pager = new Pager(totalRecords, page, recordSize);

            return View(model);
        }


        // GET: Dashboard/Bookings/Create
        [HttpGet]
        public ActionResult Action(int? ID)
        {
            ViewModels.BookingActionModel model = new ViewModels.BookingActionModel();
            model.Accomodations = _accomodationsService.GetAllAcomodation();
            model.DurationList = new List<int>();
            for (int i = 1; i <= 10; i ++)
            {
                model.DurationList.Add(i);
            }

            if(ID > 0)
            {
                Booking booking = _bookingsService.GetBookingByID(ID.Value);

                model.ID = booking.ID;
                model.AccomodationID = booking.AccomodationID;
                model.FromDate = booking.FromDate;
                model.Duration = booking.Duration;
                model.NoOfAdults = booking.NoOfAdults;
                model.NoOfChildren = booking.NoOfChildren;
                model.GuestName = booking.GuestName;
                model.Email = booking.Email;
                model.AdditionalInfo = booking.AdditionalInfo;
            }

            model.Accomodations = _accomodationsService.GetAllAcomodation();

            return View("Action", model);
        }

        // POST: Dashboard/Bookings/Create
        [HttpPost]
        public ActionResult Action(ViewModels.BookingActionModel model)
        {
            bool result;

            if (ModelState.IsValid)
            {
                if (model.ID > 0)
                {
                    Booking booking = _bookingsService.GetBookingByID(model.ID);

                    booking.AccomodationID = model.AccomodationID;
                    booking.FromDate = model.FromDate;
                    booking.Duration = model.Duration;
                    booking.NoOfAdults = model.NoOfAdults;
                    booking.NoOfChildren = model.NoOfChildren;
                    booking.GuestName = model.GuestName;
                    booking.Email = model.Email;
                    booking.AdditionalInfo = model.AdditionalInfo;

                    result = _bookingsService.UpdateBooking(booking);
                }
                else
                {
                    Booking booking = new Booking();

                    booking.AccomodationID = model.AccomodationID;
                    booking.FromDate = model.FromDate;
                    booking.Duration = model.Duration;
                    booking.NoOfAdults = model.NoOfAdults;
                    booking.NoOfChildren = model.NoOfChildren;
                    booking.GuestName = model.GuestName;
                    booking.Email = model.Email;
                    booking.AdditionalInfo = model.AdditionalInfo;

                    result = _bookingsService.SaveBooking(booking);
                }

                if (result)
                {
                    return RedirectToAction("Index", new { AccomodationTypeID = model.AccomodationID });

                }
                else
                {
                    ModelState.AddModelError("", "Unable to perform action on Accomodation Package");
                }

            }

            return View("Action", model);

        }

        // GET: Dashboard/Bookings/Delete/5
        public ActionResult Delete(int ID)
        {
            ViewModels.BookingActionModel model = new ViewModels.BookingActionModel();

            Booking booking = _bookingsService.GetBookingByID(ID);

            model.ID = booking.ID;
            model.FromDate = booking.FromDate;
            model.GuestName = booking.GuestName;
            model.Email = booking.Email;

            return View("Delete", model);
        }

        // POST: Dashboard/Bookings/Delete/5
        [HttpPost]
        public ActionResult Delete(Booking model)
        {
            JsonResult json = new JsonResult();
            bool result;

            Booking booking = _bookingsService.GetBookingByID(model.ID);

            result = _bookingsService.DeleteBooking(booking);

            if (result)
            {
                json.Data = new { Success = true };
            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Booking" };
            }

            return json;
        }
    }
}