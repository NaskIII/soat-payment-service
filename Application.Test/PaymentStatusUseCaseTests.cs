using Application.UseCases.PaymentUseCase;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Test
{
    public class PaymentStatusUseCaseTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PaymentStatusUseCase _useCase;

        public PaymentStatusUseCaseTests()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _mapperMock = new Mock<IMapper>();
            _useCase = new PaymentStatusUseCase(_paymentRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Return_PaymentStatus_When_Payment_Exists()
        {
            var orderId = Guid.NewGuid();
            var expectedStatus = PaymentStatus.Completed;
            var payment = new Payment(orderId, 100.00m, expectedStatus);

            _paymentRepositoryMock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .ReturnsAsync(payment);

            var result = await _useCase.ExecuteAsync(orderId);

            result.Should().Be(expectedStatus);
            _paymentRepositoryMock.Verify(x => x.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Throw_ArgumentNullException_When_Payment_Does_Not_Exist()
        {
            var orderId = Guid.NewGuid();

            _paymentRepositoryMock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .ReturnsAsync((Payment?)null);

            Func<Task> act = async () => await _useCase.ExecuteAsync(orderId);

            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("*Order has no Payment Information*");

            _paymentRepositoryMock.Verify(x => x.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>()), Times.Once);
        }
    }
}