using Check_Inn.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<AccomodationType> AccomodationTypes { get; set; }
    }
}