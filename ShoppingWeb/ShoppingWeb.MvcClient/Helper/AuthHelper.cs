

namespace ShoppingWeb.MvcClient.Helper
{
    public class AuthHelper
    {
        public static string? GetAccessToken(HttpContext httpContext)
        {
            return httpContext.Request.Cookies["AccessToken"];
        }

        public static bool IsAuthenticated(HttpContext httpContext)
        {
            var token = GetAccessToken(httpContext);
            return string.IsNullOrEmpty(token);
        }

    }
}
