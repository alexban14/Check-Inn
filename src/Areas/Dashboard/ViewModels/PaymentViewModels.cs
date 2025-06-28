using Check_Inn.Entities;
using Check_Inn.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.Areas.Dashboard.ViewModels
{
    public class PaymentsListingModel
    {
        public IEnumerable<Payment> Payments { get; set; }
        public string SearchTerm { get; set; }
        public string PaymentStatus { get; set; }
        public Pager Pager { get; set; }
        public int TotalRecords { get; set; }
        

        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public Dictionary<string, int> PaymentStatusStats { get; set; }
    }

    public class PaymentDetailsModel
    {
        public Payment Payment { get; set; }
        public Booking Booking { get; set; }
    }
}
