using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MyBooks.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBooks.Core.Middlewares
{
    public class UserLoggedMiddleware
    {
        private readonly RequestDelegate _next;
        private List<KeyValuePair<string, string>> permittedUrls;
        public UserLoggedMiddleware(RequestDelegate next)
        {
            _next = next;
            permittedUrls = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("/user/welcome", "get"),
                new KeyValuePair<string, string>("/api/book", "get"),
                new KeyValuePair<string, string>("/user/welcomequestions", "post")
            };
        }

        public async Task InvokeAsync(HttpContext context, UserManager<UserModel> _userManager)
        {

            if (!context.User.Identity.IsAuthenticated)
            {
                AppEnvironment.CurrentUser = null;
            }
            else
            {
                try
                {
                    AppEnvironment.CurrentUser = await _userManager.GetUserAsync(context.User);
                    if (AppEnvironment.CurrentUser != null && !AppEnvironment.CurrentUser.Welcomed)
                    {
                        string currentUrl = context.Request.Path.ToString().ToLower();
                        string method = context.Request.Method.ToLower();
                    
                        if(!permittedUrls.Contains(new KeyValuePair<string, string>(currentUrl, method)))
                            context.Response.Redirect("/user/welcome");
                    }
                }
                catch(Exception ex)
                {
                    string msj = ex.Message;
                }
                
            }

            await _next(context);
        }
    }
}
