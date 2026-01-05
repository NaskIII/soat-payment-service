using Domain.Enums;

namespace Application.Interfaces
{
    public interface IPaymentStatusUseCase : IUseCase<Guid, PaymentStatus>
    {
    }
}
