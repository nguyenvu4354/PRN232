using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Data;
using ShoppingWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            await _context.Database.EnsureCreatedAsync();

            // Roles
            if (!await _context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { RoleName = "ADMIN" },
                    new Role { RoleName = "CUSTOMER" },
                    new Role { RoleName = "STAFF" },
                    new Role { RoleName = "OWNER" }
                };
                await _context.Roles.AddRangeAsync(roles);
                await _context.SaveChangesAsync();
            }

            // Provinces
            if (!await _context.Provinces.AnyAsync())
            {
                var provinces = new List<Province>
                {
                    new Province { Id = 1, Name = "Hà Nội" },
                    new Province { Id = 2, Name = "Hồ Chí Minh" }
                };
                await _context.Provinces.AddRangeAsync(provinces);
                await _context.SaveChangesAsync();
            }

            // Districts
            if (!await _context.Districts.AnyAsync())
            {
                var districts = new List<District>
                {
                    new District { Id = 1, Name = "Cầu Giấy", ProvinceId = 1 },
                    new District { Id = 2, Name = "Thanh Xuân", ProvinceId = 1 },
                    new District { Id = 3, Name = "Quận 1", ProvinceId = 2 }
                };
                await _context.Districts.AddRangeAsync(districts);
                await _context.SaveChangesAsync();
            }

            // Wards
            if (!await _context.Wards.AnyAsync())
            {
                var wards = new List<Ward>
                {
                    new Ward { Id = 1, Name = "Dịch Vọng", DistrictId = 1 },
                    new Ward { Id = 2, Name = "Quan Hoa", DistrictId = 1 },
                    new Ward { Id = 3, Name = "Bến Nghé", DistrictId = 3 }
                };
                await _context.Wards.AddRangeAsync(wards);
                await _context.SaveChangesAsync();
            }

            // Users
            if (!await _context.Users.AnyAsync())
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("1234567");
                var users = new List<User>();
                for (int i = 1; i <= 5; i++)
                {
                    users.Add(new User
                    {
                        Username = $"user{i}",
                        Email = $"user{i}@example.com",
                        FullName = $"User {i}",
                        Phone = $"01234567{i}",
                        Address = $"Số {i} - Hà Nội",
                        PasswordHash = hashedPassword,
                        RoleId = 2,
                        ProvinceId = 1,
                        DistrictId = 1,
                        WardId = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();
            }

            // Brands
            if (!await _context.Brands.AnyAsync())
            {
                var brands = new List<Brand>
                {
                    new Brand { BrandName = "Nike", Description = "Sportswear and athletic shoes" },
                    new Brand { BrandName = "Adidas", Description = "Sportswear and athletic equipment" },
                    new Brand { BrandName = "Apple", Description = "Electronics and technology" },
                    new Brand { BrandName = "Samsung", Description = "Electronics and mobile devices" },
                    new Brand { BrandName = "Uniqlo", Description = "Casual wear and fashion" }
                };
                await _context.Brands.AddRangeAsync(brands);
                await _context.SaveChangesAsync();
            }

            // Categories
            if (!await _context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { CategoryName = "Electronics", Description = "Electronic devices and gadgets" },
                    new Category { CategoryName = "Clothing", Description = "Fashion and apparel" },
                    new Category { CategoryName = "Sports", Description = "Sports equipment and gear" },
                    new Category { CategoryName = "Home & Garden", Description = "Home improvement and garden supplies" },
                    new Category { CategoryName = "Books", Description = "Books and educational materials" }
                };
                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync();
            }

            // Products
            if (!await _context.Products.AnyAsync())
            {
                var brands = await _context.Brands.ToListAsync();
                var categories = await _context.Categories.ToListAsync();

                var products = new List<Product>
                {
                    new Product
                    {
                        ProductName = "Nike Air Max 270",
                        Description = "Comfortable running shoes with Air Max technology",
                        Price = 129.99m,
                        StockQuantity = 50,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/nike_air_max_270.jpg",
                        BrandId = brands.First(b => b.BrandName == "Nike").BrandId,
                        CategoryId = categories.First(c => c.CategoryName == "Sports").CategoryId,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        ProductName = "Adidas Ultraboost 21",
                        Description = "Premium running shoes with Boost technology",
                        Price = 179.99m,
                        StockQuantity = 30,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/adidas_ultraboost_21.jpg",
                        BrandId = brands.First(b => b.BrandName == "Adidas").BrandId,
                        CategoryId = categories.First(c => c.CategoryName == "Sports").CategoryId,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        ProductName = "iPhone 14 Pro",
                        Description = "Latest iPhone with advanced camera system",
                        Price = 999.99m,
                        StockQuantity = 25,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/iphone_14_pro.jpg",
                        BrandId = brands.First(b => b.BrandName == "Apple").BrandId,
                        CategoryId = categories.First(c => c.CategoryName == "Electronics").CategoryId,
                        CreatedAt = DateTime.Now
                    }
                };
                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
            }

            // Carts & OrderDetails
            var product1 = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == "Nike Air Max 270");
            var product2 = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == "Adidas Ultraboost 21");
            var product3 = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == "iPhone 14 Pro");

            if (product1 == null || product2 == null || product3 == null)
            {
                Console.WriteLine("⚠️ Missing required products for OrderDetail.");
                return;
            }

            if (!await _context.Carts.AnyAsync(c => c.IsCart == false))
            {
                var carts = new List<Cart>();
                var orderDetails = new List<OrderDetail>();

                for (int i = 1; i <= 5; i++)
                {
                    var cart = new Cart
                    {
                        UserId = i,
                        IsCart = false,
                        OrderDate = DateTime.UtcNow.AddDays(-i),
                        TotalAmount = (product1.Price * 1) + (product2.Price * 2) + (product3.Price * 1),
                        ShippingAddress = $"123 Street {i}",
                        ProvinceId = 1,
                        DistrictId = 1,
                        WardId = 1,
                        PaymentCode = $"PAY000{i}",
                        OrderCode = $"ORD000{i}",
                        CreatedAt = DateTime.UtcNow.AddDays(-i),
                        UpdatedAt = DateTime.UtcNow.AddDays(-i)
                    };
                    carts.Add(cart);
                }

                await _context.Carts.AddRangeAsync(carts);
                await _context.SaveChangesAsync();

                foreach (var cart in carts)
                {
                    orderDetails.AddRange(new List<OrderDetail>
                    {
                        new OrderDetail
                        {
                            CartId = cart.CartId,
                            ProductId = product1.ProductId,
                            Quantity = 1,
                            UnitPrice = product1.Price,
                            Status = "Paid",
                            CreatedAt = cart.CreatedAt
                        },
                        new OrderDetail
                        {
                            CartId = cart.CartId,
                            ProductId = product2.ProductId,
                            Quantity = 2,
                            UnitPrice = product2.Price,
                            Status = "Paid",
                            CreatedAt = cart.CreatedAt
                        },
                        new OrderDetail
                        {
                            CartId = cart.CartId,
                            ProductId = product3.ProductId,
                            Quantity = 1,
                            UnitPrice = product3.Price,
                            Status = "Paid",
                            CreatedAt = cart.CreatedAt
                        }
                    });
                }

                await _context.OrderDetails.AddRangeAsync(orderDetails);
                await _context.SaveChangesAsync();
            }

            Console.WriteLine("✅ Seeding completed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in seeding: {ex.Message}");
            throw;
        }
    }
}
