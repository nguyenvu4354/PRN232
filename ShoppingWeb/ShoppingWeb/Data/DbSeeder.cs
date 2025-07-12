using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Data;
using ShoppingWeb.Enum;
using ShoppingWeb.Models;

public class DbSeeder
{
    private readonly ShoppingWebContext _context;

    public DbSeeder(ShoppingWebContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        Console.WriteLine("👉 Starting seed...");

        try
        {
            // Đảm bảo database có thể kết nối
            await _context.Database.CanConnectAsync();

            var userExist = await _context.Users.AnyAsync(u => u.Email == "admin@example.com");

            if (!userExist)
            {
                Console.WriteLine("🛠️ Seeding initial data...");

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("1234567");

                var users = new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        Email = "admin@example.com",
                        FullName = "Admin User",
                        Phone = "1234567890",
                        Address = "123 Admin Street",
                        PasswordHash = hashedPassword,
                        RoleId = (int)UserRole.ADMIN
                    },
                    new User
                    {
                        Username = "user",
                        Email = "user@example.com",
                        FullName = "User",
                        Phone = "123456709",
                        PasswordHash = hashedPassword,
                        RoleId = (int)UserRole.CUSTOMER
                    },
                    new User
                    {
                        Username = "staff",
                        Email = "staff@example.com",
                        FullName = "Staff",
                        Phone = "123456709",
                        PasswordHash = hashedPassword,
                        RoleId = (int)UserRole.STAFF
                    },
                    new User
                    {
                        Username = "owner",
                        Email = "owner@example.com",
                        FullName = "Owner",
                        Phone = "123456709",
                        PasswordHash = hashedPassword,
                        RoleId = (int)UserRole.OWNER
                    }
                };

                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();

                Console.WriteLine("✅ Seeding completed!");
            }
            else
            {
                Console.WriteLine("⏩ Admin user already exists. Skipping seeding.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in seeding: {ex.Message}");
            throw;
        }
    }
}