using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.Test
{
    public class ServiceAccountsTests
    {
        [Fact]
        public void Should_Create_ServiceAccount_With_Generated_Id_And_Current_Date()
        {
            // Arrange
            var serviceName = new Name("PaymentService");
            var clientId = Guid.NewGuid();
            var secretHash = "hashed_secret_123";

            // Act
            var serviceAccount = new ServiceAccounts(serviceName, clientId, secretHash);

            // Assert
            serviceAccount.Id.Should().NotBeEmpty();
            serviceAccount.ServiceName.Should().Be(serviceName);
            serviceAccount.ClientId.Should().Be(clientId);
            serviceAccount.ClientSecretHash.Should().Be(secretHash);
            serviceAccount.IsActive.Should().BeTrue();
            serviceAccount.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Should_Create_ServiceAccount_With_Specific_Values()
        {
            // Arrange
            var id = Guid.NewGuid();
            var serviceName = new Name("AuthService");
            var clientId = Guid.NewGuid();
            var secretHash = "hashed_secret_456";
            var createdAt = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);

            // Act
            var serviceAccount = new ServiceAccounts(id, serviceName, clientId, secretHash, createdAt);

            // Assert
            serviceAccount.Id.Should().Be(id);
            serviceAccount.ServiceName.Should().Be(serviceName);
            serviceAccount.ClientId.Should().Be(clientId);
            serviceAccount.ClientSecretHash.Should().Be(secretHash);
            serviceAccount.IsActive.Should().BeTrue();
            serviceAccount.CreatedAt.Should().Be(createdAt);
        }

        [Fact]
        public void Should_Deactivate_ServiceAccount()
        {
            // Arrange
            var serviceAccount = new ServiceAccounts(new Name("LogService"), Guid.NewGuid(), "hash");

            // Act
            serviceAccount.Deactivate();

            // Assert
            serviceAccount.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Should_Rotate_Client_Secret()
        {
            // Arrange
            var oldHash = "old_hash";
            var newHash = "new_secure_hash_789";
            var serviceAccount = new ServiceAccounts(new Name("NotificationService"), Guid.NewGuid(), oldHash);

            // Act
            serviceAccount.RotateSecret(newHash);

            // Assert
            serviceAccount.ClientSecretHash.Should().Be(newHash);
            serviceAccount.ClientSecretHash.Should().NotBe(oldHash);
        }
    }
}