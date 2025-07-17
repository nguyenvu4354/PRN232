

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        public static string? GetUserIdFromToken(HttpContext httpContext)
        {
            var token = GetAccessToken(httpContext);
            if (string.IsNullOrEmpty(token))
                return null;

            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = handler.ReadJwtToken(token);

                // Lấy claim theo tên (phụ thuộc vào server JWT trả về)
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid");

                return userIdClaim?.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid JWT token: " + ex.Message);
                return null;
            }
        }

        public static string? GetRole(HttpContext httpContext)
        {
            var token = GetAccessToken(httpContext);
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            }
            catch
            {
                return null;
            }
        }

    }
}
