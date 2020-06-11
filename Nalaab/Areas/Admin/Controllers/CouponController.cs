using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nalaab.DataAccess.Repository.IRepository;
using Nalaab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Nalaab.Utility;

namespace Nalaab.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

    public class CouponController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View(_unitOfWork.Coupon.GetAll().ToList());
        }

        public IActionResult Upsert(int? id)
        {
            Coupon coupon = new Coupon();
            if (id == null)
            {
                //this is for create
                return View(coupon);
            }
            //this is for edit
            coupon = _unitOfWork.Coupon.Get(id.GetValueOrDefault());
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                if (coupon.Id == 0)
                {
                    _unitOfWork.Coupon.Add(coupon);

                }
                else
                {
                    _unitOfWork.Coupon.Update(coupon);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon =  _unitOfWork.Coupon.GetFirstOrDefault(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var coupons = _unitOfWork.Coupon.GetFirstOrDefault(m => m.Id == id);
            _unitOfWork.Coupon.Remove(coupons);
             _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Coupon.GetAll();
            return Json(new { data = allObj });
        }

        //[HttpDelete]
        //public IActionResult Delete(int id)
        //{
        //    var objFromDb = _unitOfWork.Coupon.Get(id);
        //    if (objFromDb == null)
        //    {
        //        return Json(new { success = false, message = "حدث خطأ أثناء الحذف" });
        //    }
        //    _unitOfWork.Coupon.Remove(objFromDb);
        //    _unitOfWork.Save();
        //    return Json(new { success = true, message = "تم الحذف بنجاح" });

        //}

        #endregion
    }
}