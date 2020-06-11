using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nalaab.DataAccess.Repository.IRepository;
using Nalaab.Models;
using Nalaab.Utility;

namespace Nalaab.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class AgeGroupController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AgeGroupController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            AgeGroup ageGroup = new AgeGroup();
            if (id == null)
            {
                //this is for create
                return View(ageGroup);
            }
            //this is for edit
            ageGroup = _unitOfWork.AgeGroup.Get(id.GetValueOrDefault());
            if (ageGroup == null)
            {
                return NotFound();
            }
            return View(ageGroup);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(AgeGroup ageGroup)
        {
            if (ModelState.IsValid)
            {
                if (ageGroup.Id == 0)
                {
                    _unitOfWork.AgeGroup.Add(ageGroup);

                }
                else
                {
                    _unitOfWork.AgeGroup.Update(ageGroup);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(ageGroup);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.AgeGroup.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.AgeGroup.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء الحذف" });
            }
            _unitOfWork.AgeGroup.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "تم الحذف بنجاح" });

        }

        #endregion
    }
}
