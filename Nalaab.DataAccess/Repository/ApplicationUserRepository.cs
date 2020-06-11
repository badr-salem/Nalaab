using Nalaab.DataAccess.Data;
using Nalaab.DataAccess.Repository.IRepository;
using Nalaab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nalaab.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationUser applicationUser)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(s => s.Id == applicationUser.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = applicationUser.Name;
                objFromDb.PhoneNumber = applicationUser.PhoneNumber;
                objFromDb.City = applicationUser.City;
                objFromDb.StreetAddress = applicationUser.StreetAddress;
                objFromDb.State = applicationUser.State;
                objFromDb.PostalCode = applicationUser.PostalCode;

            }
        }

    }
}
