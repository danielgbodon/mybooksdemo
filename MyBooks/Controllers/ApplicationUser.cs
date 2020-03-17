using Microsoft.AspNetCore.Identity;
using MyBooks.Core;
using MyBooks.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MyBooks.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace MyBooks.Controllers
{

    public class ApplicationUser : IUser
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStringLocalizer<ApplicationUser> _localizer;

        public ApplicationUser(
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            RoleManager<IdentityRole> roleManager,
            IStringLocalizer<ApplicationUser> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _localizer = localizer;
        }
        
        public async Task<UserModel> Edit(UserModel user)
        {
            if (user.Id != AppEnvironment.CurrentUser.Id)
                throw new Exception(_localizer["SinPermisoEdicion"]);

            try
            {
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new Exception(_localizer["UsuarioEditadoKO"]);

                return user;
            }
            catch (DbUpdateConcurrencyException)
            {
                return await _userManager.FindByIdAsync(user.Id);
            }
        }

        public async Task<UserModel> CheckUser(string username, string password)
        {
            var user = await _userManager.FindByEmailAsync(username);
            if (user == null) return null;

            bool passOk = await _userManager.CheckPasswordAsync(user, password);
            if (!passOk) return null;

            return user;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserModel> Register(UserModel user, string password, string role)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (password == null) password = "";
                    IdentityResult result = await _userManager.CreateAsync(user, password);
                    if (!result.Succeeded)
                        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description).ToList()));

                    if (!(await _roleManager.RoleExistsAsync(role)))
                    {
                        result = await _roleManager.CreateAsync(new IdentityRole(role));
                        if (!result.Succeeded)
                            throw new Exception(_localizer["CreacionRolesKO"]);
                    }

                    result = await _userManager.AddToRoleAsync(user, role);
                    if (!result.Succeeded)
                        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description).ToList()));

                    scope.Complete();
                    return user;                    
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
        }
    }
}
