using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.Entities
{
    public class Accomodation
    {
        public int ID { get; set; }
        public int AccomodationPackageID { get; set; }
        public virtual AccomodationPackage AccomodationPackage { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image {  get; set; }
    }
}