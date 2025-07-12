using ShoppingWeb.MvcClient.DTOs.Auth;
using ShoppingWeb.MvcClient.Models;
using ShoppingWeb.MvcClient.Response;
using ShoppingWeb.MvcClient.Services;
using System.Net.Http.Headers;
using System.Text.Json;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginViewModel model)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AuthApi");
            var response = await client.PostAsJsonAsync("Auth/login", model);
            Console.WriteLine("Raw JSON response: " + response);
            var bodyJson = JsonSerializer.Serialize(model);
            Console.WriteLine("📤 Request JSON being sent: " + bodyJson);



            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();


            var wrapper = JsonSerializer.Deserialize<ApiResponse<AuthResponseDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (wrapper == null || !wrapper.Success || wrapper.Data == null)
                return null;



            return wrapper;

        }
        catch (Exception ex)
        {
            // Log the exception (not implemented here)
            Console.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }


    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterViewModel model)
    {
        var client = _httpClientFactory.CreateClient("AuthApi");
        var response = await client.PostAsJsonAsync("Auth/register", model);
        var json = await response.Content.ReadAsStringAsync();

        Console.WriteLine("Raw response: " + response);
        Console.WriteLine("Raw JSON response: " + json);

        var wrapper = JsonSerializer.Deserialize<ApiResponse<AuthResponseDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (wrapper == null || !wrapper.Success || wrapper.Data == null)
            return null;


        if (response.IsSuccessStatusCode)
        {
            return wrapper;
        }

        return null;
    }

    public async Task<bool> LogoutAsync(string accessToken)
    {
        var client = _httpClientFactory.CreateClient("AuthApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.PostAsync("Auth/logout", null); // hoặc thêm `new StringContent("")` nếu API yêu cầu body

        return response.IsSuccessStatusCode;
    }
}