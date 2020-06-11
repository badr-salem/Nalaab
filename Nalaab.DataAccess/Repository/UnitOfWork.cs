using Nalaab.DataAccess.Data;
using Nalaab.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nalaab.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            AgeGroup = new AgeGroupRepository(_db);
            Product = new ProductRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            Coupon = new CouponRepository(_db);

            SP_Call = new SP_Call(_db);
        }

        public ICategoryRepository Category { get; private set; }
        public IAgeGroupRepository AgeGroup { get; private set; }
        public IProductRepository Product { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public ICouponRepository Coupon { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }


        public ISP_Call SP_Call { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
