using Application.Dtos.ServiceAccountDtos.Request;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers.V1
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/v1/service-account")]
    public class ServiceAccountController : ControllerBase
    {

        private readonly IAuthenticateServiceAccount _authenticateServiceAccount;

        public ServiceAccountController(IAuthenticateServiceAccount authenticateServiceAccount)
        {
            _authenticateServiceAccount = authenticateServiceAccount;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateDto request)
        {
            try
            {
                string token = await _authenticateServiceAccount.ExecuteAsync(request);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing the request.", Details = ex.Message });
            }
        }
    }
}
