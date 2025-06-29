using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShoppingWeb.Config;
using ShoppingWeb.DTOs.Auth;
using ShoppingWeb.Enum;
using ShoppingWeb.Exceptions;
using ShoppingWeb.Helpers;
using ShoppingWeb.Mapping;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;
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
    private IEmailService _emailService;


    public AuthService(ShoppingWebContext context, IOptions<JwtSettings> jwtSettings, ILogger<AuthService> logger, IEmailService emailService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerRequest)
    {

        try
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerRequest.Email))
            {
                throw new UserAlreadyExistException("Email already exists.");
            }

            if (await _context.Users.AnyAsync(u => u.Phone == registerRequest.Phone))
            {
                throw new UserAlreadyExistException("Phone number already exists.");
            }

            var passwordHash = PasswordHelper.HashPassword(registerRequest.Password);
            var user = new User
            {
                Username = registerRequest.Username,
                PasswordHash = passwordHash,
                Email = registerRequest.Email,
                FullName = registerRequest.FullName,
                Phone = registerRequest.Phone,
                RoleId = (int)UserRole.CUSTOMER, // Use enum instead of hardcoded value
                CreatedAt = DateTime.UtcNow,
            };

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            // var userToken = new UserToken
            // {
            //     UserId = user.UserId,
            //     AccessToken = token,
            //     RefreshToken = refreshToken,
            //     CreatedAt = DateTime.UtcNow,
            //     IssuedAt = DateTime.UtcNow,
            //     ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            // };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Users.AddAsync(user);
                // await _context.UserTokens.AddAsync(userToken);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            _logger.LogInformation("User registered successfully: {Email}", registerRequest.Email);
            return new AuthResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = UserMapper.toResponseDto(user),
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while registering user: {Email}", registerRequest.Email);
            throw;
        }
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginRequest)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(loginRequest.Email));
            if (user == null || !PasswordHelper.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            var userToken = new UserToken
            {
                UserId = user.UserId,
                AccessToken = token,
                RefreshToken = refreshToken,
                CreatedAt = DateTime.Now,
                IssuedAt = DateTime.Now,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.ExpiryMinutes)
            };

            _context.UserTokens.Update(userToken);
            await _context.SaveChangesAsync();
            return new AuthResponseDTO()
            {
                Token = token,
                RefreshToken = refreshToken,
                Expires = DateTime.Now.AddHours(_jwtSettings.ExpiryMinutes),
                User = UserMapper.toResponseDto(user),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            throw;
        }
    }

    // public async Task ForgotPasswordAsync(string email) // hehe
    // {
    //     try
    //     {
    //         _logger.LogInformation("Forgot password request for email: {Email}", email);
    //         var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
    //
    //         if (user != null)
    //         {
    //             var resetToken = GenerateRefreshToken();
    //             var resetTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
    //
    //             var passwordResetTokens = new PasswordResetToken
    //             {
    //                 UserId = user.UserId,
    //                 Token = PasswordHelper.HashPassword(resetToken),
    //                 ExpiresAt = resetTokenExpiry
    //             };
    //             await _context.PasswordResetTokens.AddAsync(passwordResetTokens);
    //             await _context.SaveChangesAsync();
    //
    //             await _emailService.SendPasswordResetEmailAsync(email, resetToken, null);
    //             await Task.Delay(1000); // Consistent delay
    //             return true; // Always return true to prevent enumeration
    //         }
    //         else
    //         {
    //             _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
    //
    //             await Task.Delay(1000);
    //             throw;
    //         }
    //         
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError(e,e.Message);
    //         throw;
    //     }
    // }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        // if (string.IsNullOrWhiteSpace(email))
        // {
        //     _logger.LogWarning("Invalid forgot password request: Email is null or empty.");
        //     await Task.Delay(1000); // Consistent delay to prevent timing attacks
        //     return true; // Return true to avoid enumeration
        // }

        try
        {
            _logger.LogInformation("Forgot password request for email: {Email}", email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                var resetToken = GenerateRefreshToken();
                var resetTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ResetTokenExpiryMinutes); // Separate setting
                var passwordResetToken = new PasswordResetToken
                {
                    UserId = user.UserId,
                    Token = resetToken, // Store raw token securely
                    ExpiresAt = resetTokenExpiry
                };

                await _context.PasswordResetTokens.AddAsync(passwordResetToken);
                await _context.SaveChangesAsync();
                await _emailService.SendPasswordResetEmailAsync(email, resetToken, null);
            }
            else
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
            }

            await Task.Delay(1000); // Consistent delay
            return true; // Always return true to prevent enumeration
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password for email: {Email}", email);
            await Task.Delay(1000); // Consistent delay even on error
            throw;
        }
    }

    public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var tokenInDb = await _context.UserTokens.FirstOrDefaultAsync(t =>
                t.RefreshToken == refreshToken
                && !t.IsRevoked
                && t.ExpiresAt > DateTime.Now);

            if (tokenInDb == null)
            {
                _logger.LogWarning("Invalid or expired refresh token.");
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == tokenInDb.UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found for refresh token.");
                throw new UnauthorizedAccessException("User not found.");
            }

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            tokenInDb.IsRevoked = true; // Revoke old token
            var newUserToken = new UserToken
            {
                UserId = user.UserId,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                CreatedAt = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
            };

            _context.UserTokens.Update(tokenInDb); // Update old token
            await _context.UserTokens.AddAsync(newUserToken); // Add new token
            await _context.SaveChangesAsync();
            _logger.LogInformation("Token refreshed successfully for user ID: {UserId}", user.UserId);

            return new AuthResponseDTO
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = UserMapper.toResponseDto(user),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token ");
            throw;
        }
    }

    public async Task LogoutAsync(string token)
    {
        try
        {
            var userToken = await _context.UserTokens.FirstOrDefaultAsync(t => t.AccessToken == token);
            if (userToken != null)
            {
                userToken.IsRevoked = true; // Mark token as revoked
                userToken.RevokedAt = DateTime.UtcNow;
                _context.UserTokens.Update(userToken);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User logged out successfully for token.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during logout.");
            throw;
        }
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {


        try
        {
            var userToken = await _context.UserTokens.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken && !t.IsRevoked);
            if (userToken == null)
            {
                _logger.LogWarning("Invalid or already revoked refresh token.");
                return false;
            }

            userToken.IsRevoked = true;
            _context.UserTokens.Update(userToken);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Refresh token revoked successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token.");
            throw;
        }
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    private string GenerateJwtToken(User user)
    {
        if (user == null)
        {
            _logger.LogError("Cannot generate JWT for null user.");
            throw new ArgumentNullException(nameof(user));
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, ((UserRole)user.RoleId).ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
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



    public async Task<bool> ResetPasswordAsync(string tokenResetPassword, string newPassword)
    {
        try
        {
            var tokenInDb = await _context.PasswordResetTokens.FirstOrDefaultAsync(t =>
                t.Token == tokenResetPassword && t.Used == false && t.ExpiresAt > DateTime.UtcNow);


            if (tokenInDb == null)
            {
                _logger.LogWarning("Invalid or expired password reset token.");
                return false;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == tokenInDb.UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found for reset token.");
                return false;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                user.PasswordHash = PasswordHelper.HashPassword(newPassword);
                tokenInDb.Used = true;
                _context.Users.Update(user);
                _context.PasswordResetTokens.Update(tokenInDb);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            _logger.LogInformation("Password reset successfully for user ID: {UserId}", user.UserId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error resetting password for token: {Token}", tokenResetPassword);
            throw;
        }

    }
}