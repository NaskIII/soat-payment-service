using Xunit;
using FluentAssertions;
using Domain.ValueObjects;

namespace Domain.Test
{
    public class CpfValueObjectTests
    {
        [Fact]
        public void Constructor_ShouldCreateCpf_WhenNumberIsValidAndClean()
        {
            var validCpfNumber = "52998224725";

            var cpf = new Cpf(validCpfNumber);

            cpf.Should().NotBeNull();
            cpf.Value.Should().Be(validCpfNumber);
        }

        [Fact]
        public void Constructor_ShouldCleanString_WhenInputHasFormatting()
        {
            var formattedCpf = "529.982.247-25";
            var expectedCleanValue = "52998224725";

            var cpf = new Cpf(formattedCpf);

            cpf.Value.Should().Be(expectedCleanValue);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_ShouldThrowArgumentException_WhenInputIsNoOrEmpty(string invalidInput)
        {
            var ex = Assert.Throws<ArgumentException>(() => new Cpf(invalidInput));
            ex.Message.Should().Contain("CPF cannot be null or empty");
        }

        [Theory]
        [InlineData("123")]
        [InlineData("12345678901234")]
        [InlineData("00000000000")]
        [InlineData("11111111111")]
        [InlineData("12345678900")]
        [InlineData("52998224700")]
        public void Constructor_ShouldThrowArgumentException_WhenCpfIsInvalid(string invalidCpf)
        {
            var ex = Assert.Throws<ArgumentException>(() => new Cpf(invalidCpf));
            ex.Message.Should().Be("Invalid CPF.");
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenValuesAreSame()
        {
            var cpf1 = new Cpf("529.982.247-25");
            var cpf2 = new Cpf("52998224725");

            cpf1.Should().Be(cpf2);
            cpf1.Equals(cpf2).Should().BeTrue();
            (cpf1 == cpf2).Should().BeFalse();
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenValuesAreDifferent()
        {
            var cpf1 = new Cpf("52998224725");
            var cpf2 = new Cpf("51471627802");

            cpf1.Should().NotBe(cpf2);
            cpf1.Equals(cpf2).Should().BeFalse();
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenComparingToNullOrOtherType()
        {
            var cpf = new Cpf("52998224725");

            cpf.Equals(null).Should().BeFalse();
            cpf.Equals(new object()).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_ShouldBeSame_ForSameValues()
        {
            var cpf1 = new Cpf("52998224725");
            var cpf2 = new Cpf("52998224725");

            cpf1.GetHashCode().Should().Be(cpf2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnFormattedCpf()
        {
            var rawCpf = "52998224725";
            var cpf = new Cpf(rawCpf);

            var result = cpf.ToString();

            result.Should().Be("529.982.247-25");
        }
    }
}