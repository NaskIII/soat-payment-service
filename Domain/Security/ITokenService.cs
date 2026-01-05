using System.Security.Claims;

namespace Domain.Security
{
    public interface ITokenService
    {
        string GenerateToken(Guid clientId, string cpf);
        string GenerateToken(Guid userId, string email, string[] roles);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
