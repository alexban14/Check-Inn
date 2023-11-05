using Check_Inn.DAL;
using Check_Inn.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.Services
{
    public class AccomodationTypesService
    {
        public IEnumerable<AccomodationType> GetAllAccomodationTypes()
        {
            CheckInnContext context = new CheckInnContext();

            return context.AccomodationTypes.ToList();
        }

        public bool SaveAccomodationType(AccomodationType accomodationType)
        {
            CheckInnContext context = new CheckInnContext();

            context.AccomodationTypes.Add(accomodationType);

            return context.SaveChanges() > 0;
        }
    }
}