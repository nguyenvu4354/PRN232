using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.Models;
using ShoppingWeb.MvcClient.Services;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var result = await _authService.LoginAsync(model);

        if (result == null || result.Data.Token == null || result.Data == null)
        {
            ViewBag.Error = result?.Message ?? "Login failed";
            return View(model);
        }

        Response.Cookies.Append("AccessToken", result.Data.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = new DateTimeOffset(result.Data.Expires)
        });

        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (model.Password != model.ConfirmPassword)
        {
            ViewBag.Error = "Passwords do not match.";
            return View(model);
        }

        var success = await _authService.RegisterAsync(model);
        if (success == null)
        {
            ViewBag.Error = success?.ErrorCode ?? "Registration failed.";
            return View(model);
        }

        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        var accessToken = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth");


        var success = await _authService.LogoutAsync(accessToken);

        // Clear cookie
        Response.Cookies.Delete("AccessToken");

        return RedirectToAction("Index", "Home");
    }
}