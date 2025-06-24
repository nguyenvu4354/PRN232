using System.Net;
using System.Security.Claims;
using ShoppingWeb.DTOs.User;
using ShoppingWeb.Helpers;
using ShoppingWeb.Mapping;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class UserService : IUserService
    {
        private ShoppingWebContext _context;

        public UserService (ShoppingWebContext context)
        {
            _context = context;
        }

        public async Task<bool> ChangePassowrdAsync(int userId,ChangePasswordRequestDTO passwordRequestDTO)
        {
            //var userId = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) throw new Exception("not login");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("user does not contain in database");

            user.PasswordHash = PasswordHelper.HashPassword(passwordRequestDTO.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserProfileResponseDTO> UpdateProfileAsync(int userId, UpdateUserProfileRequestDTO profileRequestDTO)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("user does not contain in database");

              UserMapper.toEntity(profileRequestDTO,user);
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return UserMapper.toResponseProfileDto(user);
        }

        public async Task<UserProfileResponseDTO> ViewProfileAsync(int userId)
        {

            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("user does not contain in database");
            return UserMapper.toResponseProfileDto(user);
        }
    }
}
