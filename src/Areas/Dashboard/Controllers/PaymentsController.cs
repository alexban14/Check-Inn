using Check_Inn.Areas.Dashboard.ViewModels;
using Check_Inn.ViewModels;
using Check_Inn.Entities;
using Check_Inn.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Check_Inn.Areas.Dashboard.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly PaymentService _paymentService;
        private readonly BookingsService _bookingsService;

        public PaymentsController(
            PaymentService paymentService,
            BookingsService bookingsService
        ) {
            _paymentService = paymentService;
            _bookingsService = bookingsService;
        }

        public ActionResult Index(string searchTerm, string paymentStatus, int? page)
        {
            int recordSize = 10;
            page = page ?? 1;

            PaymentsListingModel model = new PaymentsListingModel
            {
                SearchTerm = searchTerm,
                PaymentStatus = paymentStatus
            };

            model.Payments = _paymentService.SearchPayments(searchTerm, paymentStatus, page, recordSize);
            model.TotalRecords = _paymentService.SearchPaymentsCount(searchTerm, paymentStatus);
            model.Pager = new Pager(model.TotalRecords, page, recordSize);

            // Calculate statistics
            model.TotalRevenue = _paymentService.GetTotalRevenue();
            model.TodayRevenue = _paymentService.GetRevenueForDate(DateTime.Today);
            model.MonthRevenue = _paymentService.GetRevenueForMonth(DateTime.Today.Month, DateTime.Today.Year);
            model.PaymentStatusStats = _paymentService.GetPaymentStatusStatistics();

            return View(model);
        }

        public ActionResult Details(int id)
        {
            Payment payment = _paymentService.GetPaymentByID(id);
            if (payment == null)
            {
                return HttpNotFound();
            }

            var booking = _bookingsService.GetBookingByID(payment.BookingID);
            
            PaymentDetailsModel model = new PaymentDetailsModel
            {
                Payment = payment,
                Booking = booking
            };

            return View(model);
        }
    }
}
