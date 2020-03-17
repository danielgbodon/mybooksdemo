using Microsoft.AspNetCore.Builder;

namespace MyBooks.Core.Middlewares
{
    public static class UserLoggedMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserLogged(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserLoggedMiddleware>();
        }
    }
}
