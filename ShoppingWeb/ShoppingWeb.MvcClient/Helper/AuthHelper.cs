namespace ShoppingWeb.MvcClient.Helper
{
    public static class AuthHelper
    {
        public static string GetAccessToken(HttpRequestBase request)
        {
            var token = request.Cookies["AccessToken"]?.Value;
            return string.IsNullOrEmpty(token) ? null : token;
        }

        public static bool IsAuthenticated(HttpRequestBase request)
        {
            return !string.IsNullOrEmpty(GetAccessToken(request));
        }
    }
}