using Check_Inn.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.ViewModels
{
    public class BookingsListViewModel
    {
        public IEnumerable<Booking> Bookings { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int RecordSize { get; set; }
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((double)TotalCount / RecordSize);
            }
        }
    }

    public class BookingViewModel
    {
        public AccomodationPackage AccomodationPackage { get; set; }
        public Accomodation Accomodation { get; set; }
        public Booking Booking { get; set; }
        public List<int> DurationList { get; set; }
    }

    public class BookingActionModel
    {
        public string AccomodationPackageName {  get; set; }
        public decimal AccomodationFeePerNight { get; set; }
        public string AccomodationDescription { get; set; }
        public string AccomodationName { get; set; }
        public string AccomodationImage {  get; set; }

        public List<int> DurationList { get; set; }
        public int ID { get; set; }
        public int AccomodationID { get; set; }
        public DateTime FromDate { get; set; }
        public int Duration { get; set; }
        public int NoOfAdults { get; set; }
        public int NoOfChildren { get; set; }
        public string GuestName { get; set; }
        public string Email { get; set; }
        public string AdditionalInfo { get; set; }
        public int AccomodationPackageID { get; set; }
    }
}