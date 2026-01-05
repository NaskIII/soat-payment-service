using Application.Dtos.CheckoutDtos.Request;
using Application.Dtos.MercadoPagoDtos.Request;
using Application.Exceptions;
using Application.Interfaces;
using Domain.BaseInterfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.CheckoutUseCases
{
    public class CheckoutUseCase : ICheckoutUseCase
    {

        private readonly IPaymentGateway _paymentGateway;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<CheckoutUseCase> _logger;

        public CheckoutUseCase(
            IPaymentGateway paymentGateway,
            IPaymentRepository paymentRepository,
            ILogger<CheckoutUseCase> logger)
        {
            _paymentGateway = paymentGateway;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<(Guid, string)> ExecuteAsync(PaymentRequestDto request)
        {
            (string paymentId, string qrCode)= await _paymentGateway.GeneratePaymentQrCodeAsync(request);

            Payment payment = new Payment(request.external_reference, request.total_amount, PaymentStatus.Pending, paymentId, qrCode);

            await _paymentRepository.BeginTransactionAsync();

            try
            {
                await _paymentRepository.AddAsync(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar pagamento para o pedido {OrderId}", request.external_reference);
                await _paymentRepository.RollbackAsync();
                throw new ApplicationException("Erro ao processar pagamento. Tente novamente mais tarde.");
            }

            await _paymentRepository.CommitTransactionAsync();

            return (payment.OrderId,  qrCode);
        }
    }
}
