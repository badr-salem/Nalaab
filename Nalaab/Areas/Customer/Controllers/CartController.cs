using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nalaab.DataAccess.Repository.IRepository;
using Nalaab.Models;
using Nalaab.Models.ViewModels;
using Nalaab.Utility;
using Stripe;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Nalaab.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private TwilioSettings _twilioOptions { get; set; }
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager , IOptions<TwilioSettings> twilionOptions)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
            _twilioOptions = twilionOptions.Value;
        }

        public IActionResult Index()
        {
           
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart
                 .GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product").ToList()

            };
            ShoppingCartVM.OrderHeader.OrderTotalOriginal = 0;
           
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                                                        .GetFirstOrDefault(u => u.Id == claim.Value);

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = list.Product.Price;
                ShoppingCartVM.OrderHeader.OrderTotalOriginal += (list.Price * list.Count);
                list.Product.Description = SD.ConvertToRawHtml(list.Product.Description);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            }
            ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotalOriginal;


            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                ShoppingCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.GetFirstOrDefault(c => c.Name.ToLower() == ShoppingCartVM.OrderHeader.CouponCode.ToLower());
                ShoppingCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, ShoppingCartVM.OrderHeader.OrderTotalOriginal);
            }


            //if (CouponString != null)
            //{
            //    ShoppingCartVM.OrderHeader.CouponCode = CouponString;
            //    var couponFromDb = _unitOfWork.Coupon.GetFirstOrDefault(c => c.Name.ToLower() == ShoppingCartVM.OrderHeader.CouponCode.ToLower());
            //    ShoppingCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, ShoppingCartVM.OrderHeader.OrderTotalOriginal);

            //}


            return View(ShoppingCartVM);
        }


        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart
                 .GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product").ToList()

            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                .GetFirstOrDefault(c => c.Id == claim.Value);

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price =  list.Product.Price;

                ShoppingCartVM.OrderHeader.OrderTotalOriginal += (list.Price * list.Count);
            }
            ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotalOriginal;

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                ShoppingCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.GetFirstOrDefault(c => c.Name.ToLower() == ShoppingCartVM.OrderHeader.CouponCode.ToLower());
                ShoppingCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, ShoppingCartVM.OrderHeader.OrderTotalOriginal);
            }


            return View(ShoppingCartVM);
        }


        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                                                            .GetFirstOrDefault(c => c.Id == claim.Value);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart
                                        .GetAll(c => c.ApplicationUserId == claim.Value,
                                        includeProperties: "Product").ToList();

            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

           
            ShoppingCartVM.OrderHeader.OrderTotalOriginal = 0;
            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = item.Product.Price;
                    
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };

                ShoppingCartVM.OrderHeader.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                _unitOfWork.OrderDetails.Add(orderDetails);
                
            }

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                ShoppingCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.GetFirstOrDefault(c => c.Name.ToLower() == ShoppingCartVM.OrderHeader.CouponCode.ToLower());
                ShoppingCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, ShoppingCartVM.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotalOriginal;

            }

            ShoppingCartVM.OrderHeader.CouponCodeDiscount = ShoppingCartVM.OrderHeader.OrderTotalOriginal - ShoppingCartVM.OrderHeader.OrderTotal;

            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.ssShoppingCart, 0);

            
                //process the payment
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal * 100),
                    Currency = "sar",
                    Description = "Order ID : " + ShoppingCartVM.OrderHeader.Id,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);

                if (charge.Id == null)
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
                else
                {
                    ShoppingCartVM.OrderHeader.TransactionId = charge.Id;
                }
                if (charge.Status.ToLower() == "succeeded")
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
                    ShoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;
                }

            _unitOfWork.Save();

            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });

        }



        public IActionResult SummaryNoPay()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart
                 .GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product").ToList()

            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                .GetFirstOrDefault(c => c.Id == claim.Value);

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = list.Product.Price;

                ShoppingCartVM.OrderHeader.OrderTotalOriginal += (list.Price * list.Count);
            }
            ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotalOriginal;

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                ShoppingCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.GetFirstOrDefault(c => c.Name.ToLower() == ShoppingCartVM.OrderHeader.CouponCode.ToLower());
                ShoppingCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, ShoppingCartVM.OrderHeader.OrderTotalOriginal);
            }


            return View(ShoppingCartVM);
        }


        [HttpPost]
        [ActionName("SummaryNoPay")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryNoPayPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                                                            .GetFirstOrDefault(c => c.Id == claim.Value);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart
                                        .GetAll(c => c.ApplicationUserId == claim.Value,
                                        includeProperties: "Product").ToList();

            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            ShoppingCartVM.OrderHeader.OrderTotalOriginal = 0;
            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = item.Product.Price;

                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                ShoppingCartVM.OrderHeader.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                _unitOfWork.OrderDetails.Add(orderDetails);

            }

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                ShoppingCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.GetFirstOrDefault(c => c.Name.ToLower() == ShoppingCartVM.OrderHeader.CouponCode.ToLower());
                ShoppingCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, ShoppingCartVM.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotalOriginal;

            }

            ShoppingCartVM.OrderHeader.CouponCodeDiscount = ShoppingCartVM.OrderHeader.OrderTotalOriginal - ShoppingCartVM.OrderHeader.OrderTotal;

            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
           
            HttpContext.Session.SetInt32(SD.ssShoppingCart, 0);


            //order will be created without online payment

            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            _unitOfWork.Save();


            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });

        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            TwilioClient.Init(_twilioOptions.AccountSid, _twilioOptions.AuthToken);
            try
            {
                var message = MessageResource.Create(
                    body: "شكرا لطلبك من نلعب رقم طلبك هو:" + id,
                    from: new Twilio.Types.PhoneNumber(_twilioOptions.PhoneNumber),
                    to: new Twilio.Types.PhoneNumber(orderHeader.PhoneNumber)
                    );
            }
            catch (Exception ex)
            {

            }
            return View(id);
        }




        public IActionResult AddCoupon()
        {
         
            
            if (String.IsNullOrEmpty(ShoppingCartVM.OrderHeader.CouponCode))
            {
                ShoppingCartVM.OrderHeader.CouponCode = "";
            }
            else {
                HttpContext.Session.SetString(SD.ssCouponCode, ShoppingCartVM.OrderHeader.CouponCode);
                //HttpContext.Session.SetObject(SD.ssCouponCode,  ShoppingCartVM.OrderHeader.CouponCode);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {

            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault
                            (c => c.Id == cartId, includeProperties: "Product");
            cart.Count += 1;
            cart.Price = cart.Product.Price;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault
                            (c => c.Id == cartId, includeProperties: "Product");

            if (cart.Count == 1)
            {
                var cnt = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);
            }
            else
            {
                cart.Count -= 1;
                cart.Price = cart.Product.Price;
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault
                            (c => c.Id == cartId, includeProperties: "Product");

            var cnt = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);


            return RedirectToAction(nameof(Index));
        }



    }
}