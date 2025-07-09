using ShoppingWeb.MvcClient.DTOs.Common;
using ShoppingWeb.MvcClient.DTOs.User;

namespace ShoppingWeb.MvcClient.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;

        public UserApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResultDTO<UserListItemResponseDTO>> GetPagedUsersAsync(int page, int pageSize)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponseDTO<PagedResultDTO<UserListItemResponseDTO>>>(
                $"https://localhost:7168/api/user/paged?page={page}&pageSize={pageSize}");

            return response?.Data!;
        }
    }
}
