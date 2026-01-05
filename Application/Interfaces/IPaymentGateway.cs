using Application.Dtos.MercadoPagoDtos.Request;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IPaymentGateway
    {
        Task<(string, string)> GeneratePaymentQrCodeAsync(PaymentRequestDto paymentRequest);
        Task<(PaymentStatus, string)> GetPaymentStatusAsync(string externalPaymentId);
    }
}
