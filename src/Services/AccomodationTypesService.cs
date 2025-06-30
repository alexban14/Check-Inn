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
        private readonly ICheckInnContext _context;

        public AccomodationTypesService(ICheckInnContext context)
        {
            _context = context;
        }
        public IEnumerable<AccomodationType> GetAllAccomodationTypes()
        {
            return _context.AccomodationTypes.ToList();
        }

        public IEnumerable<AccomodationType> SearchAccomodationType(string searchTerm)
        {
            IEnumerable<AccomodationType> accomodationTypes = _context.AccomodationTypes.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                accomodationTypes = accomodationTypes.Where( a => a.Name.ToLower().Contains(searchTerm.ToLower()) );
            }

            return accomodationTypes.ToList();
        }

        public AccomodationType GetAccomodationTypeByID(int ID)
        {
            return _context.AccomodationTypes.Find(ID);
        }

        public bool SaveAccomodationType(AccomodationType accomodationType)
        {
            _context.AccomodationTypes.Add(accomodationType);

            return _context.SaveChanges() > 0;
        }

        public bool UpdateAccomodationType(AccomodationType accomodationType)
        {
            _context.Entry(accomodationType).State = System.Data.Entity.EntityState.Modified;

            return _context.SaveChanges() > 0;
        }
        public bool DeleteAccomodationType(AccomodationType accomodationType)
        {
            _context.Entry(accomodationType).State = System.Data.Entity.EntityState.Deleted;

            return _context.SaveChanges() > 0;
        }
    }
}