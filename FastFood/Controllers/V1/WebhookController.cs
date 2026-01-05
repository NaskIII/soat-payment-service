using Application.Dtos.WebhookDtos.Request;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers.V1
{
    [ApiController]
    [Route("api/v1/payments/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly IProcessPaymentUseCase _processPaymentUseCase;
        private readonly ILogger _logger;

        public WebhookController(IProcessPaymentUseCase processPaymentUseCase, ILogger<WebhookController> logger)
        {
            _processPaymentUseCase = processPaymentUseCase;
            _logger = logger;
        }

        [HttpPost("mercado-pago")]
        [AllowAnonymous]
        public async Task<IActionResult> MercadoPagoWebhook([FromBody] MercadoPagoWebhookRequest request)
        {
            if (request.Topic == "merchant_order")
            {
                _logger.LogInformation("Ignored merchant_order webhook: {Resource}", request.Resource);
                return Ok();
            }

            if (request.Type == "payment" || request.Action == "payment.updated" || request.Action == "payment.created")
            {
                _logger.LogInformation("Processing payment webhook. Action: {Action}", request.Action);
                await _processPaymentUseCase.ExecuteAsync(request);
            }

            return Ok();
        }
    }
}