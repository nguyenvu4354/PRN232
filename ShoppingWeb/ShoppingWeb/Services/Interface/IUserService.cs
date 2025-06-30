using ShoppingWeb.DTOs.User;

namespace ShoppingWeb.Services.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserListItemResponseDTO>> GetAllUsersAsync();
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDTO passwordRequestDTO);
        Task<UserProfileResponseDTO> ViewProfileAsync(int userId);
        Task<UserProfileResponseDTO> UpdateProfileAsync(int userId, UpdateUserProfileRequestDTO profileRequestDTO);
    }
}
