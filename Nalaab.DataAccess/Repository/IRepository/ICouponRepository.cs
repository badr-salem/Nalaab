using Nalaab.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nalaab.DataAccess.Repository.IRepository
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        void Update(Coupon coupon);
    }
}
