using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nalaab.DataAccess.Repository.IRepository;
using Nalaab.Models;
using Nalaab.Models.ViewModels;
using Nalaab.Utility;

namespace Nalaab.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(string SearchString)
        {
            ViewBag.CurrentFilter = SearchString;
            IndexVM IndexVM = new IndexVM();
            if (!String.IsNullOrEmpty(SearchString))
            {

                IndexVM.Product = from x in _unitOfWork.Product.GetAll(b => b.Title.Contains(SearchString), includeProperties: "Category,AgeGroup") select x;

                IndexVM.Coupon = _unitOfWork.Coupon.GetAll(u => u.IsActive == true).ToList();
                
            }
            else
            {
                IndexVM.Product = from x in _unitOfWork.Product.GetAll(includeProperties: "Category,AgeGroup").OrderByDescending(u=>u.Id) select x;

                IndexVM.Coupon = _unitOfWork.Coupon.GetAll(u => u.IsActive == true);
            }

               
           
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == claim.Value)
                    .ToList().Count();

                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }
            return View(IndexVM);
        }

        public IActionResult IndexWithCategory(string category)
        {
            IndexVM IndexVM = new IndexVM()
            {
                Product = _unitOfWork.Product.GetAll(u => u.Category.Name == category, includeProperties: "Category,AgeGroup"),
                Coupon = _unitOfWork.Coupon.GetAll(u => u.IsActive == true)

        };

            return View(IndexVM);
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.
                        GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,AgeGroup");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            return View(cartObj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                //then we will add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == CartObject.ApplicationUserId && u.ProductId == CartObject.ProductId
                    , includeProperties: "Product"
                    );

                if (cartFromDb == null)
                {
                    //no records exists in database for that product for that user
                    _unitOfWork.ShoppingCart.Add(CartObject);
                }
                else
                {
                    cartFromDb.Count += CartObject.Count;
                    //_unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart
                   .GetAll(c => c.ApplicationUserId == CartObject.ApplicationUserId)
                   .ToList().Count();

                //HttpContext.Session.SetObject(SD.ssShoppingCart, CartObject);
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);


                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productFromDb = _unitOfWork.Product.
                        GetFirstOrDefault(u => u.Id == CartObject.ProductId, includeProperties: "Category,AgeGroup");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };
                return View(cartObj);
            }


        }


    }
}