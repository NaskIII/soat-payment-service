using Application.Dtos.CheckoutDtos.Request;
using Application.Dtos.MercadoPagoDtos.Request;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace FastFood.Controllers.V1
{
    [Authorize]
    [ApiController]
    [Route("api/v1/payment")]
    public class PaymentController : ControllerBase
    {

        private readonly ICheckoutUseCase _checkoutUseCase;
        private readonly IPaymentStatusUseCase _paymentStatusUseCase;

        public PaymentController(ICheckoutUseCase checkoutUseCase, IPaymentStatusUseCase paymentStatusUseCase)
        {
            _checkoutUseCase = checkoutUseCase;
            _paymentStatusUseCase = paymentStatusUseCase;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] PaymentRequestDto request)
        {
            try
            {
                (Guid orderId, string qrCode)  = await _checkoutUseCase.ExecuteAsync(request);

                return Ok(new { qr_data = qrCode, in_store_order_id = orderId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao processar sua solicitação.");
            }
        }

        [HttpGet("status/{orderId}")]
        public async Task<ActionResult<int>> GetPaymentStatusByOrderId(Guid orderId)
        {
            try
            {
                var paymentStatus = await _paymentStatusUseCase.ExecuteAsync(orderId);

                return Ok(paymentStatus);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
