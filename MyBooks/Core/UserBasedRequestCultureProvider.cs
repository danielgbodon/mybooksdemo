using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Globalization;

namespace MyBooks.Core
{
    public class UserBasedRequestCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            CultureInfo requestCulture = CultureInfo.CurrentCulture;
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                //return Task.FromResult(new ProviderCultureResult("en-GB"));
                return Task.FromResult(new ProviderCultureResult(requestCulture.ToString()));
            }

            Claim cultureClaim = httpContext.User.FindFirst(AppEnvironment.CLAIM_KEY_CULTURE);
            if (cultureClaim == null || cultureClaim.Value == null)
            {
                return Task.FromResult(new ProviderCultureResult(requestCulture.ToString()));
            }

            return Task.FromResult(new ProviderCultureResult(cultureClaim.Value));
        }
    }
}
