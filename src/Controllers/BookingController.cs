using Check_Inn.Services;
using Check_Inn.ViewModels;
using Check_Inn.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Check_Inn.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private BookingsService _bookingService;
        private PaymentService _paymentService;
        private AccomodationPackagesService _accomodationPackagesService;
        private AccomodationsService _accomodationsService;

        public BookingController(
            BookingsService bookingsService, 
            PaymentService paymentService,
            AccomodationPackagesService accomodationPackagesService,
            AccomodationsService accomodationsService
        ) {
            _bookingService = bookingsService;
            _paymentService = paymentService;
            _accomodationPackagesService = accomodationPackagesService;
            _accomodationsService = accomodationsService;
        }

        private CheckInnUserManager _userManager;
        public CheckInnUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<CheckInnUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index(int? page = 1, int? recordSize = 10)
        {
            string userId = User.Identity.GetUserId();
            User user = UserManager.FindById(userId);
            
            var bookings = _bookingService.GetBookingsByUserEmail(user.Email, page, recordSize);
            var totalBookings = _bookingService.GetBookingCountByUserEmail(user.Email);
            
            var model = new BookingsListViewModel
            {
                Bookings = bookings,
                TotalCount = totalBookings,
                CurrentPage = page.Value,
                RecordSize = recordSize.Value
            };

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var booking = _bookingService.GetBookingByID(id);

            string userId = User.Identity.GetUserId();
            User user = UserManager.FindById(userId);

            if (booking == null || booking.Email != user.Email)
            {
                return HttpNotFound();
            }

            var payments = _paymentService.GetPaymentsByBookingID(id);
            ViewBag.Payments = payments;

            var pendingPayment = payments.FirstOrDefault(p => p.PaymentStatus == "Pending");
            ViewBag.HasPendingPayment = pendingPayment != null;
            ViewBag.PendingPayment = pendingPayment;

            var accomodation = _accomodationsService.GetAccomodationByID(booking.AccomodationID);
            var package = _accomodationPackagesService.GetAccomodationPackageByID(accomodation.AccomodationPackageID);
            ViewBag.TotalAmount = package.FeePerNight * booking.Duration;

            return View("Details", booking);
        }

        public ActionResult Delete(int id)
        {
            var booking = _bookingService.GetBookingByID(id);

            string userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            string userEmail = user.Email;

            if (booking == null || booking.Email != userEmail)
            {
                return HttpNotFound();
            }

            if (booking.FromDate.Date < DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "Past bookings cannot be cancelled.";
                return RedirectToAction("Details", new { id = id });
            }

            return View(booking);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var booking = _bookingService.GetBookingByID(id);

            string userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            string userEmail = user.Email;

            if (booking == null || booking.Email != userEmail)
            {
                return HttpNotFound();
            }

            if (booking.FromDate.Date < DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "Past bookings cannot be cancelled.";
                return RedirectToAction("Details", new { id = id });
            }

            _bookingService.DeleteBooking(booking);

            TempData["SuccessMessage"] = "Your booking has been successfully cancelled.";
            return RedirectToAction("Index");
        }
    }
}