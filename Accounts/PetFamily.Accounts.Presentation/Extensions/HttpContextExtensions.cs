using Microsoft.AspNetCore.Http;

namespace PetFamily.Accounts.Presentation.Controllers;

public static class HttpContextExtensions
{
    public static void AddRefreshTokenCookie(this HttpContext httpContext, string refreshToken)
    {
        var isHttps = httpContext.Request.IsHttps;
        httpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
    }
}