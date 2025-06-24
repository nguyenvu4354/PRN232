using ShoppingWeb.DTOs;
using ShoppingWeb.DTOs.User;
using ShoppingWeb.Models;

namespace ShoppingWeb.Mapping
{
    public class UserMapper
    {
        public static UserInfoDTO toResponseDto(User user)
        {
            return new UserInfoDTO
            {
                UserID = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                RoleID = 1
            };
        }

        public static UserProfileResponseDTO toResponseProfileDto(User user)
        {
            return new UserProfileResponseDTO
            {
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                CreatedAt = user.CreatedAt,
            };
        }

        public static void toEntity(UpdateUserProfileRequestDTO userRequestDTO, User user)
        {
            user.Address = userRequestDTO.Address;
            user.Email = userRequestDTO.Email;
            user.FullName = userRequestDTO.FullName;
            user.Phone = userRequestDTO.Phone;
        }
    }
}
