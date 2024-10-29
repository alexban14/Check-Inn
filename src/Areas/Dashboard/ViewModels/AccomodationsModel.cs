using Check_Inn.Entities;
using Check_Inn.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.Areas.Dashboard.ViewModels
{
    public class AccomodationsListingModel
    {
        public IEnumerable<Accomodation> Accomodations { get; set; }
        public IEnumerable<AccomodationPackage> AccomodationPackages { get; set; }
        public int? AccomodationPackageID { get; set; }
        public string SearchTerm { get; set; }
        public Pager Pager { get; set; }
    }

    public class AccomodationActionModel
    {
        public int ID { get; set; }
        public int AccomodationPackageID { get; set; }
        public AccomodationPackage AccomodationPackage { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image {  get; set; }
        public IEnumerable<AccomodationPackage> AccomodationPackages { get; set; }
    }
}