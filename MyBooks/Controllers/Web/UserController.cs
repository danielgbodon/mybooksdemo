using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyBooks.Core;
using MyBooks.Core.Interfaces;
using MyBooks.Models;
using MyBooks.ViewModels;

namespace MyBooks.Controllers.Web
{
    
    public class UserController : Controller
    {
        private readonly IUser _user;
        private readonly IBookList _bookList;
        private readonly IStringLocalizer<ApplicationUser> _localizer;

        public UserController(IUser user, IBookList bookList, IStringLocalizer<ApplicationUser> localizer)
        {
            _user = user;
            _bookList = bookList;
            _localizer = localizer;
        }

        [AllowAnonymous]
        public IActionResult Login(string errorMessage, string ReturnUrl = null)
        {
            if (!string.IsNullOrEmpty(errorMessage)) ViewData["ErrorMessage"] = errorMessage;
            ViewData["ReturnUrl"] = (string.IsNullOrEmpty(ReturnUrl)) ? "/" : ReturnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginUserViewModel loginUser, string returnUrl)
        {
            UserModel userChecked = await _user.CheckUser(loginUser.Username, loginUser.Password);
            if (userChecked == null)
            {
                return Login(_localizer["LoginIncorrecto"]);
            }

            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userChecked.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, userChecked.UserName));
            identity.AddClaim(new Claim(AppEnvironment.CLAIM_KEY_CULTURE, userChecked.Culture));

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(identity));

            return Redirect((string.IsNullOrEmpty(returnUrl)) ? "/" : returnUrl);
        }

        public IActionResult Welcome(string errorMessage="")
        {
            if (!string.IsNullOrEmpty(errorMessage)) ViewData["ErrorMessage"] = errorMessage;
            ViewData["HideHeader"] = true;
            ViewData["HideSidebar"] = true;
            return View("Welcome");
        }

        [HttpPost]
        public async Task<IActionResult> WelcomeQuestions([FromForm] string ReadedBookId, [FromForm] string CurrentBookId,[FromForm] string DesiredBookId)
        {
            if(AppEnvironment.CurrentUser.Welcomed) return Redirect("/");

            List<string> listElems = new List<string>() { CurrentBookId, ReadedBookId, DesiredBookId };
            if((listElems.Distinct().ToList()).Count != listElems.Count)
                return Welcome(_localizer["BienvenidaErrorMismoLibro"]);

            AppEnvironment.CurrentUser.Welcomed = true;
            try
            {
                List<BookListModel> userBookLists = await _bookList.GetAll();
                await _bookList.AddBookToList(ReadedBookId, userBookLists[0].Id);
                await _bookList.AddBookToList(CurrentBookId, userBookLists[1].Id);
                await _bookList.AddBookToList(DesiredBookId, userBookLists[2].Id);

                AppEnvironment.CurrentUser = await _user.Edit(AppEnvironment.CurrentUser);
                return Redirect("/");
            }
            catch(Exception ex)
            {
                return Welcome(ex.Message);
            }
        }

        public IActionResult Logout()
        {
            _user.Logout();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Profile()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewData["Load"] = "Register";
            return View("Login");
        }

        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            ViewData["Load"] = "ResetPassword";
            return View("Login");
        }
    }
}