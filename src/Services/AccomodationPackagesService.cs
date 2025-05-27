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
        private CheckInnMySqlContext context;

        public AccomodationPackagesService(CheckInnMySqlContext context)
        {
            this.context = context;
        }
        
        public IEnumerable<AccomodationPackage> GetAllAcomodationPackages()
        {
            return context.AccomodationPackages.ToList();
        }

        public IEnumerable<AccomodationPackage> GetAllAccomodationPackagesByAccomodationType(int accomodationTypeID)
        {
            return context.AccomodationPackages.Where(x => x.AccomodationTypeID == accomodationTypeID).ToList();
        }

        public IEnumerable<AccomodationPackage> SearchAccomodationPackage(string searchTerm, int? AccomodationTypeID, int? page, int? recordSize)
        {
            IEnumerable<AccomodationPackage> accomodationPackages = context.AccomodationPackages.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                accomodationPackages = accomodationPackages.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower()) );
            }

            if (AccomodationTypeID.HasValue && AccomodationTypeID > 0)
            {
                accomodationPackages = accomodationPackages.Where(a => a.AccomodationTypeID == AccomodationTypeID.Value);
            }

            var skip = (page - 1) * recordSize;

            return accomodationPackages
                .OrderBy(x => x.AccomodationTypeID)
                .Skip((int)skip)
                .Take((int)recordSize)
                .ToList();
        }

        public int SearchAccomodationPackageCount(string searchTerm, int? AccomodationTypeID)
        {
            IEnumerable<AccomodationPackage> accomodationPackages = context.AccomodationPackages.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                accomodationPackages = accomodationPackages.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower()) );
            }

            if (AccomodationTypeID.HasValue && AccomodationTypeID > 0)
            {
                accomodationPackages = accomodationPackages.Where(a => a.AccomodationTypeID == AccomodationTypeID.Value);
            }

            return accomodationPackages.Count();
        }

        public AccomodationPackage GetAccomodationPackageByID(int ID)
        {
            using (CheckInnMySqlContext context = new CheckInnMySqlContext())
            { 
                return context.AccomodationPackages.Find(ID);
            }
        }

        public bool SaveAccomodationPackage(AccomodationPackage accomodationPackage)
        {
            context.AccomodationPackages.Add(accomodationPackage);

            return context.SaveChanges() > 0;
        }

        public bool UpdateAccomodationPackage(AccomodationPackage accomodationPackage)
        {
            context.Entry(accomodationPackage).State = System.Data.Entity.EntityState.Modified;

            return context.SaveChanges() > 0;
        }

        public bool DeleteAccomodationPackage(AccomodationPackage accomodationPackage)
        {
            context.Entry(accomodationPackage).State = System.Data.Entity.EntityState.Deleted;

            return context.SaveChanges() > 0;
        }
    }
}