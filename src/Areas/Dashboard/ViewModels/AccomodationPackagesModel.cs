﻿using Check_Inn.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Check_Inn.ViewModels;

namespace Check_Inn.Areas.Dashboard.ViewModels
{
    public class AccomodationPackagesListingModel
    {
        public IEnumerable<AccomodationPackage> AccomodationPackages { get; set; }
        public IEnumerable<AccomodationType> AccomodationTypes { get; set; }
        public int? AccomodationTypeID { get; set; }
        public string SearchTerm { get; set; }

        public Pager Pager { get; set; }
    }

    public class AccomodationPackageActionModel
    {
        public int ID { get; set; }

        public int AccomodationTypeID { get; set; }
        public AccomodationType AccomodationType { get; set; }


        public string Name { get; set; }
        public int NoOfRoom { get; set; }
        public decimal FeePerNight { get; set; }

        public IEnumerable<AccomodationType> AccomodationTypes { get; set; }
    }
}