using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Models;

namespace AuthService.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context, IPasswordService passwordService)
        {
            // Admin bilgilerini environment'tan al
            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                Console.WriteLine("⚠️ ADMIN_EMAIL veya ADMIN_PASSWORD env değişkeni ayarlanmamış.");
                return;
            }

            // Zaten varsa ekleme
            if (!context.Users.Any(u => u.Email == adminEmail))
            {
                var hashedPassword = passwordService.HashPassword(adminPassword);

                context.Users.Add(new User
                {
                    Email = adminEmail,
                    PasswordHash = hashedPassword,
                    Role = "Admin"
                });

                context.SaveChanges();
                Console.WriteLine($"✅ Admin user created: {adminEmail}");
            }
            else
            {
                Console.WriteLine("ℹ️ Admin user already exists, seeding skipped.");
            }
        }
    }
}
