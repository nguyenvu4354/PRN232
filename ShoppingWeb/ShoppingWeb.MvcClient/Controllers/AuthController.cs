using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.DTOs.Auth;
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

        // Kiểm tra role, nếu là STAFF thì redirect về StaffProductList
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(result.Data.Token);
        var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        if (role == "STAFF")
            return RedirectToAction("StaffProductList", "StaffProduct");

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

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Error = "Email is required.";
            return View();
        }

        var success = await _authService.ForgotPasswordAsync(request.Email);
        if (!success)
        {
            ViewBag.Error = "Failed to send password reset email. Please check the email address.";
            return View();
        }
        ViewBag.Message = "Password reset email has been sent. Please check your inbox.";
        return View();
    }

    [HttpGet("reset-password")]
    public IActionResult ResetPassword(string token, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            ViewBag.Error = "Invalid reset link.";
            return View();
        }

        var model = new ResetPasswordViewModel
        {
            Token = token,
            Email = email
        };
        return View(model);
    }

    [HttpPost("reset-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _authService.ResetPasswordAsync(model);
            if (!result.Success)
            {
                ViewBag.Error = "Failed to reset password. Please try again.";
                return View(model);
            }

            ViewBag.Success = "Password reset successfully. You can now log in with your new password.";
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            ViewBag.Error = "An unexpected error occurred. Please try again.";
            return View(model);
        }
    }
}



