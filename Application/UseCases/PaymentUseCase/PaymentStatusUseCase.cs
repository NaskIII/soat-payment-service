using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryInterfaces;

namespace Application.UseCases.PaymentUseCase
{
    public class PaymentStatusUseCase : IPaymentStatusUseCase
    {

        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public PaymentStatusUseCase(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<PaymentStatus> ExecuteAsync(Guid request)
        {
            Payment? payment = await _paymentRepository.GetSingleAsync(x => x.OrderId == request);
            if (payment == null)
            {
                throw new ArgumentNullException("Order has no Payment Information");
            }

            return payment.PaymentStatus;
        }
    }
}
