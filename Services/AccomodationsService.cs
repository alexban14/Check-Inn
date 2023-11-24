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
        CheckInnContext context;

        public AccomodationsService()
        {
            context = new CheckInnContext();
        }

        public IEnumerable<Accomodation> GetAllAcomodation()
        {
            return context.Accomodations.ToList();
        }

        public IEnumerable<Accomodation> GetAllAccomodationsByAccomodationType(int accomodationPackageId)
        {
            return context.Accomodations.Where(x => x.AccomodationPackageID == accomodationPackageId).ToList();
        }

        public IEnumerable<Accomodation> SearchAccomodation(string searchTerm, int? AccomodationPackageID, int? page, int? recordSize)
        {
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
            return context.Accomodations.Find(ID);
        }

        public bool SaveAccomodation(Accomodation accomodation)
        {
            context.Accomodations.Add(accomodation);

            return context.SaveChanges() > 0;
        }

        public bool UpdateAccomodation(Accomodation accomodation)
        {
            context.Entry(accomodation).State = System.Data.Entity.EntityState.Modified;

            return context.SaveChanges() > 0;
        }

        public bool DeleteAccomodation(Accomodation accomodation)
        {
            context.Entry(accomodation).State = System.Data.Entity.EntityState.Deleted;

            return context.SaveChanges() > 0;
        }

    }
}