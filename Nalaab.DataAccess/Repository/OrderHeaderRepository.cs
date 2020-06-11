using Nalaab.DataAccess.Data;
using Nalaab.DataAccess.Repository.IRepository;
using Nalaab.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nalaab.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            _db.Update(obj);
        }
    }
}
