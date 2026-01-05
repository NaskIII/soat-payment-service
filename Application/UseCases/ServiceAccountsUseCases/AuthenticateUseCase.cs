using Application.Dtos.ServiceAccountDtos.Request;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using Domain.Security;

namespace Application.UseCases.ServiceAccountsUseCases
{
    public class AuthenticateUseCase : IAuthenticateServiceAccount
    {

        private readonly IAccountServiceRepository _accountServiceRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthenticateUseCase(
            IAccountServiceRepository accountServiceRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService
            ) 
        { 
            _accountServiceRepository = accountServiceRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<string> ExecuteAsync(AuthenticateDto request)
        {
            ServiceAccounts? serviceAccounts = await _accountServiceRepository.GetSingleAsync(x => x.ClientId == request.ClientId);

            if (serviceAccounts == null)
            {
                throw new NotFoundException("Service account not found.");
            }

            bool isValidSecret = _passwordHasher.VerifyPassword(request.ClientSecret, serviceAccounts.ClientSecretHash);

            if (!isValidSecret)
            {
                throw new UnauthorizedAccessException("Invalid client secret.");
            }

            string token = _tokenService.GenerateToken(serviceAccounts.Id, serviceAccounts.ClientId.ToString());

            return token;
        }
    }
}
