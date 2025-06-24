using ShoppingWeb.DTOs.User;

namespace ShoppingWeb.Services.Interface
{
    public interface IUserService
    {
        Task<bool> ChangePassowrdAsync(int userId, ChangePasswordRequestDTO passwordRequestDTO);
        Task<UserProfileResponseDTO> ViewProfileAsync(int userId);
        Task<UserProfileResponseDTO> UpdateProfileAsync(int userId,UpdateUserProfileRequestDTO profileRequestDTO);
    }
}
