using Check_Inn.DAL;
using Check_Inn.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Inn.Services
{
    public class AccomodationsService
    {
        public IEnumerable<Accomodation> GetAllAcomodation()
        {
            CheckInnContext context = new CheckInnContext();

            return context.Accomodations.ToList();
        }

        public IEnumerable<Accomodation> SearchAccomodation(string searchTerm, int? AccomodationPackageID, int? page, int? recordSize)
        {
            CheckInnContext context = new CheckInnContext();

            IEnumerable<Accomodation> accomodations = context.Accomodations.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                accomodations = accomodations.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower()) );
            }

            if (AccomodationPackageID.HasValue && AccomodationPackageID > 0)
            {
                accomodations = accomodations.Where(a => a.AccomodationPackageID == AccomodationPackageID.Value);
            }

            var skip = (page - 1) * recordSize;

            return accomodations
                .OrderBy(x => x.AccomodationPackageID)
                .Skip((int)skip)
                .Take((int)recordSize)
                .ToList();
        }

        public int SearchAccomodationCount(string searchTerm, int? AccomodationPackageID)
        {
            CheckInnContext context = new CheckInnContext();

            IEnumerable<Accomodation> accomodations = context.Accomodations.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                accomodations = accomodations.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower()) );
            }

            if (AccomodationPackageID.HasValue && AccomodationPackageID > 0)
            {
                accomodations = accomodations.Where(a => a.AccomodationPackageID == AccomodationPackageID.Value);
            }

            return accomodations.Count();
        }

        public Accomodation GetAccomodationByID(int ID)
        {
            using (CheckInnContext context = new CheckInnContext())
            { 
                return context.Accomodations.Find(ID);
            }
        }

        public bool SaveAccomodation(Accomodation accomodation)
        {
            CheckInnContext context = new CheckInnContext();

            context.Accomodations.Add(accomodation);

            return context.SaveChanges() > 0;
        }

        public bool UpdateAccomodation(Accomodation accomodation)
        {
            CheckInnContext context = new CheckInnContext();

            context.Entry(accomodation).State = System.Data.Entity.EntityState.Modified;

            return context.SaveChanges() > 0;
        }

        public bool DeleteAccomodation(Accomodation accomodation)
        {
            CheckInnContext context = new CheckInnContext();

            context.Entry(accomodation).State = System.Data.Entity.EntityState.Deleted;

            return context.SaveChanges() > 0;
        }

    }
}