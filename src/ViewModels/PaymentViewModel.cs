using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Check_Inn.Entities;

namespace Check_Inn.ViewModels
{
    public class PaymentViewModel
    {
        public int BookingID { get; set; }
        public string BookingReference { get; set; }
        public string AccomodationName { get; set; }
        public string AccomodationPackageName { get; set; }
        public string AccomodationImage { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Duration { get; set; }
        public decimal FeePerNight { get; set; }
        public decimal TotalAmount { get; set; }
        public string GuestName { get; set; }
        public string Email { get; set; }
        public int NoOfAdults { get; set; }
        public int NoOfChildren { get; set; }
        public string ClientSecret { get; set; }
        public string PublicKey { get; set; }
    }
}
