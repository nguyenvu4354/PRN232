using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Data;
using ShoppingWeb.DTOs.Common;
using ShoppingWeb.DTOs.User;
using ShoppingWeb.Enum;
using ShoppingWeb.Exceptions;
using ShoppingWeb.Helpers;
using ShoppingWeb.Mapping;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class UserService : IUserService
    {
        private ShoppingWebContext _context;
        private ILogger<UserService> _logger;

        public UserService(ShoppingWebContext context, ILogger<UserService> logger)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }
        public async Task<IEnumerable<UserListItemResponseDTO>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId != (int)UserRole.ADMIN)
                .ToListAsync();

            var result = users.Select(u => new UserListItemResponseDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Address = u.Address,
                RoleName = u.Role.RoleName
            });

            _logger.LogInformation("Fetched {Count} non-admin users.", result.Count());
            return result;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDTO passwordRequestDTO)
        {
            //var userId = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (passwordRequestDTO == null || string.IsNullOrWhiteSpace(passwordRequestDTO.NewPassword))
            {
                _logger.LogWarning("Invalid password change request: DTO is null or password is empty.");
                throw new ArgumentException("New password cannot be empty.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found during password change.", userId);
                throw new UserNotFoundException("User not found.");
            }

            user.PasswordHash = PasswordHelper.HashPassword(passwordRequestDTO.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Password changed successfully for user ID {UserId}.", userId);
            return true;
        }

        public async Task<UserProfileResponseDTO> UpdateProfileAsync(int userId, UpdateUserProfileRequestDTO profileRequestDTO)
        {

            if (profileRequestDTO == null)
            {
                _logger.LogWarning("Invalid profile update request: DTO is null.");
                throw new ArgumentNullException(nameof(profileRequestDTO));
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found during profile update.", userId);
                throw new UserNotFoundException("User not found.");
            }

            UserMapper.toEntity(profileRequestDTO, user);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();


            _logger.LogInformation("Profile updated successfully for user ID {UserId}.", userId);
            return UserMapper.toResponseProfileDto(user);
        }

        public async Task<UserProfileResponseDTO> ViewProfileAsync(int userId)
        {

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found during profile update.", userId);
                throw new UserNotFoundException("User not found.");
            }

            return UserMapper.toResponseProfileDto(user);
        }
        public async Task<PagedResultDTO<UserListItemResponseDTO>> GetUsersPagedAsync(int page, int pageSize)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId != (int)UserRole.ADMIN);

            var totalItems = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = users.Select(u => new UserListItemResponseDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Address = u.Address,
                RoleName = u.Role.RoleName,
                IsActive = u.IsActive
            });

            return new PagedResultDTO<UserListItemResponseDTO>
            {
                Items = result,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }
        public async Task<bool> UpdateUserStatusAsync(int userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found when updating IsActive.", userId);
                throw new UserNotFoundException("User not found.");
            }

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User ID {UserId} status updated to {Status}.", userId, isActive);
            return true;
        }
        public async Task<IEnumerable<UserListItemResponseDTO>> SearchUsersByUsernameAsync(string username)
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Username.Contains(username))
                .ToListAsync();

            return users.Select(u => new UserListItemResponseDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Address = u.Address,
                RoleName = u.Role.RoleName,
                IsActive = u.IsActive
            });
        }

        public async Task<UserListItemResponseDTO> GetUserDetailByIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.RoleId != (int)UserRole.ADMIN);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                throw new UserNotFoundException("User not found.");
            }

            return new UserListItemResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                RoleName = user.Role.RoleName,
                IsActive = user.IsActive
            };
        }
    }
}
