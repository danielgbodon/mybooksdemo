using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyBooks.Core;
using MyBooks.Core.Interfaces;
using MyBooks.Models;
using MyBooks.ViewModels;

namespace MyBooks.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IBookList _bookList;
        private readonly IAntiforgery _antiforgery;
        private readonly IStringLocalizer<ApplicationUser> _localizer;

        public UserController(IUser user, IBookList bookList, IAntiforgery antiforgery, IStringLocalizer<ApplicationUser> localizer)
        {
            _user = user;
            _bookList = bookList;
            _antiforgery = antiforgery;
            _localizer = localizer;
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<object> Register([FromForm]  RegisterUserViewModel registerUser)
        {
            try
            {
                string currentCulture = System.Globalization.CultureInfo.CurrentCulture.ToString().ToLower();
                registerUser.Culture = (AppEnvironment.SupportedCultures.ContainsValue(currentCulture)) ?
                    currentCulture : AppEnvironment.SupportedCultures.Values.First();

                UserModel result = await _user.Register(registerUser.getUser(), registerUser.Password, registerUser.Role);
                AppEnvironment.CurrentUser = result;
                await createDefaultBookList(result);
                
                return new ResponseViewModel()
                {
                    Message = _localizer["UsuarioRegistradoOK"],
                };
            }
            catch (Exception ex)
            {
                return new ResponseViewModel()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    VerificationToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken
                };
            }
            
        }

        private async Task createDefaultBookList(UserModel user)
        {
            int pos = 0;
            foreach(KeyValuePair<string, string> bl in AppEnvironment.DefaultBookLists)
            {
                await _bookList.Add(new BookListModel() { Name = bl.Key, Color = bl.Value, User = user, Order = pos, Removable = false });
                pos++;
            }
        }

        [HttpPost("resetpassword")]
        [ValidateAntiForgeryToken]
        public Task<ResponseViewModel> ResetPassword([FromForm]  UserModel user)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ResponseViewModel> Edit([FromForm]  UserModel user)
        {
            UserModel result = await _user.Edit(user);
            if(result == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await Response.WriteAsync(_localizer["UsuarioEditadoKO"]);
                return null;
            }

            return new ResponseViewModel()
            {
                Message = _localizer["UsuarioEditadoOK"],
                Result = result
            };
        }

        [HttpPut("culture")]
        [ValidateAntiForgeryToken]
        public async Task<ResponseViewModel> ChangeCulture([FromForm]  string Culture)
        {
            Culture = Culture.ToLower();
            if (!AppEnvironment.SupportedCultures.ContainsKey(Culture))
            {
                return new ResponseViewModel()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = _localizer["IdiomaNoDisponible"],
                    VerificationToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken
                };
            }

            AppEnvironment.CurrentUser.Culture = AppEnvironment.SupportedCultures[Culture];
            await _user.Edit(AppEnvironment.CurrentUser);

            var user = User as ClaimsPrincipal;
            var identity = User.Identity as ClaimsIdentity;
            identity.RemoveClaim(User.FindFirst(AppEnvironment.CLAIM_KEY_CULTURE));
            identity.AddClaim(new Claim(AppEnvironment.CLAIM_KEY_CULTURE, AppEnvironment.CurrentUser.Culture));

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, user);

            return new ResponseViewModel()
            {
                Message = _localizer["IdiomaCambiado"],
                VerificationToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken
            };
        }
    }
}