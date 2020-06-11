using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nalaab.DataAccess.Repository.IRepository;
using Nalaab.Models;

namespace Nalaab.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;


        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager , IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "رقم الجوال")]
            public string PhoneNumber { get; set; }
            [Display(Name = "الإسم")]
            public string Name { get; set; }

            [Display(Name = "العنوان")]
            public string StreetAddress { get; set; }

            [Display(Name = "المدينة")]
            public string City { get; set; }

            [Display(Name = "الحي")]
            public string State { get; set; }

            [Display(Name = "الرمز البريدي")]
            public string PostalCode { get; set; }


        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == user.Id);
            var name = userFromDb.Name;
            var city = userFromDb.City;
            var state = userFromDb.State;
            var streetAdress = userFromDb.StreetAddress;
            var postalCode = userFromDb.PostalCode;



            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Name = name,
                City = city,
                State = state,
                PostalCode = postalCode,
                StreetAddress = streetAdress

            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            ApplicationUser userFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == user.Id);
            var name = userFromDb.Name;
            var city = userFromDb.City;
            var state = userFromDb.State;
            var streetAdress = userFromDb.StreetAddress;
            var postalCode = userFromDb.PostalCode;



            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            if (ModelState.IsValid)
            {
                userFromDb.Name = Input.Name;
                userFromDb.City = Input.City;
                userFromDb.StreetAddress = Input.StreetAddress;
                userFromDb.State = Input.State;
                userFromDb.PostalCode = Input.PostalCode;

                _unitOfWork.Save();

            }


            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "تم تحديث ملفك الشخصي بنجاح";
            return RedirectToPage();
        }
    }
}
