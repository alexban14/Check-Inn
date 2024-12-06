using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Check_Inn.Entities
{
    public class AccomodationType
    {
        public int ID { get; set; }
        [Column(TypeName = "TEXT")]
        public string Name { get; set; }
        [Column(TypeName = "TEXT")]
        public string Description { get; set; }
        [Column(TypeName = "TEXT")]
        public string Image {  get; set; }
    }
}