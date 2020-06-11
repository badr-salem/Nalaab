using Nalaab.DataAccess.Data;
using Nalaab.DataAccess.Repository.IRepository;
using Nalaab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nalaab.DataAccess.Repository
{
    public class AgeGroupRepository : Repository<AgeGroup>, IAgeGroupRepository
    {
        private readonly ApplicationDbContext _db;

        public AgeGroupRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(AgeGroup ageGroup)
        {
            var objFromDb = _db.Categories.FirstOrDefault(s => s.Id == ageGroup.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = ageGroup.Name;
               
            }
        }
    }
}
