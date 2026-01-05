using Domain.Entities;
using Domain.ValueObjects;
using Infraestructure.Security;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infraestructure.DatabaseConfiguration
{
    public static class ModelBuilderConfiguration
    {

        public static void SeedDatabase(this ModelBuilder modelBuilder)
        {

            BCryptPasswordHasher passwordHasher = new BCryptPasswordHasher();
            string hashedSecretOne = passwordHasher.HashPassword("SuperSecret");

            ServiceAccounts serviceAccount = new ServiceAccounts(
                Guid.Parse("911C65EA-54F6-4310-A739-82914326E53C"),
                new Name("OrderService"),
                Guid.Parse("17036AA9-79D8-4FB7-9218-741D316C003F"),
                "$2a$11$6mcP.1X3HO9WzBv/boKC2uJOGhR6UlbyGtrTeS8sZoAJEr7vOqzjq",
                new DateTime(2026, 1, 4, 0, 0, 0, DateTimeKind.Utc)
            );

            modelBuilder.Entity<ServiceAccounts>().HasData(serviceAccount);
        }
    }
}
