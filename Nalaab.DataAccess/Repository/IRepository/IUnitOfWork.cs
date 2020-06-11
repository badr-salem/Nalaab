using System;
using System.Collections.Generic;
using System.Text;

namespace Nalaab.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IAgeGroupRepository AgeGroup { get; }
        IProductRepository Product { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailsRepository OrderDetails { get; }
        ICouponRepository Coupon { get; }


        ISP_Call SP_Call { get; }


        void Save();
    }
}
