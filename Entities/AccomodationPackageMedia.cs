﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.Entities
{
    public class AccomodationPackageMedia
    {
        public int ID { get; set; }
        public int AccomodationPackageID { get; set; }
        public int MediaID { get; set; }
        public virtual Media Media { get; set; }

    }
}