using Nalaab.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nalaab.Utility
{
    public static class SD
    {
        public const string Role_User_Indi = "Individual Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";



        public const string ssShoppingCart = "Shoping Cart Session";
        public const string ssCouponCode = "ssCouponCode";

        //public const string StatusPending = "Pending";
        //public const string StatusApproved = "Approved";
        //public const string StatusInProcess = "Processing";
        //public const string StatusShipped = "Shipped";
        //public const string StatusCancelled = "Cancelled";
        //public const string StatusRefunded = "Refunded";
        public const string StatusPending = "قيد الإنتظار";
        public const string StatusApproved = "مؤكد";
        public const string StatusInProcess = "جاري المعالجة";
        public const string StatusShipped = "تم الشحن";
        public const string StatusCancelled = "ملغي";
        public const string StatusRefunded = "مرتجع";

        //public const string PaymentStatusPending = "Pending";
        //public const string PaymentStatusApproved = "Approved";
        //public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        //public const string PaymentStatusRejected = "Rejected";
        //public const string PaymentStatusRefunded = "Refunded";
        public const string PaymentStatusPending = "قيد الإنتظار";
        public const string PaymentStatusApproved = "مدفوع";
        public const string PaymentStatusDelayedPayment = "غير مدفوع";
        public const string PaymentStatusRejected = "مرفوضة";
        public const string PaymentStatusRefunded = "مرتجع للعميل";
        public const string PaymentStatusCancelled = "ملغي غير مدفوع";



        public static double DiscountedPrice(Coupon couponFromDb, double OriginalOrderTotal)
        {
            if (couponFromDb == null)
            {
                return OriginalOrderTotal;
            }
            else
            {
                if (couponFromDb.MinimumAmount > OriginalOrderTotal)
                {
                    return OriginalOrderTotal;
                }
                else
                {
                    //everything is valid
                    if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Riyal && couponFromDb.IsActive == true)
                    {
                        //$10 off $100
                        //return Math.Round(OriginalOrderTotal - couponFromDb.Discount, 2);
                         return (OriginalOrderTotal - couponFromDb.Discount);
                    }
                    if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Percent && couponFromDb.IsActive == true)
                    {
                        //10% off $100
                        //return Math.Round(OriginalOrderTotal - (OriginalOrderTotal * couponFromDb.Discount / 100), 2);
                        return (OriginalOrderTotal - ((couponFromDb.Discount / 100) * OriginalOrderTotal));
                    }
                }
            }
            return OriginalOrderTotal;
        }





        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

    }
}
