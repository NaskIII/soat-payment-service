using Xunit;
using FluentAssertions;
using Domain.ValueObjects;

namespace Domain.Test
{
    public class NameValueObjectTests
    {
        [Fact]
        public void Constructor_ShouldCreateName_WhenInputIsValid()
        {
            var validName = "John Doe";

            var name = new Name(validName);

            name.Should().NotBeNull();
            name.Value.Should().Be(validName);
        }

        [Fact]
        public void Constructor_ShouldTrimWhitespace_WhenInputHasSpaces()
        {
            var input = "  Jane Doe  ";
            var expected = "Jane Doe";

            var name = new Name(input);

            name.Value.Should().Be(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrowArgumentException_WhenInputIsInvalid(string invalidInput)
        {
            var ex = Assert.Throws<ArgumentException>(() => new Name(invalidInput));

            ex.Message.Should().Contain("Name cannot be null or empty");
            ex.ParamName.Should().Be("name");
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenNamesAreSameCaseInsensitive()
        {
            var name1 = new Name("John Doe");
            var name2 = new Name("john doe");

            name1.Should().Be(name2);
            name1.Equals(name2).Should().BeTrue();
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenNamesAreDifferent()
        {
            var name1 = new Name("John");
            var name2 = new Name("Jane");

            name1.Should().NotBe(name2);
            name1.Equals(name2).Should().BeFalse();
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenComparingToNull()
        {
            var name = new Name("John");

            name.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_ShouldBeSame_ForEqualObjects()
        {
            var name1 = new Name("John");
            var name2 = new Name("John");

            name1.GetHashCode().Should().Be(name2.GetHashCode());
        }
    }
}