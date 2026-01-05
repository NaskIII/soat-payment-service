using Application.Dtos.CheckoutDtos.Request;
using Application.Dtos.MercadoPagoDtos.Request;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers.V1
{
    [Obsolete("This controller is deprecated and will be removed in future versions.")]
    [Authorize]
    [ApiController]
    [Route("api/v1/checkout")]
    public class CheckoutController : ControllerBase
    {

        private readonly ICheckoutUseCase _checkoutUseCase;

        public CheckoutController(ICheckoutUseCase checkoutUseCase)
        {
            _checkoutUseCase = checkoutUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] PaymentRequestDto request)
        {
            try
            {
                (Guid orderId, string qrData) = await _checkoutUseCase.ExecuteAsync(request);
                return Ok(new
                {
                    orderId,
                    qrData
                });
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
    }
}
