using Nalaab.DataAccess.Data;
using Nalaab.DataAccess.Repository.IRepository;
using Nalaab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nalaab.DataAccess.Repository
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        private readonly ApplicationDbContext _db;

        public CouponRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Coupon coupon)
        {
            var objFromDb = _db.Coupons.FirstOrDefault(s => s.Id == coupon.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = coupon.Name;
                objFromDb.MinimumAmount = coupon.MinimumAmount;
                objFromDb.Discount = coupon.Discount;
                objFromDb.CouponType = coupon.CouponType;
                objFromDb.IsActive = coupon.IsActive;
                

            }
        }
    }
}
