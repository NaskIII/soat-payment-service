using Application.Dtos.CheckoutDtos.Request;
using Application.Dtos.MercadoPagoDtos.Request;

namespace Application.Interfaces
{
    public interface ICheckoutUseCase : IUseCase<PaymentRequestDto, (Guid, string)>
    {
    }
}
