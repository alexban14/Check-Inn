using Check_Inn.DAL;
using Check_Inn.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.Services
{
    public class AccomodationPackagesService
    {
        public IEnumerable<AccomodationPackage> GetAllAcomodationPackages()
        {
            CheckInnContext context = new CheckInnContext();

            return context.AccomodationPackages.ToList();
        }

        public IEnumerable<AccomodationPackage> SearchAccomodationPackage(string searchTerm)
        {
            CheckInnContext context = new CheckInnContext();

            IEnumerable<AccomodationPackage> accomodationPackages = context.AccomodationPackages.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                accomodationPackages = accomodationPackages.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower()) );
            }

            return accomodationPackages.ToList();
        }

        public AccomodationPackage GetAccomodationPackageByID(int ID)
        {
            CheckInnContext context = new CheckInnContext();

            return context.AccomodationPackages.Find(ID);
        }

        public bool SaveAccomodationPackage(AccomodationPackage accomodationPackage)
        {
            CheckInnContext context = new CheckInnContext();

            context.AccomodationPackages.Add(accomodationPackage);

            return context.SaveChanges() > 0;
        }

        public bool UpdateAccomodationPackage(AccomodationPackage accomodationPackage)
        {
            CheckInnContext context = new CheckInnContext();

            context.Entry(accomodationPackage).State = System.Data.Entity.EntityState.Modified;

            return context.SaveChanges() > 0;
        }

        public bool Delete(AccomodationPackage accomodationPackage)
        {
            CheckInnContext context = new CheckInnContext();

            context.Entry(accomodationPackage).State = System.Data.Entity.EntityState.Deleted;

            return context.SaveChanges() > 0;
        }
    }
}