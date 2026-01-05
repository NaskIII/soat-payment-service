using Application.Dtos.WebhookDtos.Request;
using Application.Interfaces;
using Domain.BaseInterfaces;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.PaymentUseCase
{
    public class ProcessPaymentUseCase : IProcessPaymentUseCase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IUpdateOrderStatusGateway _updateOrderStatusGateway;

        private readonly ILogger _logger;

        public ProcessPaymentUseCase(
            IPaymentRepository paymentRepository,
            IPaymentGateway paymentGateway,
            IUpdateOrderStatusGateway updateOrderStatusGateway,
            ILogger<ProcessPaymentUseCase> logger)
        {
            _paymentRepository = paymentRepository;
            _paymentGateway = paymentGateway;
            _updateOrderStatusGateway = updateOrderStatusGateway;
            _logger = logger;
        }

        public async Task ExecuteAsync(MercadoPagoWebhookRequest request)
        {
            _logger.LogInformation("Received: {Request}", request.ToString());
            if (request?.Action != "payment.created" || string.IsNullOrEmpty(request.Data?.Id))
            {
                if (request?.Action != "payment.updated" || string.IsNullOrEmpty(request.Data?.Id))
                {
                    return;
                }
            }

            string externalPaymentId = request.Data.Id;
            _logger.LogInformation("Processing webhook for external payment ID: {ExternalPaymentId}", externalPaymentId);

            (PaymentStatus verifiedStatus, string orderId) = await _paymentGateway.GetPaymentStatusAsync(externalPaymentId);

            var payment = await _paymentRepository.GetSingleAsync(p => p.OrderId == new Guid(orderId));
            if (payment == null)
            {
                _logger.LogWarning("Payment for Order ID {OrderId} not found.", orderId);
                return;
            }

            if (payment.PaymentStatus != PaymentStatus.Pending)
            {
                _logger.LogInformation("Payment {PaymentId} is not pending. Status: {Status}. Ignoring webhook.", payment.PaymentId, payment.PaymentStatus);
                return;
            }

            if (verifiedStatus == PaymentStatus.Completed)
            {

                try
                {
                    payment.MarkAsCompleted();

                    await _paymentRepository.UpdateAsync(payment);

                    _logger.LogInformation("Payment {PaymentId} for Order {OrderId} was completed successfully.", payment.PaymentId, orderId);

                    await _updateOrderStatusGateway.UpdateOrderStatusAsync(new Guid(orderId), PaymentStatus.Completed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update order and payment status for OrderId {OrderId}", orderId);
                    throw;
                }
            }
            else if (verifiedStatus == PaymentStatus.Failed)
            {
                payment.MarkAsFailed();
                await _paymentRepository.UpdateAsync(payment);

                await _updateOrderStatusGateway.UpdateOrderStatusAsync(new Guid(orderId), PaymentStatus.Failed);

                _logger.LogWarning("Payment {PaymentId} for Order {OrderId} failed.", payment.PaymentId, payment.OrderId);
            }
            else if (verifiedStatus == PaymentStatus.Refunded)
            {
                payment.MarkAsFailed();
                await _paymentRepository.UpdateAsync(payment);

                await _updateOrderStatusGateway.UpdateOrderStatusAsync(new Guid(orderId), PaymentStatus.Refunded);

                _logger.LogWarning("Payment {PaymentId} for Order {OrderId} refunded.", payment.PaymentId, payment.OrderId);
            }
            else if (verifiedStatus == PaymentStatus.Cancelled)
            {
                payment.MarkAsCancelled();
                await _paymentRepository.UpdateAsync(payment);

                await _updateOrderStatusGateway.UpdateOrderStatusAsync(new Guid(orderId), PaymentStatus.Cancelled);

                _logger.LogWarning("Payment {PaymentId} for Order {OrderId} cancelled.", payment.PaymentId, payment.OrderId);
            }
        }
    }
}
