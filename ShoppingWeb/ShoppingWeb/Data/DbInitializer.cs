using ShoppingWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingWeb.Data
{
    public static class DbInitializer
    {
        public static void SeedData(ShoppingWebContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed Roles
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { RoleName = "ADMIN" },
                    new Role { RoleName = "CUSTOMER" },
                    new Role { RoleName = "STAFF" },
                    new Role { RoleName = "OWNER" }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
                Console.WriteLine("Roles seeded:");
                foreach (var role in context.Roles.ToList())
                {
                    Console.WriteLine($"ID: {role.RoleId}, Name: {role.RoleName}");
                }
            }

            //// Seed Provinces
            //if (!context.Provinces.Any())
            //{
            //    var provinces = new List<Province>
            //    {
            //        new Province { Id = 1, Name = "Hà Nội" },
            //        new Province { Id = 2, Name = "Hồ Chí Minh" }
            //    };
            //    context.Provinces.AddRange(provinces);
            //    context.SaveChanges();
            //    Console.WriteLine("Provinces seeded:");
            //    foreach (var province in context.Provinces.ToList())
            //    {
            //        Console.WriteLine($"ID: {province.Id}, Name: {province.Name}");
            //    }
            //}

            //// Seed Districts
            //if (!context.Districts.Any())
            //{
            //    var districts = new List<District>
            //    {
            //        new District { Id = 1, Name = "Cầu Giấy", ProvinceId = 1 },
            //        new District { Id = 2, Name = "Thanh Xuân", ProvinceId = 1 },
            //        new District { Id = 3, Name = "Quận 1", ProvinceId = 2 }
            //    };
            //    context.Districts.AddRange(districts);
            //    context.SaveChanges();
            //    Console.WriteLine("Districts seeded:");
            //    foreach (var district in context.Districts.ToList())
            //    {
            //        Console.WriteLine($"ID: {district.Id}, Name: {district.Name}");
            //    }
            //}

            //// Seed Wards
            //if (!context.Wards.Any())
            //{
            //    var wards = new List<Ward>
            //    {
            //        new Ward { Id = 1, Name = "Dịch Vọng", DistrictId = 1 },
            //        new Ward { Id = 2, Name = "Quan Hoa", DistrictId = 1 },
            //        new Ward { Id = 3, Name = "Bến Nghé", DistrictId = 3 }
            //    };
            //    context.Wards.AddRange(wards);
            //    context.SaveChanges();
            //    Console.WriteLine("Wards seeded:");
            //    foreach (var ward in context.Wards.ToList())
            //    {
            //        Console.WriteLine($"ID: {ward.Id}, Name: {ward.Name}");
            //    }
            //}

            // Seed Users
            if (!context.Users.Any())
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("1234567");
                var users = new List<User>
                {
                    new User
                    {
                        Username = "user1",
                        Email = "user1@example.com",
                        FullName = "User 1",
                        Phone = "012345671",
                        Address = "Số 1 - Hà Nội",
                        PasswordHash = hashedPassword,
                        RoleId = 2,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new User
                    {
                        Username = "user2",
                        Email = "user2@example.com",
                        FullName = "User 2",
                        Phone = "012345672",
                        Address = "Số 2 - Hà Nội",
                        PasswordHash = hashedPassword,
                        RoleId = 2,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new User
                    {
                        Username = "user3",
                        Email = "user3@example.com",
                        FullName = "User 3",
                        Phone = "012345673",
                        Address = "Số 3 - Hà Nội",
                        PasswordHash = hashedPassword,
                        RoleId = 2,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new User
                    {
                        Username = "user4",
                        Email = "user4@example.com",
                        FullName = "User 4",
                        Phone = "012345674",
                        Address = "Số 4 - Hà Nội",
                        PasswordHash = hashedPassword,
                        RoleId = 2,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new User
                    {
                        Username = "user5",
                        Email = "user5@example.com",
                        FullName = "User 5",
                        Phone = "012345675",
                        Address = "Số 5 - Hà Nội",
                        PasswordHash = hashedPassword,
                        RoleId = 2,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
                Console.WriteLine("Users seeded:");
                foreach (var user in context.Users.ToList())
                {
                    Console.WriteLine($"ID: {user.UserId}, Username: {user.Username}");
                }
            }

            // Seed Brands
            if (!context.Brands.Any())
            {
                var brands = new List<Brand>
                {
                    new Brand { BrandName = "Nike", Description = "Sportswear and athletic shoes" },
                    new Brand { BrandName = "Adidas", Description = "Sportswear and athletic equipment" },
                    new Brand { BrandName = "Apple", Description = "Electronics and technology" },
                    new Brand { BrandName = "Samsung", Description = "Electronics and mobile devices" },
                    new Brand { BrandName = "Uniqlo", Description = "Casual wear and fashion" }
                };
                context.Brands.AddRange(brands);
                context.SaveChanges();
                Console.WriteLine("Brands seeded:");
                foreach (var brand in context.Brands.ToList())
                {
                    Console.WriteLine($"ID: {brand.BrandId}, Name: {brand.BrandName}");
                }
            }

            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { CategoryName = "Electronics", Description = "Electronic devices and gadgets" },
                    new Category { CategoryName = "Clothing", Description = "Fashion and apparel" },
                    new Category { CategoryName = "Sports", Description = "Sports equipment and gear" },
                    new Category { CategoryName = "Home & Garden", Description = "Home improvement and garden supplies" },
                    new Category { CategoryName = "Books", Description = "Books and educational materials" }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
                Console.WriteLine("Categories seeded:");
                foreach (var category in context.Categories.ToList())
                {
                    Console.WriteLine($"ID: {category.CategoryId}, Name: {category.CategoryName}");
                }
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var brands = context.Brands.ToList();
                var categories = context.Categories.ToList();

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
                        CreatedAt = DateTime.Now
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
                        CreatedAt = DateTime.Now
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
                        CreatedAt = DateTime.Now
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
                        CreatedAt = DateTime.Now
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
                        CreatedAt = DateTime.Now
                    }
                };
                context.Products.AddRange(products);
                context.SaveChanges();
                Console.WriteLine("Products seeded:");
                foreach (var product in context.Products.ToList())
                {
                    Console.WriteLine($"ID: {product.ProductId}, Name: {product.ProductName}");
                }
            }

            // Seed Carts
            if (!context.Carts.Any())
            {
                var carts = new List<Cart>
                {
                    new Cart
                    {
                        UserId = 1,
                        OrderDate = DateTime.Now.AddDays(-1),
                        TotalAmount = 149.98m,
                        IsCart = false,
                        ShippingAddress = "123 Main St, District 1, Ho Chi Minh City",
                        PaymentCode = "PAY001",
                        OrderCode = "ORD001",
                        CreatedAt = DateTime.Now.AddDays(-1),
                        UpdatedAt = DateTime.Now
                    },
                    new Cart
                    {
                        UserId = 2,
                        OrderDate = DateTime.Now.AddDays(-2),
                        TotalAmount = 999.99m,
                        IsCart = false,
                        ShippingAddress = "456 Elm St, District 3, Hanoi",
                        PaymentCode = "PAY002",
                        OrderCode = "ORD002",
                        CreatedAt = DateTime.Now.AddDays(-2),
                        UpdatedAt = DateTime.Now
                    },
                    new Cart
                    {
                        UserId = 3,
                        OrderDate = null,
                        TotalAmount = 39.98m,
                        IsCart = true,
                        ShippingAddress = "789 Oak St, District 7, Da Nang",
                        PaymentCode = null,
                        OrderCode = null,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Cart
                    {
                        UserId = 4,
                        OrderDate = DateTime.Now.AddDays(-3),
                        TotalAmount = 179.99m,
                        IsCart = false,
                        ShippingAddress = "101 Pine St, District 5, Can Tho",
                        PaymentCode = "PAY003",
                        OrderCode = "ORD003",
                        CreatedAt = DateTime.Now.AddDays(-3),
                        UpdatedAt = DateTime.Now
                    },
                    new Cart
                    {
                        UserId = 5,
                        OrderDate = null,
                        TotalAmount = 0m,
                        IsCart = true,
                        ShippingAddress = "202 Cedar St, District 9, Hai Phong",
                        PaymentCode = null,
                        OrderCode = null,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };
                context.Carts.AddRange(carts);
                context.SaveChanges();
            }

            // Seed OrderDetails
            if (!context.OrderDetails.Any())
            {
                var carts = context.Carts.ToList();
                var products = context.Products.ToList();

                var orderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        CartId = carts.FirstOrDefault(c => c.OrderCode == "ORD001")?.CartId
                            ?? throw new Exception("Cart 'ORD001' not found."),
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Uniqlo Cotton T-Shirt")?.ProductId
                            ?? throw new Exception("Product 'Uniqlo Cotton T-Shirt' not found."),
                        Quantity = 2,
                        UnitPrice = 19.99m,
                        Status = "Completed",
                        CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    new OrderDetail
                    {
                        CartId = carts.FirstOrDefault(c => c.OrderCode == "ORD002")?.CartId
                            ?? throw new Exception("Cart 'ORD002' not found."),
                        ProductId = products.FirstOrDefault(p => p.ProductName == "iPhone 14 Pro")?.ProductId
                            ?? throw new Exception("Product 'iPhone 14 Pro' not found."),
                        Quantity = 1,
                        UnitPrice = 999.99m,
                        Status = "Completed",
                        CreatedAt = DateTime.Now.AddDays(-2)
                    },
                    new OrderDetail
                    {
                        CartId = carts.FirstOrDefault(c => c.OrderCode == null && c.UserId == 3)?.CartId
                            ?? throw new Exception("Cart for UserId 3 not found."),
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Uniqlo Cotton T-Shirt")?.ProductId
                            ?? throw new Exception("Product 'Uniqlo Cotton T-Shirt' not found."),
                        Quantity = 2,
                        UnitPrice = 19.99m,
                        Status = "Pending",
                        CreatedAt = DateTime.Now
                    },
                    new OrderDetail
                    {
                        CartId = carts.FirstOrDefault(c => c.OrderCode == "ORD003")?.CartId
                            ?? throw new Exception("Cart 'ORD003' not found."),
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Adidas Ultraboost 21")?.ProductId
                            ?? throw new Exception("Product 'Adidas Ultraboost 21' not found."),
                        Quantity = 1,
                        UnitPrice = 179.99m,
                        Status = "Completed",
                        CreatedAt = DateTime.Now.AddDays(-3)
                    },
                    new OrderDetail
                    {
                        CartId = carts.FirstOrDefault(c => c.OrderCode == null && c.UserId == 5)?.CartId
                            ?? throw new Exception("Cart for UserId 5 not found."),
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Nike Air Max 270")?.ProductId
                            ?? throw new Exception("Product 'Nike Air Max 270' not found."),
                        Quantity = 1,
                        UnitPrice = 129.99m,
                        Status = "Pending",
                        CreatedAt = DateTime.Now
                    }
                };
                context.OrderDetails.AddRange(orderDetails);
                context.SaveChanges();
            }

            // Seed Promotions
            if (!context.Promotions.Any())
            {
                var promotions = new List<Promotion>
                {
                    new Promotion
                    {
                        Title = "Summer Sale 2025",
                        Description = "Up to 20% off on sports and clothing items",
                        DiscountPercentage = 20m,
                        DiscountAmount = null,
                        StartDate = DateTime.Now.AddDays(-10),
                        EndDate = DateTime.Now.AddDays(10),
                        IsActive = true,
                        CreatedAt = DateTime.Now.AddDays(-10),
                        UpdatedAt = DateTime.Now
                    },
                    new Promotion
                    {
                        Title = "Tech Deals",
                        Description = "Flat $100 off on electronics over $500",
                        DiscountPercentage = null,
                        DiscountAmount = 100m,
                        StartDate = DateTime.Now.AddDays(-5),
                        EndDate = DateTime.Now.AddDays(15),
                        IsActive = true,
                        CreatedAt = DateTime.Now.AddDays(-5),
                        UpdatedAt = DateTime.Now
                    },
                    new Promotion
                    {
                        Title = "Back to School",
                        Description = "15% off on selected electronics",
                        DiscountPercentage = 15m,
                        DiscountAmount = null,
                        StartDate = DateTime.Now.AddDays(-2),
                        EndDate = DateTime.Now.AddDays(30),
                        IsActive = true,
                        CreatedAt = DateTime.Now.AddDays(-2),
                        UpdatedAt = DateTime.Now
                    },
                    new Promotion
                    {
                        Title = "Winter Clearance",
                        Description = "Clearance sale on clothing items",
                        DiscountPercentage = 30m,
                        DiscountAmount = null,
                        StartDate = DateTime.Now.AddDays(-15),
                        EndDate = DateTime.Now.AddDays(5),
                        IsActive = true,
                        CreatedAt = DateTime.Now.AddDays(-15),
                        UpdatedAt = DateTime.Now
                    },
                    new Promotion
                    {
                        Title = "Flash Sale",
                        Description = "Limited time 10% off storewide",
                        DiscountPercentage = 10m,
                        DiscountAmount = null,
                        StartDate = DateTime.Now.AddDays(-1),
                        EndDate = DateTime.Now.AddDays(1),
                        IsActive = true,
                        CreatedAt = DateTime.Now.AddDays(-1),
                        UpdatedAt = DateTime.Now
                    }
                };
                context.Promotions.AddRange(promotions);
                context.SaveChanges();
            }

            // Seed ProductPromotions
            if (!context.ProductPromotions.Any())
            {
                var products = context.Products.ToList();
                var promotions = context.Promotions.ToList();

                var productPromotions = new List<ProductPromotion>();

                // Nike Air Max 270 - Summer Sale 2025
                var nikeProduct = products.FirstOrDefault(p => p.ProductName == "Nike Air Max 270");
                var summerSale = promotions.FirstOrDefault(p => p.Title == "Summer Sale 2025");
                if (nikeProduct != null && summerSale != null)
                {
                    productPromotions.Add(new ProductPromotion
                    {
                        ProductId = nikeProduct.ProductId,
                        PromotionId = summerSale.PromotionId,
                        CreatedAt = DateTime.Now.AddDays(-10)
                    });
                }

                // iPhone 14 Pro - Tech Deals
                var iphoneProduct = products.FirstOrDefault(p => p.ProductName == "iPhone 14 Pro");
                var techDeals = promotions.FirstOrDefault(p => p.Title == "Tech Deals");
                if (iphoneProduct != null && techDeals != null)
                {
                    productPromotions.Add(new ProductPromotion
                    {
                        ProductId = iphoneProduct.ProductId,
                        PromotionId = techDeals.PromotionId,
                        CreatedAt = DateTime.Now.AddDays(-5)
                    });
                }

                // Uniqlo Cotton T-Shirt - Winter Clearance
                var uniqloProduct = products.FirstOrDefault(p => p.ProductName == "Uniqlo Cotton T-Shirt");
                var winterClearance = promotions.FirstOrDefault(p => p.Title == "Winter Clearance");
                if (uniqloProduct != null && winterClearance != null)
                {
                    productPromotions.Add(new ProductPromotion
                    {
                        ProductId = uniqloProduct.ProductId,
                        PromotionId = winterClearance.PromotionId,
                        CreatedAt = DateTime.Now.AddDays(-15)
                    });
                }

                // Adidas Ultraboost 21 - Summer Sale 2025
                var adidasProduct = products.FirstOrDefault(p => p.ProductName == "Adidas Ultraboost 21");
                var summerSale2 = promotions.FirstOrDefault(p => p.Title == "Summer Sale 2025");
                if (adidasProduct != null && summerSale2 != null)
                {
                    productPromotions.Add(new ProductPromotion
                    {
                        ProductId = adidasProduct.ProductId,
                        PromotionId = summerSale2.PromotionId,
                        CreatedAt = DateTime.Now.AddDays(-10)
                    });
                }

                context.ProductPromotions.AddRange(productPromotions);
                context.SaveChanges();
            }

            // Seed Feedback
            if (!context.Feedbacks.Any())
            {
                var feedback = new List<Feedback>
                {
                    new Feedback
                    {
                        UserId = 1,
                        Content = "Great experience with the Nike Air Max 270, very comfortable!",
                        Status = "Approved",
                        CreatedAt = DateTime.Now.AddDays(-5),
                        UpdatedAt = DateTime.Now
                    },
                    new Feedback
                    {
                        UserId = 2,
                        Content = "The iPhone 14 Pro has an amazing camera, but delivery took a bit long.",
                        Status = "Pending",
                        CreatedAt = DateTime.Now.AddDays(-3),
                        UpdatedAt = DateTime.Now
                    },
                    new Feedback
                    {
                        UserId = 3,
                        Content = "Uniqlo T-shirts are great value for money!",
                        Status = "Approved",
                        CreatedAt = DateTime.Now.AddDays(-2),
                        UpdatedAt = DateTime.Now
                    },
                    new Feedback
                    {
                        UserId = 4,
                        Content = "Adidas Ultraboost is fantastic for running, highly recommend.",
                        Status = "Approved",
                        CreatedAt = DateTime.Now.AddDays(-4),
                        UpdatedAt = DateTime.Now
                    },
                    new Feedback
                    {
                        UserId = 5,
                        Content = "Website is easy to use, but I wish there were more payment options.",
                        Status = "Pending",
                        CreatedAt = DateTime.Now.AddDays(-1),
                        UpdatedAt = DateTime.Now
                    }
                };
                context.Feedbacks.AddRange(feedback);
                context.SaveChanges();
            }

            // Seed ProductReviews
            if (!context.ProductReviews.Any())
            {
                var users = context.Users.ToList();
                var products = context.Products.ToList();

                if (!users.Any() || !products.Any())
                {
                    throw new Exception("Cannot seed ProductReviews: Users or Products table is empty.");
                }

                var productReviews = new List<ProductReview>
                {
                    new ProductReview
                    {
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Nike Air Max 270")?.ProductId
                            ?? throw new Exception("Product 'Nike Air Max 270' not found."),
                        UserId = users.FirstOrDefault(u => u.Username == "user1")?.UserId
                            ?? throw new Exception("User 'user1' not found."),
                        Rating = 5,
                        Comment = "Giày tuyệt vời, rất thoải mái khi sử dụng hàng ngày!",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new ProductReview
                    {
                        ProductId = products.FirstOrDefault(p => p.ProductName == "iPhone 14 Pro")?.ProductId
                            ?? throw new Exception("Product 'iPhone 14 Pro' not found."),
                        UserId = users.FirstOrDefault(u => u.Username == "user2")?.UserId
                            ?? throw new Exception("User 'user2' not found."),
                        Rating = 4,
                        Comment = "Điện thoại tuyệt, nhưng pin có thể cải thiện. Chất lượng camera đỉnh cao.",
                        CreatedAt = DateTime.Now.AddDays(-1),
                        UpdatedAt = DateTime.Now
                    },
                    new ProductReview
                    {
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Uniqlo Cotton T-Shirt")?.ProductId
                            ?? throw new Exception("Product 'Uniqlo Cotton T-Shirt' not found."),
                        UserId = users.FirstOrDefault(u => u.Username == "user3")?.UserId
                            ?? throw new Exception("User 'user3' not found."),
                        Rating = 5,
                        Comment = "Vải mềm và bền, giá trị tốt! Hoàn hảo cho mặc thường ngày.",
                        CreatedAt = DateTime.Now.AddDays(-2),
                        UpdatedAt = DateTime.Now
                    },
                    new ProductReview
                    {
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Adidas Ultraboost 21")?.ProductId
                            ?? throw new Exception("Product 'Adidas Ultraboost 21' not found."),
                        UserId = users.FirstOrDefault(u => u.Username == "user4")?.UserId
                            ?? throw new Exception("User 'user4' not found."),
                        Rating = 4,
                        Comment = "Tuyệt vời cho chạy bộ, nhưng hơi đắt. Công nghệ Boost tạo sự khác biệt.",
                        CreatedAt = DateTime.Now.AddDays(-3),
                        UpdatedAt = DateTime.Now
                    },
                    new ProductReview
                    {
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Samsung Galaxy S23")?.ProductId
                            ?? throw new Exception("Product 'Samsung Galaxy S23' not found."),
                        UserId = users.FirstOrDefault(u => u.Username == "user5")?.UserId
                            ?? throw new Exception("User 'user5' not found."),
                        Rating = 5,
                        Comment = "Điện thoại tuyệt vời, màn hình rất đẹp! Nhanh và đáng tin cậy.",
                        CreatedAt = DateTime.Now.AddDays(-4),
                        UpdatedAt = DateTime.Now
                    }
                };
                context.ProductReviews.AddRange(productReviews);
                context.SaveChanges();
            }
        }
    }
}