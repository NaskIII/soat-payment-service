using Domain.Enums;

namespace Domain.Entities
{
    public class Payment
    {

        public Guid PaymentId { get; private set; }
        public Guid OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaymentDate { get; private set; } = DateTime.UtcNow;
        public PaymentStatus PaymentStatus { get; private set; }
        public string? TransactionId { get; private set; }
        public string? QrCodeUri { get; private set; }

        public Payment(Guid orderId, decimal amount, PaymentStatus paymentStatus, string? transactionId = null, string? qrCodeUri = null)
        {
            OrderId = orderId;
            Amount = amount;
            PaymentStatus = paymentStatus;
            TransactionId = transactionId;
            QrCodeUri = qrCodeUri;
        }

        public void MarkAsCompleted()
        {
            PaymentStatus = PaymentStatus.Completed;
        }

        public void MarkAsFailed()
        {
            PaymentStatus = PaymentStatus.Failed;
        }

        public void MarkAsCancelled()
        {
            PaymentStatus = PaymentStatus.Cancelled;
        }
    }
}
