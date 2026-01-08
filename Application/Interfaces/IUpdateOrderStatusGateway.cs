using Domain.Enums;

namespace Application.Interfaces
{
    public interface IUpdateOrderStatusGateway
    {

        public Task UpdateOrderStatusAsync(Guid orderId, OrderStatus payment);
    }
}
