using Xunit;
using FluentAssertions;
using Domain.ValueObjects;

namespace Domain.Test
{
    public class EmailValueObjectTests
    {
        [Fact]
        public void Constructor_ShouldCreateEmail_WhenAddressIsValid()
        {
            var validEmail = "test@example.com";

            var email = new Email(validEmail);

            email.Should().NotBeNull();
            email.Value.Should().Be(validEmail);
        }

        [Fact]
        public void Constructor_ShouldTrimWhitespace_WhenInputHasSpaces()
        {
            var emailWithSpaces = "  test@example.com  ";
            var expectedValue = "test@example.com";

            var email = new Email(emailWithSpaces);

            email.Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_ShouldThrowArgumentException_WhenInputIsNoOrEmpty(string invalidInput)
        {
            var ex = Assert.Throws<ArgumentException>(() => new Email(invalidInput));
            ex.Message.Should().Contain("Email cannot be null or empty");
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("#@%^%#$@#$@#.com")]
        [InlineData("@example.com")]
        [InlineData("Joe Smith <email@example.com>")]
        [InlineData("email.example.com")]
        [InlineData("email@example@example.com")]
        [InlineData("email@example")]
        public void Constructor_ShouldThrowArgumentException_WhenFormatIsInvalid(string invalidFormat)
        {
            var ex = Assert.Throws<ArgumentException>(() => new Email(invalidFormat));
            ex.Message.Should().Be("Invalid email format.");
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenEmailsAreSameCaseInsensitive()
        {
            var email1 = new Email("Test@Example.com");
            var email2 = new Email("test@example.com");

            email1.Should().Be(email2);
            email1.Equals(email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenEmailsAreDifferent()
        {
            var email1 = new Email("user1@example.com");
            var email2 = new Email("user2@example.com");

            email1.Should().NotBe(email2);
            email1.Equals(email2).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_ShouldBeSame_ForSameEmailDifferentCase()
        {
            var email1 = new Email("Test@Example.com");
            var email2 = new Email("test@example.com");

            email1.GetHashCode().Should().Be(email2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnEmailValue()
        {
            var address = "test@example.com";
            var email = new Email(address);

            email.ToString().Should().Be(address);
        }
    }
}