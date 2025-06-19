using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShoppingWeb.Config;
using ShoppingWeb.DTOs;
using ShoppingWeb.Exceptions;
using ShoppingWeb.Models;
using ShoppingWeb.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShoppingWeb.Services;

public class AuthService : IAuthService
{
    private readonly ShoppingWebContext _context;
    private readonly ILogger<AuthService> _logger;
    private JwtSettings _jwtSettings;

    public AuthService(ShoppingWebContext context, IOptions<JwtSettings> jwtSettings, ILogger<AuthService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
    }

    public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerRequest)
    {
        try
        {
            if (await _context.Users.AnyAsync(u => u.Email.Equals(registerRequest.Email)))
            {
                throw new UserAlreadyExistException("Email already exist ");
            }

            if (await _context.Users.AnyAsync(u => u.Email.Equals(registerRequest.Phone)))
            {
                throw new UserAlreadyExistException("Email already exist ");
            }

            var passwordHash = HashPassword(registerRequest.PasswordHash);

            var user = new User
            {
                Username = registerRequest.Username,
                PasswordHash = passwordHash,
                Email = registerRequest.Email,
                FullName = registerRequest.FullName,
                Phone = registerRequest.Phone,
                RoleId = 1,
                CreatedAt = DateTime.UtcNow,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            return new AuthResponseDTO()
            {
                Token = token,
                RefreshToken = refreshToken,
                Expires = DateTime.Now.AddHours(_jwtSettings.ExpiryMinutes),
                User = new UserInfoDTO
                {
                    UserID = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    RoleID = user.RoleId
                }
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while registering user: {Message}", e.Message);
            throw;
        }
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginRequest)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(loginRequest.Email));
            if (user == null || !VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            return new AuthResponseDTO()
            {
                Token = token,
                RefreshToken = refreshToken,
                Expires = DateTime.Now.AddHours(_jwtSettings.ExpiryMinutes),
                User = new UserInfoDTO
                {
                    UserID = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    RoleID = user.RoleId
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            throw;
        }
    }

    public async Task ForgotPasswordAsync(string email)
    {
        try
        {
            _logger.LogInformation("Forgot password request for email: {Email}", email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            if (user != null)
            {
                var resetToken = GenerateRefreshToken();
                var resetTokenExpiry = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryMinutes);

                user.PasswordHash = HashPassword(resetToken); // Store the reset token as a hashed password
                user.UpdatedAt = DateTime.UtcNow;

                // generate a reset password token and send it via email
                // This is a placeholder for the actual implementation of sending an email.
                return;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<AuthResponseDTO> RefreshTokenAsync(string token, string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task LogoutAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RevokeTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {

        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}