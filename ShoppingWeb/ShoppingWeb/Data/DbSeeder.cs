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
            Console.WriteLine("Database created successfully.");

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
                try
                {
                    await _context.Roles.AddRangeAsync(roles);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Roles seeded:");
                    foreach (var role in await _context.Roles.ToListAsync())
                    {
                        Console.WriteLine($"ID: {role.RoleId}, Name: {role.RoleName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding Roles: {ex.Message}");
                    throw;
                }
            }

            // Provinces
            //if (!await _context.Provinces.AnyAsync())
            //{
            //    var provinces = new List<Province>
            //    {
            //        new Province { Id = 1, Name = "Hà Nội" },
            //        new Province { Id = 2, Name = "Hồ Chí Minh" }
            //    };
            //    try
            //    {
            //        await _context.Provinces.AddRangeAsync(provinces);
            //        await _context.SaveChangesAsync();
            //        Console.WriteLine("Provinces seeded:");
            //        foreach (var province in await _context.Provinces.ToListAsync())
            //        {
            //            Console.WriteLine($"ID: {province.Id}, Name: {province.Name}");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error seeding Provinces: {ex.Message}");
            //        throw;
            //    }
            //}

            //// Districts
            //if (!await _context.Districts.AnyAsync())
            //{
            //    var districts = new List<District>
            //    {
            //        new District { Id = 1, Name = "Cầu Giấy", ProvinceId = 1 },
            //        new District { Id = 2, Name = "Thanh Xuân", ProvinceId = 1 },
            //        new District { Id = 3, Name = "Quận 1", ProvinceId = 2 }
            //    };
            //    try
            //    {
            //        await _context.Districts.AddRangeAsync(districts);
            //        await _context.SaveChangesAsync();
            //        Console.WriteLine("Districts seeded:");
            //        foreach (var district in await _context.Districts.ToListAsync())
            //        {
            //            Console.WriteLine($"ID: {district.Id}, Name: {district.Name}");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error seeding Districts: {ex.Message}");
            //        throw;
            //    }
            //}

            //// Wards
            //if (!await _context.Wards.AnyAsync())
            //{
            //    var wards = new List<Ward>
            //    {
            //        new Ward { Id = 1, Name = "Dịch Vọng", DistrictId = 1 },
            //        new Ward { Id = 2, Name = "Quan Hoa", DistrictId = 1 },
            //        new Ward { Id = 3, Name = "Bến Nghé", DistrictId = 3 }
            //    };
            //    try
            //    {
            //        await _context.Wards.AddRangeAsync(wards);
            //        await _context.SaveChangesAsync();
            //        Console.WriteLine("Wards seeded:");
            //        foreach (var ward in await _context.Wards.ToListAsync())
            //        {
            //            Console.WriteLine($"ID: {ward.Id}, Name: {ward.Name}");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error seeding Wards: {ex.Message}");
            //        throw;
            //    }
            //}

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
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                try
                {
                    await _context.Users.AddRangeAsync(users);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Users seeded:");
                    foreach (var user in await _context.Users.ToListAsync())
                    {
                        Console.WriteLine($"ID: {user.UserId}, Username: {user.Username}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding Users: {ex.Message}");
                    throw;
                }
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
                try
                {
                    await _context.Brands.AddRangeAsync(brands);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Brands seeded:");
                    foreach (var brand in await _context.Brands.ToListAsync())
                    {
                        Console.WriteLine($"ID: {brand.BrandId}, Name: {brand.BrandName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding Brands: {ex.Message}");
                    throw;
                }
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
                try
                {
                    await _context.Categories.AddRangeAsync(categories);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Categories seeded:");
                    foreach (var category in await _context.Categories.ToListAsync())
                    {
                        Console.WriteLine($"ID: {category.CategoryId}, Name: {category.CategoryName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding Categories: {ex.Message}");
                    throw;
                }
            }

            // Products
            if (!await _context.Products.AnyAsync())
            {
                var brands = await _context.Brands.ToListAsync();
                var categories = await _context.Categories.ToListAsync();

                if (!brands.Any() || !categories.Any())
                {
                    throw new Exception("Cannot seed Products: Brands or Categories table is empty.");
                }

                var products = new List<Product>
                {
                    new Product
                    {
                        ProductName = "Nike Air Max 270",
                        Description = "Comfortable running shoes with Air Max technology",
                        Price = 129.99m,
                        StockQuantity = 50,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/nike_air_max_270.jpg",
                        BrandId = brands.FirstOrDefault(b => b.BrandName == "Nike")?.BrandId
                            ?? throw new Exception("Brand 'Nike' not found."),
                        CategoryId = categories.FirstOrDefault(c => c.CategoryName == "Sports")?.CategoryId
                            ?? throw new Exception("Category 'Sports' not found."),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        ProductName = "Adidas Ultraboost 21",
                        Description = "Premium running shoes with Boost technology",
                        Price = 179.99m,
                        StockQuantity = 30,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/adidas_ultraboost_21.jpg",
                        BrandId = brands.FirstOrDefault(b => b.BrandName == "Adidas")?.BrandId
                            ?? throw new Exception("Brand 'Adidas' not found."),
                        CategoryId = categories.FirstOrDefault(c => c.CategoryName == "Sports")?.CategoryId
                            ?? throw new Exception("Category 'Sports' not found."),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        ProductName = "iPhone 14 Pro",
                        Description = "Latest iPhone with advanced camera system",
                        Price = 999.99m,
                        StockQuantity = 25,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/iphone_14_pro.jpg",
                        BrandId = brands.FirstOrDefault(b => b.BrandName == "Apple")?.BrandId
                            ?? throw new Exception("Brand 'Apple' not found."),
                        CategoryId = categories.FirstOrDefault(c => c.CategoryName == "Electronics")?.CategoryId
                            ?? throw new Exception("Category 'Electronics' not found."),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        ProductName = "Samsung Galaxy S23",
                        Description = "Premium Android smartphone",
                        Price = 899.99m,
                        StockQuantity = 35,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/samsung_galaxy_s23.jpg",
                        BrandId = brands.FirstOrDefault(b => b.BrandName == "Samsung")?.BrandId
                            ?? throw new Exception("Brand 'Samsung' not found."),
                        CategoryId = categories.FirstOrDefault(c => c.CategoryName == "Electronics")?.CategoryId
                            ?? throw new Exception("Category 'Electronics' not found."),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        ProductName = "Uniqlo Cotton T-Shirt",
                        Description = "Comfortable cotton t-shirt for everyday wear",
                        Price = 19.99m,
                        StockQuantity = 100,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/uniqlo_cotton_tshirt.jpg",
                        BrandId = brands.FirstOrDefault(b => b.BrandName == "Uniqlo")?.BrandId
                            ?? throw new Exception("Brand 'Uniqlo' not found."),
                        CategoryId = categories.FirstOrDefault(c => c.CategoryName == "Clothing")?.CategoryId
                            ?? throw new Exception("Category 'Clothing' not found."),
                        CreatedAt = DateTime.UtcNow
                    }
                };
                try
                {
                    await _context.Products.AddRangeAsync(products);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Products seeded:");
                    foreach (var product in await _context.Products.ToListAsync())
                    {
                        Console.WriteLine($"ID: {product.ProductId}, Name: {product.ProductName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding Products: {ex.Message}");
                    throw;
                }
            }

            // Carts & OrderDetails
            var product1 = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == "Nike Air Max 270");
            var product2 = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == "Adidas Ultraboost 21");
            var product3 = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == "iPhone 14 Pro");
            var product4 = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == "Uniqlo Cotton T-Shirt");

            if (product1 == null || product2 == null || product3 == null || product4 == null)
            {
                Console.WriteLine("⚠️ Missing required products for OrderDetail.");
                foreach (var p in await _context.Products.ToListAsync())
                {
                    Console.WriteLine($"Available product: {p.ProductName}");
                }
                throw new Exception("Required products not found.");
            }

            if (!await _context.Carts.AnyAsync())
            {
                var carts = new List<Cart>
                {
                    new Cart
                    {
                        UserId = 1,
                        OrderDate = DateTime.UtcNow.AddDays(-1),
                        TotalAmount = 149.98m,
                        IsCart = false,
                        ShippingAddress = "123 Main St, Cầu Giấy, Hà Nội",
                        PaymentCode = "PAY001",
                        OrderCode = "ORD001",
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Cart
                    {
                        UserId = 2,
                        OrderDate = DateTime.UtcNow.AddDays(-2),
                        TotalAmount = 999.99m,
                        IsCart = false,
                        ShippingAddress = "456 Elm St, Thanh Xuân, Hà Nội",
                        PaymentCode = "PAY002",
                        OrderCode = "ORD002",
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Cart
                    {
                        UserId = 3,
                        OrderDate = null,
                        TotalAmount = 39.98m,
                        IsCart = true,
                        ShippingAddress = "789 Oak St, Cầu Giấy, Hà Nội",
                        PaymentCode = null,
                        OrderCode = null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Cart
                    {
                        UserId = 4,
                        OrderDate = DateTime.UtcNow.AddDays(-3),
                        TotalAmount = 179.99m,
                        IsCart = false,
                        ShippingAddress = "101 Pine St, Quận 1, Hồ Chí Minh",
                        PaymentCode = "PAY003",
                        OrderCode = "ORD003",
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Cart
                    {
                        UserId = 5,
                        OrderDate = null,
                        TotalAmount = 0m,
                        IsCart = true,
                        ShippingAddress = "202 Cedar St, Cầu Giấy, Hà Nội",
                        PaymentCode = null,
                        OrderCode = null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };
                try
                {
                    await _context.Carts.AddRangeAsync(carts);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Carts seeded:");
                    foreach (var cart in await _context.Carts.ToListAsync())
                    {
                        Console.WriteLine($"ID: {cart.CartId}, UserId: {cart.UserId}, OrderCode: {cart.OrderCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding Carts: {ex.Message}");
                    throw;
                }

                var orderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        CartId = (await _context.Carts.FirstOrDefaultAsync(c => c.OrderCode == "ORD001"))?.CartId
                            ?? throw new Exception("Cart 'ORD001' not found."),
                        ProductId = product4.ProductId,
                        Quantity = 2,
                        UnitPrice = 19.99m,
                        Status = "Completed",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new OrderDetail
                    {
                        CartId = (await _context.Carts.FirstOrDefaultAsync(c => c.OrderCode == "ORD002"))?.CartId
                            ?? throw new Exception("Cart 'ORD002' not found."),
                        ProductId = product3.ProductId,
                        Quantity = 1,
                        UnitPrice = 999.99m,
                        Status = "Completed",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new OrderDetail
                    {
                        CartId = (await _context.Carts.FirstOrDefaultAsync(c => c.OrderCode == null && c.UserId == 3))?.CartId
                            ?? throw new Exception("Cart for UserId 3 not found."),
                        ProductId = product4.ProductId,
                        Quantity = 2,
                        UnitPrice = 19.99m,
                        Status = "Pending",
                        CreatedAt = DateTime.UtcNow
                    },
                    new OrderDetail
                    {
                        CartId = (await _context.Carts.FirstOrDefaultAsync(c => c.OrderCode == "ORD003"))?.CartId
                            ?? throw new Exception("Cart 'ORD003' not found."),
                        ProductId = product2.ProductId,
                        Quantity = 1,
                        UnitPrice = 179.99m,
                        Status = "Completed",
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new OrderDetail
                    {
                        CartId = (await _context.Carts.FirstOrDefaultAsync(c => c.OrderCode == null && c.UserId == 5))?.CartId
                            ?? throw new Exception("Cart for UserId 5 not found."),
                        ProductId = product1.ProductId,
                        Quantity = 1,
                        UnitPrice = 129.99m,
                        Status = "Pending",
                        CreatedAt = DateTime.UtcNow
                    }
                };
                try
                {
                    await _context.OrderDetails.AddRangeAsync(orderDetails);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("OrderDetails seeded:");
                    foreach (var orderDetail in await _context.OrderDetails.ToListAsync())
                    {
                        Console.WriteLine($"CartId: {orderDetail.CartId}, ProductId: {orderDetail.ProductId}, Quantity: {orderDetail.Quantity}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding OrderDetails: {ex.Message}");
                    throw;
                }
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