using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.DTOs.Common;
using ShoppingWeb.MvcClient.DTOs.User;
using ShoppingWeb.MvcClient.Response;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShoppingWeb.MvcClient.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShoppingApi");
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"user/paged?page={page}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "API Error: " + response.StatusCode;
                return View(new PagedResultDTO<UserListItemResponseDTO>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponseDTO<PagedResultDTO<UserListItemResponseDTO>>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(result?.Data);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _httpClient.GetAsync($"user/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "API Error: " + response.StatusCode;
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponseDTO<UserListItemResponseDTO>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(result?.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var getUserResponse = await _httpClient.GetAsync($"user/{id}");

            if (!getUserResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to retrieve user.";
                return RedirectToAction("Index");
            }

            var json = await getUserResponse.Content.ReadAsStringAsync();
            var userResult = JsonSerializer.Deserialize<ApiResponseDTO<UserListItemResponseDTO>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var currentStatus = userResult?.Data?.IsActive ?? true;

            var updatePayload = new
            {
                IsActive = !currentStatus
            };

            var patchContent = new StringContent(JsonSerializer.Serialize(updatePayload), System.Text.Encoding.UTF8, "application/json");

            var patchResponse = await _httpClient.PatchAsync($"user/{id}/status", patchContent);

            if (patchResponse.IsSuccessStatusCode)
            {
                TempData["Success"] = "User status updated.";
            }
            else
            {
                TempData["Error"] = "Failed to update user status.";
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Search(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                TempData["Error"] = "Username cannot be empty.";
                return RedirectToAction("Index");
            }

            var response = await _httpClient.GetAsync($"user/search?username={Uri.EscapeDataString(username)}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to search users.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponseDTO<IEnumerable<UserListItemResponseDTO>>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var searchResult = new PagedResultDTO<UserListItemResponseDTO>
            {
                Items = result?.Data?.ToList() ?? new List<UserListItemResponseDTO>(),
                Page = 1,
                PageSize = result?.Data?.Count() ?? 10,
                TotalItems = result?.Data?.Count() ?? 0
            };

            return View("Index", searchResult);
        }

        // [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            var accessToken = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["Error"] = "Please login first!";
                return RedirectToAction("Login", "Auth");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("User/profile");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Auth");
            }

            var json = await response.Content.ReadAsStringAsync();
            var wrapper = JsonSerializer.Deserialize<ApiResponse<UserProfileResponseDTO>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (wrapper?.Data == null)
            {
                TempData["Error"] = "Cannot load profile.";
                return RedirectToAction("Login", "Auth");
            }

            return View(wrapper.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileResponseDTO model)
        {
            var accessToken = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["Error"] = "Please login!";
                return RedirectToAction("Login", "Auth");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Map sang DTO gửi API
            var updateDto = new UserProfileUpdateDTO
            {
                FullName = model.FullName,
                Phone = model.Phone,
                Address = model.Address,
                Email = model.Email,
                Username = model.Username
            };

            var response = await _httpClient.PostAsJsonAsync("User/update-profile", updateDto);
            Console.WriteLine($"Update Profile Response: {response.StatusCode}");
            Console.WriteLine($"Update Profile Response: {response}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Update failed.";
                return RedirectToAction("Profile", model);
            }

            TempData["Success"] = "Profile updated!";
            return RedirectToAction("Profile");
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDTO requestDto)
        {
            if (requestDto.NewPassword != requestDto.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(requestDto); // hoặc View mới
            }

            var accessToken = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["Error"] = "Please login first!";
                return RedirectToAction("Login", "Auth");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsJsonAsync("User/change-password", requestDto); // Gọi API đổi pass

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Change password failed.";
                return View(requestDto);
            }

            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("ChangePassword"); // hoặc Profile
        }

    }
}
