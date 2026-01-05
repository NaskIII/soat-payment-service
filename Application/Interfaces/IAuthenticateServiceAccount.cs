using Application.Dtos.ServiceAccountDtos.Request;

namespace Application.Interfaces
{
    public interface IAuthenticateServiceAccount : IUseCase<AuthenticateDto, string>
    {
    }
}
