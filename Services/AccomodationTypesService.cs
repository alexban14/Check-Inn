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

        public IEnumerable<AccomodationType> SearchAccomodationType(string searchTerm)
        {
            CheckInnContext context = new CheckInnContext();

            IEnumerable<AccomodationType> accomodationTypes = context.AccomodationTypes.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                accomodationTypes = accomodationTypes.Where( a => a.Name.ToLower().Contains(searchTerm.ToLower()) );
            }

            return accomodationTypes.ToList();
        }

        public AccomodationType GetAccomodationTypeByID(int ID)
        {
            CheckInnContext context = new CheckInnContext();

            return context.AccomodationTypes.Find(ID);
        }

        public bool SaveAccomodationType(AccomodationType accomodationType)
        {
            CheckInnContext context = new CheckInnContext();

            context.AccomodationTypes.Add(accomodationType);

            return context.SaveChanges() > 0;
        }

        public bool UpdateAccomodationType(AccomodationType accomodationType)
        {
            CheckInnContext context = new CheckInnContext();

            context.Entry(accomodationType).State = System.Data.Entity.EntityState.Modified;

            return context.SaveChanges() > 0;
        }
        public bool DeleteAccomodationType(AccomodationType accomodationType)
        {
            CheckInnContext context = new CheckInnContext();

            context.Entry(accomodationType).State = System.Data.Entity.EntityState.Deleted;

            return context.SaveChanges() > 0;
        }
    }
}