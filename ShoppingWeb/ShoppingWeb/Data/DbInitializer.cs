using ShoppingWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoppingWeb.Data
{
    public static class DbInitializer
    {
        public static void SeedData(ShoppingWebContext context)
        {
            // Đảm bảo database được tạo
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
                    },
                    new Product 
                    { 
                        ProductName = "MacBook Pro 16", 
                        Description = "Professional laptop for creative work", 
                        Price = 2499.99m, 
                        StockQuantity = 15,
                        ImageUrl = "https://res.cloudinary.com/dkvndgyyl/image/upload/v1/shoppingweb/products/macbook_pro_16.jpg",
                        BrandId = brands.FirstOrDefault(b => b.BrandName == "Apple")?.BrandId,
                        CategoryId = categories.FirstOrDefault(c => c.CategoryName == "Electronics")?.CategoryId,
                        CreatedAt = DateTime.Now
                    }
                };
                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
} 