using Domain.Utils;
using FluentAssertions;
using Xunit;

namespace Domain.Test
{
    public class ServiceAccountHelperTests
    {
        [Fact]
        public void GenerateClientId_Should_Return_Valid_Guid_String_Format()
        {
            var clientId = ServiceAccountHelper.GenerateClientId();

            clientId.Should().NotBeNullOrEmpty();
            clientId.Should().HaveLength(32);
            Guid.TryParse(clientId, out _).Should().BeTrue();
        }

        [Fact]
        public void GenerateClientId_Should_Return_Unique_Values()
        {
            var clientId1 = ServiceAccountHelper.GenerateClientId();
            var clientId2 = ServiceAccountHelper.GenerateClientId();

            clientId1.Should().NotBe(clientId2);
        }

        [Fact]
        public void GenerateClientSecret_Should_Return_String_With_Default_Length()
        {
            var secret = ServiceAccountHelper.GenerateClientSecret();

            secret.Should().NotBeNullOrEmpty();
            secret.Should().HaveLength(32);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(64)]
        [InlineData(128)]
        public void GenerateClientSecret_Should_Return_String_With_Specified_Length(int length)
        {
            var secret = ServiceAccountHelper.GenerateClientSecret(length);

            secret.Should().HaveLength(length);
        }

        [Fact]
        public void GenerateClientSecret_Should_Contain_Only_Valid_Characters()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";

            var secret = ServiceAccountHelper.GenerateClientSecret(100);

            foreach (var c in secret)
            {
                validChars.Should().Contain(c.ToString());
            }
        }

        [Fact]
        public void GenerateClientSecret_Should_Return_Unique_Values()
        {
            var secret1 = ServiceAccountHelper.GenerateClientSecret();
            var secret2 = ServiceAccountHelper.GenerateClientSecret();

            secret1.Should().NotBe(secret2);
        }
    }
}