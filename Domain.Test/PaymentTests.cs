using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Domain.Test
{
    public class PaymentTests
    {
        [Fact]
        public void Should_Create_Payment_With_Correct_Values()
        {
            var orderId = Guid.NewGuid();
            var amount = 150.00m;
            var status = PaymentStatus.Pending;
            var transactionId = "TX-123456";
            var qrCodeUri = "https://qrcode.com/pay";

            var payment = new Payment(orderId, amount, status, transactionId, qrCodeUri);

            payment.PaymentId.Should().BeEmpty();
            payment.OrderId.Should().Be(orderId);
            payment.Amount.Should().Be(amount);
            payment.PaymentStatus.Should().Be(status);
            payment.TransactionId.Should().Be(transactionId);
            payment.QrCodeUri.Should().Be(qrCodeUri);
            payment.PaymentDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Should_Create_Payment_With_Default_Values()
        {
            var orderId = Guid.NewGuid();
            var amount = 50.00m;
            var status = PaymentStatus.Pending;

            var payment = new Payment(orderId, amount, status);

            payment.TransactionId.Should().BeNull();
            payment.QrCodeUri.Should().BeNull();
        }

        [Fact]
        public void Should_Mark_Payment_As_Completed()
        {
            var payment = new Payment(Guid.NewGuid(), 100.00m, PaymentStatus.Pending);

            payment.MarkAsCompleted();

            payment.PaymentStatus.Should().Be(PaymentStatus.Completed);
        }

        [Fact]
        public void Should_Mark_Payment_As_Failed()
        {
            var payment = new Payment(Guid.NewGuid(), 100.00m, PaymentStatus.Pending);

            payment.MarkAsFailed();

            payment.PaymentStatus.Should().Be(PaymentStatus.Failed);
        }

        [Fact]
        public void Should_Mark_Payment_As_Cancelled()
        {
            var payment = new Payment(Guid.NewGuid(), 100.00m, PaymentStatus.Pending);

            payment.MarkAsCancelled();

            payment.PaymentStatus.Should().Be(PaymentStatus.Cancelled);
        }
    }
}