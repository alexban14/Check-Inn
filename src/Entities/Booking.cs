using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Check_Inn.Entities
{
    public class Booking
    {
        public int ID { get; set; }
        public int AccomodationID { get; set; }
        public virtual Accomodation Accomodation { get; set; }
        public DateTime FromDate { get; set; }
        public int Duration { get; set; }
        public int NoOfAdults { get; set; }
        public int NoOfChildren { get; set; }
        [Column(TypeName = "TEXT")]
        public string GuestName { get; set; }
        [Column(TypeName = "TEXT")]
        public string Email { get; set; }
        [Column(TypeName = "TEXT")]
        public string AdditionalInfo { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }

        [NotMapped]
        public Payment LatestPayment => Payments?.OrderByDescending(p => p.PaymentDate).FirstOrDefault();

        [NotMapped]
        public bool HasPendingPayment => Payments?.Any(p => p.PaymentStatus == "Pending") ?? false;
    }
}