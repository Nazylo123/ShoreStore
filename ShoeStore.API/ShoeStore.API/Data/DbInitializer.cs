using Microsoft.EntityFrameworkCore;
using ShoeStore.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace ShoeStore.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // context.Database.EnsureCreated(); // Comment this out

            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Sneakers", Description = "Casual and athletic shoes" },
                    new Category { Name = "Formal Shoes", Description = "Business and formal wear" },
                    new Category { Name = "Boots", Description = "Winter and outdoor boots" },
                    new Category { Name = "Sandals", Description = "Summer and casual sandals" },
                    new Category { Name = "Sports", Description = "Athletic and sports shoes" }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // Seed Brands
            if (!context.Brands.Any())
            {
                var brands = new List<Brand>
                {
                    new Brand { Name = "Nike", Description = "Just Do It" },
                    new Brand { Name = "Adidas", Description = "Impossible is Nothing" },
                    new Brand { Name = "Puma", Description = "Forever Faster" },
                    new Brand { Name = "Converse", Description = "Classic Style" },
                    new Brand { Name = "Vans", Description = "Off The Wall" },
                    new Brand { Name = "New Balance", Description = "Fearlessly Independent" },
                    new Brand { Name = "Reebok", Description = "Be More Human" },
                    new Brand { Name = "Under Armour", Description = "The Only Way Is Through" }
                };
                context.Brands.AddRange(brands);
                context.SaveChanges();
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Nike Air Max 270",
                        Description = "Comfortable running shoes with Air Max technology",
                        Price = 150.00m,
                        StockQuantity = 50,
                        CategoryId = context.Categories.First(c => c.Name == "Sneakers").Id,
                        BrandId = context.Brands.First(b => b.Name == "Nike").Id,
                        ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400"
                    },
                    new Product
                    {
                        Name = "Adidas Ultraboost 22",
                        Description = "Premium running shoes with Boost technology",
                        Price = 180.00m,
                        StockQuantity = 30,
                        CategoryId = context.Categories.First(c => c.Name == "Sports").Id,
                        BrandId = context.Brands.First(b => b.Name == "Adidas").Id,
                        ImageUrl = "https://images.unsplash.com/photo-1608231387042-66d1773070a5?w=400"
                    },
                    new Product
                    {
                        Name = "Converse Chuck Taylor",
                        Description = "Classic canvas sneakers",
                        Price = 65.00m,
                        StockQuantity = 100,
                        CategoryId = context.Categories.First(c => c.Name == "Sneakers").Id,
                        BrandId = context.Brands.First(b => b.Name == "Converse").Id,
                        ImageUrl = "https://images.unsplash.com/photo-1607522370275-f14206abe5d3?w=400"
                    },
                    new Product
                    {
                        Name = "Vans Old Skool",
                        Description = "Iconic skate shoes with side stripe",
                        Price = 60.00m,
                        StockQuantity = 75,
                        CategoryId = context.Categories.First(c => c.Name == "Sneakers").Id,
                        BrandId = context.Brands.First(b => b.Name == "Vans").Id,
                        ImageUrl = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?w=400"
                    },
                    new Product
                    {
                        Name = "New Balance 574",
                        Description = "Classic lifestyle sneakers",
                        Price = 80.00m,
                        StockQuantity = 60,
                        CategoryId = context.Categories.First(c => c.Name == "Sneakers").Id,
                        BrandId = context.Brands.First(b => b.Name == "New Balance").Id,
                        ImageUrl = "https://images.unsplash.com/photo-1556906781-9a412961c28c?w=400"
                    }
                };
                context.Products.AddRange(products);
                context.SaveChanges();
            }

            // Seed Admin User
            if (!context.UserAccounts.Any(u => u.Username == "admin"))
            {
                var adminUser = new UserAccount
                {
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"),
                    Email = "admin@shoestore.com",
                    Role = "Admin"
                };
                context.UserAccounts.Add(adminUser);
                context.SaveChanges();
            }

            // Seed Sample Customer
            if (!context.UserAccounts.Any(u => u.Username == "customer"))
            {
                var customer = new Customer
                {
                    Name = "John Doe",
                    Phone = "0123456789",
                    Address = "123 Main St, City",
                    Email = "customer@example.com"
                };
                context.Customers.Add(customer);
                context.SaveChanges();

                var customerUser = new UserAccount
                {
                    Username = "customer",
                    PasswordHash = HashPassword("customer123"),
                    Email = "customer@example.com",
                    Role = "Customer",
                    CustomerId = customer.Id
                };
                context.UserAccounts.Add(customerUser);
                context.SaveChanges();
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
} 