using Application.Dtos.WebhookDtos.Request;
using Application.Interfaces;
using Application.UseCases.PaymentUseCase;
using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Test
{
    public class ProcessPaymentUseCaseTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IPaymentGateway> _paymentGatewayMock;
        private readonly Mock<IUpdateOrderStatusGateway> _updateOrderStatusGatewayMock;
        private readonly Mock<ILogger<ProcessPaymentUseCase>> _loggerMock;
        private readonly ProcessPaymentUseCase _useCase;

        public ProcessPaymentUseCaseTests()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _paymentGatewayMock = new Mock<IPaymentGateway>();
            _updateOrderStatusGatewayMock = new Mock<IUpdateOrderStatusGateway>();
            _loggerMock = new Mock<ILogger<ProcessPaymentUseCase>>();

            _useCase = new ProcessPaymentUseCase(
                _paymentRepositoryMock.Object,
                _paymentGatewayMock.Object,
                _updateOrderStatusGatewayMock.Object,
                _loggerMock.Object
            );
        }

        private MercadoPagoWebhookRequest CreateRequest(string action, string? id)
        {
            return new MercadoPagoWebhookRequest
            {
                Action = action,
                Data = new WebhookData { Id = id }
            };
        }

        [Theory]
        [InlineData("payment.created", null)]
        [InlineData("payment.created", "")]
        [InlineData("payment.updated", null)]
        [InlineData("invalid.action", "123")]
        public async Task ExecuteAsync_Should_Ignore_Request_When_Invalid_Action_Or_Id(string action, string? id)
        {
            var request = CreateRequest(action, id);

            await _useCase.ExecuteAsync(request);

            _paymentGatewayMock.Verify(x => x.GetPaymentStatusAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Log_Warning_And_Return_When_Payment_Not_Found()
        {
            // Arrange
            var externalId = "12345";
            var orderId = Guid.NewGuid();
            var request = CreateRequest("payment.created", externalId);

            _paymentGatewayMock.Setup(x => x.GetPaymentStatusAsync(externalId))
                .ReturnsAsync((PaymentStatus.Completed, orderId.ToString()));

            _paymentRepositoryMock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .ReturnsAsync((Payment?)null);

            await _useCase.ExecuteAsync(request);

            _paymentRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Payment>()), Times.Never);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Payment for Order ID {orderId} not found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Ignore_When_Payment_Is_Already_Processed()
        {
            var externalId = "12345";
            var orderId = Guid.NewGuid();
            var request = CreateRequest("payment.updated", externalId);

            var existingPayment = new Payment(orderId, 100m, PaymentStatus.Completed);

            _paymentGatewayMock.Setup(x => x.GetPaymentStatusAsync(externalId))
                .ReturnsAsync((PaymentStatus.Completed, orderId.ToString()));

            _paymentRepositoryMock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .ReturnsAsync(existingPayment);

            await _useCase.ExecuteAsync(request);

            _paymentRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Payment>()), Times.Never);
            _updateOrderStatusGatewayMock.Verify(x => x.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<OrderStatus>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Update_To_Received_When_Status_Is_Verified_Completed()
        {
            var externalId = "12345";
            var orderId = Guid.NewGuid();
            var request = CreateRequest("payment.created", externalId);

            var existingPayment = new Payment(orderId, 100m, PaymentStatus.Pending);

            _paymentGatewayMock.Setup(x => x.GetPaymentStatusAsync(externalId))
                .ReturnsAsync((PaymentStatus.Completed, orderId.ToString()));

            _paymentRepositoryMock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .ReturnsAsync(existingPayment);

            await _useCase.ExecuteAsync(request);

            existingPayment.PaymentStatus.Should().Be(PaymentStatus.Completed);

            _paymentRepositoryMock.Verify(x => x.UpdateAsync(existingPayment), Times.Once);
            _updateOrderStatusGatewayMock.Verify(x => x.UpdateOrderStatusAsync(orderId, OrderStatus.Received), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Update_To_Failed_When_Status_Is_Verified_Failed()
        {
            var externalId = "12345";
            var orderId = Guid.NewGuid();
            var request = CreateRequest("payment.created", externalId);

            var existingPayment = new Payment(orderId, 100m, PaymentStatus.Pending);

            _paymentGatewayMock.Setup(x => x.GetPaymentStatusAsync(externalId))
                .ReturnsAsync((PaymentStatus.Failed, orderId.ToString()));

            _paymentRepositoryMock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .ReturnsAsync(existingPayment);

            await _useCase.ExecuteAsync(request);

            existingPayment.PaymentStatus.Should().Be(PaymentStatus.Failed);

            _paymentRepositoryMock.Verify(x => x.UpdateAsync(existingPayment), Times.Once);
            _updateOrderStatusGatewayMock.Verify(x => x.UpdateOrderStatusAsync(orderId, OrderStatus.Canceled), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Mark_As_Failed_But_Notify_Refunded_When_Status_Is_Refunded()
        {
            var externalId = "12345";
            var orderId = Guid.NewGuid();
            var request = CreateRequest("payment.updated", externalId);

            var existingPayment = new Payment(orderId, 100m, PaymentStatus.Pending);

            _paymentGatewayMock.Setup(x => x.GetPaymentStatusAsync(externalId))
                .ReturnsAsync((PaymentStatus.Refunded, orderId.ToString()));

            _paymentRepositoryMock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .ReturnsAsync(existingPayment);

            await _useCase.ExecuteAsync(request);

            existingPayment.PaymentStatus.Should().Be(PaymentStatus.Failed);

            _paymentRepositoryMock.Verify(x => x.UpdateAsync(existingPayment), Times.Once);

            _updateOrderStatusGatewayMock.Verify(x => x.UpdateOrderStatusAsync(orderId, OrderStatus.Canceled), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Throw_Exception_And_Log_Error_On_Database_Failure()
        {
            var externalId = "12345";
            var orderId = Guid.NewGuid();
            var request = CreateRequest("payment.created", externalId);
            var existingPayment = new Payment(orderId, 100m, PaymentStatus.Pending);

            _paymentGatewayMock.Setup(x => x.GetPaymentStatusAsync(externalId))
                .ReturnsAsync((PaymentStatus.Completed, orderId.ToString()));

            _paymentRepositoryMock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .ReturnsAsync(existingPayment);

            var dbException = new Exception("DB Error");
            _paymentRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Payment>()))
                .ThrowsAsync(dbException);

            Func<Task> act = async () => await _useCase.ExecuteAsync(request);

            await act.Should().ThrowAsync<Exception>().WithMessage("DB Error");

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    dbException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}