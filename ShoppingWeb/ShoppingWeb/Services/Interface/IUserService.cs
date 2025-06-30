using ShoppingWeb.DTOs.Common;
using ShoppingWeb.DTOs.User;

namespace ShoppingWeb.Services.Interface
{
    public interface IUserService
    {
        Task<PagedResultDTO<UserListItemResponseDTO>> GetUsersPagedAsync(int page, int pageSize);
        Task<IEnumerable<UserListItemResponseDTO>> GetAllUsersAsync();

        Task<UserListItemResponseDTO> GetUserDetailByIdAsync(int userId);

        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDTO passwordRequestDTO);
        Task<UserProfileResponseDTO> ViewProfileAsync(int userId);
        Task<UserProfileResponseDTO> UpdateProfileAsync(int userId, UpdateUserProfileRequestDTO profileRequestDTO);
    }
}
