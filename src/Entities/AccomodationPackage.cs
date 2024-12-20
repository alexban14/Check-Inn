using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Check_Inn.Entities
{
    public class AccomodationPackage
    {
        public int ID { get; set; }
        public int AccomodationTypeID { get; set; }
        public virtual AccomodationType AccomodationType { get; set;}
        [Column(TypeName = "TEXT")]
        public string Name { get; set; }
        public int NoOfRoom { get; set; }
        public decimal FeePerNight { get; set; }
    }
}