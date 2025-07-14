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
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var brands = context.Brands.ToList();
                var categories = context.Categories.ToList();

                var products = new List<Product>
                {
                    new Product
                    {
                        ProductName = "Nike Air Max 270",
                        Description = "Comfortable running shoes with Air Max technology",
                        Price = 129.99m,
                        StockQuantity = 50,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/nike_air_max_270.jpg",
                        BrandId = brands.FirstOrDefault(b => b.BrandName == "Nike")?.BrandId,
                        CategoryId = categories.FirstOrDefault(c => c.CategoryName == "Sports")?.CategoryId,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        ProductName = "Adidas Ultraboost 21",
                        Description = "Premium running shoes with Boost technology",
                        Price = 179.99m,
                        StockQuantity = 30,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/adidas_ultraboost_21.jpg",
                        BrandId = brands.FirstOrDefault(b => b.BrandName == "Adidas")?.BrandId,
                        CategoryId = categories.FirstOrDefault(c => c.CategoryName == "Sports")?.CategoryId,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        ProductName = "iPhone 14 Pro",
                        Description = "Latest iPhone with advanced camera system",
                        Price = 999.99m,
                        StockQuantity = 25,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/iphone_14_pro.jpg",
                        BrandId = brands.FirstOrDefault(b => b.BrandName == "Apple")?.BrandId,
                        CategoryId = categories.FirstOrDefault(c => c.CategoryName == "Electronics")?.CategoryId,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        ProductName = "Samsung Galaxy S23",
                        Description = "Premium Android smartphone",
                        Price = 899.99m,
                        StockQuantity = 35,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/samsung_galaxy_s23.jpg",
                        BrandId = brands.FirstOrDefault(b => b.BrandName == "Samsung")?.BrandId,
                        CategoryId = categories.FirstOrDefault(c => c.CategoryName == "Electronics")?.CategoryId,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        ProductName = "Uniqlo Cotton T-Shirt",
                        Description = "Comfortable cotton t-shirt for everyday wear",
                        Price = 19.99m,
                        StockQuantity = 100,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/uniqlo_cotton_tshirt.jpg",
                        BrandId = brands.FirstOrDefault(b => b.BrandName == "Uniqlo")?.BrandId,
                        CategoryId = categories.FirstOrDefault(c => c.CategoryName == "Clothing")?.CategoryId,
                        CreatedAt = DateTime.Now
                    }
                };
                context.Products.AddRange(products);
                context.SaveChanges();
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
                        ProvinceId = 1,
                        DistrictId = 1,
                        WardId = 1,
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
                        ProvinceId = 2,
                        DistrictId = 2,
                        WardId = 2,
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
                        ProvinceId = 3,
                        DistrictId = 3,
                        WardId = 3,
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
                        ProvinceId = 4,
                        DistrictId = 4,
                        WardId = 4,
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
                        ProvinceId = 5,
                        DistrictId = 5,
                        WardId = 5,
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
                        CartId = carts.FirstOrDefault(c => c.OrderCode == "ORD001")?.CartId ?? 1,
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Uniqlo Cotton T-Shirt")?.ProductId ?? 1,
                        Quantity = 2,
                        UnitPrice = 19.99m,
                        Status = "Completed",
                        CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    new OrderDetail
                    {
                        CartId = carts.FirstOrDefault(c => c.OrderCode == "ORD002")?.CartId ?? 2,
                        ProductId = products.FirstOrDefault(p => p.ProductName == "iPhone 14 Pro")?.ProductId ?? 2,
                        Quantity = 1,
                        UnitPrice = 999.99m,
                        Status = "Completed",
                        CreatedAt = DateTime.Now.AddDays(-2)
                    },
                    new OrderDetail
                    {
                        CartId = carts.FirstOrDefault(c => c.OrderCode == null && c.UserId == 3)?.CartId ?? 3,
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Uniqlo Cotton T-Shirt")?.ProductId ?? 1,
                        Quantity = 2,
                        UnitPrice = 19.99m,
                        Status = "Pending",
                        CreatedAt = DateTime.Now
                    },
                    new OrderDetail
                    {
                        CartId = carts.FirstOrDefault(c => c.OrderCode == "ORD003")?.CartId ?? 4,
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Adidas Ultraboost 21")?.ProductId ?? 3,
                        Quantity = 1,
                        UnitPrice = 179.99m,
                        Status = "Completed",
                        CreatedAt = DateTime.Now.AddDays(-3)
                    },
                    new OrderDetail
                    {
                        CartId = carts.FirstOrDefault(c => c.OrderCode == null && c.UserId == 5)?.CartId ?? 5,
                        ProductId = products.FirstOrDefault(p => p.ProductName == "Nike Air Max 270")?.ProductId ?? 4,
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
                if (nikeProduct != null && summerSale != null && nikeProduct.ProductId >= 1 && nikeProduct.ProductId <= 5)
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
                if (iphoneProduct != null && techDeals != null && iphoneProduct.ProductId >= 1 && iphoneProduct.ProductId <= 5)
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
                if (uniqloProduct != null && winterClearance != null && uniqloProduct.ProductId >= 1 && uniqloProduct.ProductId <= 5)
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
                if (adidasProduct != null && summerSale2 != null && adidasProduct.ProductId >= 1 && adidasProduct.ProductId <= 5)
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
        }
    }
}