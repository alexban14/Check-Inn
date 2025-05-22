using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Check_Inn.Entities
{
    public class Payment
    {
        public int ID { get; set; }
        
        public int BookingID { get; set; }
        public virtual Booking Booking { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public DateTime PaymentDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } // "Pending", "Completed", "Failed", "Refunded"
        
        [StringLength(100)]
        public string StripePaymentIntentId { get; set; }
        
        [StringLength(100)]
        public string StripeChargeId { get; set; }
        
        [Column(TypeName = "TEXT")]
        public string Notes { get; set; }
        
        public Payment()
        {
            PaymentDate = DateTime.Now;
            PaymentStatus = "Pending";
        }
    }
}
