using Application.Dtos.CheckoutDtos.Request;
using Application.Dtos.MercadoPagoDtos.Request;
using Application.UseCases.CheckoutUseCases;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace Application.Test
{
    public class CheckoutUseCaseTests
    {
        private readonly Mock<IPaymentGateway> _paymentGatewayMock;
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<ILogger<CheckoutUseCase>> _loggerMock;
        private readonly CheckoutUseCase _checkoutUseCase;

        public CheckoutUseCaseTests()
        {
            _paymentGatewayMock = new Mock<IPaymentGateway>();
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _loggerMock = new Mock<ILogger<CheckoutUseCase>>();

            _checkoutUseCase = new CheckoutUseCase(
                _paymentGatewayMock.Object,
                _paymentRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_Should_Create_Payment_And_Commit_Transaction_When_Successful()
        {
            var orderId = Guid.NewGuid();
            var totalAmount = 150.00m;

            var items = new List<MercadoPagoItem>
            {
                new MercadoPagoItem
                {
                    Title = "Item Teste",
                    Quantity = 1,
                    UnitPrice = totalAmount,
                    TotalAmount = totalAmount
                }
            };

            var request = new PaymentRequestDto(
                orderId,
                "Pedido de Teste",
                totalAmount,
                "https://callback.url",
                items
            );

            var generatedPaymentId = "123456789";
            var generatedQrCode = "000201010212...";

            _paymentGatewayMock.Setup(x => x.GeneratePaymentQrCodeAsync(request))
                .ReturnsAsync((generatedPaymentId, generatedQrCode));

            var result = await _checkoutUseCase.ExecuteAsync(request);

            result.Item1.Should().Be(orderId);
            result.Item2.Should().Be(generatedQrCode);

            _paymentGatewayMock.Verify(x => x.GeneratePaymentQrCodeAsync(request), Times.Once);

            _paymentRepositoryMock.Verify(x => x.BeginTransactionAsync(), Times.Once);

            _paymentRepositoryMock.Verify(x => x.AddAsync(It.Is<Payment>(p =>
                p.OrderId == orderId &&
                p.Amount == totalAmount &&
                p.PaymentStatus == PaymentStatus.Pending &&
                p.TransactionId == generatedPaymentId &&
                p.QrCodeUri == generatedQrCode
            )), Times.Once);

            _paymentRepositoryMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
            _paymentRepositoryMock.Verify(x => x.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Rollback_And_Throw_Exception_When_Repository_Fails()
        {
            var orderId = Guid.NewGuid();
            var totalAmount = 100.00m;
            var items = new List<MercadoPagoItem>();

            var request = new PaymentRequestDto(
                orderId,
                "Pedido Fail",
                totalAmount,
                null,
                items
            );

            _paymentGatewayMock.Setup(x => x.GeneratePaymentQrCodeAsync(request))
                .ReturnsAsync(("123", "qr_code"));

            var dbException = new Exception("Database connection failed");
            _paymentRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Payment>()))
                .ThrowsAsync(dbException);

            Func<Task> act = async () => await _checkoutUseCase.ExecuteAsync(request);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("Erro ao processar pagamento. Tente novamente mais tarde.");

            _paymentRepositoryMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _paymentRepositoryMock.Verify(x => x.RollbackAsync(), Times.Once);
            _paymentRepositoryMock.Verify(x => x.CommitTransactionAsync(), Times.Never);

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