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
using System.Security.Cryptography;

namespace ShoppingWeb.Services
{
    public class UserService : IUserService
    {
        private ShoppingWebContext _context;
        private ILogger<UserService> _logger;
        private readonly IEmailService _emailService;

        public UserService(ShoppingWebContext context, ILogger<UserService> logger, IEmailService emailService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

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
        public async Task<UserListItemResponseDTO> CreateStaffUserAsync(CreateUserRequestDTO requestDTO)
        {
            if (requestDTO == null)
                throw new ArgumentNullException(nameof(requestDTO));

            var randomSuffix = GenerateRandomSuffix(5);
            var usernameWithSuffix = requestDTO.Username + randomSuffix;

            if (await _context.Users.AnyAsync(u => u.Username == usernameWithSuffix))
                throw new ArgumentException("Generated username already exists. Please try again.");

            var user = new User
            {
                Username = usernameWithSuffix,
                Email = requestDTO.Email,
                FullName = requestDTO.FullName,
                Phone = requestDTO.Phone,
                Address = requestDTO.Address,
                RoleId = (int)UserRole.STAFF,
                PasswordHash = PasswordHelper.HashPassword("123"),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new staff user: {Username}", user.Username);

            try
            {
                await _emailService.SendWelcomeEmailAsync(user.Email, user.Username, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", user.Email);
            }

            return new UserListItemResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                RoleName = "Staff",
                IsActive = user.IsActive
            };
        }

        private string GenerateRandomSuffix(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[RandomNumberGenerator.GetInt32(s.Length)]).ToArray());
        }
    }
}
