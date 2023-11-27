using Check_Inn.Entities;
using Check_Inn.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.Areas.Dashboard.ViewModels
{
    public class BookingsListingModel
    {
        public IEnumerable<Booking> Bookings { get; set;}
        public IEnumerable<Accomodation> Accomodations { get; set;}
        public int? AccomodationID { get; set;}
        public string SearchTerm { get; set;}
        public Pager Pager { get; set;}
    }

    public class BookingActionModel
    {
        public IEnumerable<Accomodation> Accomodations { get; set;}
        public List<int> DurationList { get; set; }
        public int ID { get; set; }
        public int AccomodationID { get; set; }
        public Accomodation Accomodation { get; set; }
        public DateTime FromDate { get; set; }
        public int Duration { get; set; }
        public int NoOfAdults { get; set; }
        public int NoOfChildren { get; set; }
        public string GuestName { get; set; }
        public string Email { get; set; }
        public string AdditionalInfo { get; set; }

    }
}