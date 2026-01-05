using Application.Dtos.WebhookDtos.Request;

namespace Application.Interfaces
{
    public interface IProcessPaymentUseCase : IUseCase<MercadoPagoWebhookRequest>
    {
    }
}
