using Application.Dtos.ServiceAccountDtos.Request;
using Application.Exceptions;
using Application.Interfaces;
using Application.UseCases.ServiceAccountsUseCases;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using Domain.Security;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Test
{
    public class AuthenticateUseCaseTests
    {
        private readonly Mock<IAccountServiceRepository> _accountServiceRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AuthenticateUseCase _useCase;

        public AuthenticateUseCaseTests()
        {
            _accountServiceRepositoryMock = new Mock<IAccountServiceRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenServiceMock = new Mock<ITokenService>();

            _useCase = new AuthenticateUseCase(
                _accountServiceRepositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenServiceMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_Should_Return_Token_When_Credentials_Are_Valid()
        {
            var clientId = Guid.NewGuid();
            var clientSecret = "my_secret_123";
            var hashedSecret = "hashed_secret_123";
            var expectedToken = "valid.jwt.token";

            var request = new AuthenticateDto(clientId, clientSecret);

            var serviceAccount = new ServiceAccounts(
                new Name("TestService"),
                clientId,
                hashedSecret
            );

            _accountServiceRepositoryMock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<ServiceAccounts, bool>>>()))
                .ReturnsAsync(serviceAccount);

            _passwordHasherMock.Setup(x => x.VerifyPassword(clientSecret, hashedSecret))
                .Returns(true);

            _tokenServiceMock.Setup(x => x.GenerateToken(serviceAccount.Id, clientId.ToString()))
                .Returns(expectedToken);

            var result = await _useCase.ExecuteAsync(request);

            result.Should().Be(expectedToken);

            _accountServiceRepositoryMock.Verify(x => x.GetSingleAsync(It.IsAny<Expression<Func<ServiceAccounts, bool>>>()), Times.Once);
            _passwordHasherMock.Verify(x => x.VerifyPassword(clientSecret, hashedSecret), Times.Once);
            _tokenServiceMock.Verify(x => x.GenerateToken(serviceAccount.Id, clientId.ToString()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Throw_NotFoundException_When_Account_Does_Not_Exist()
        {
            var request = new AuthenticateDto(Guid.NewGuid(), "any_secret");

            _accountServiceRepositoryMock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<ServiceAccounts, bool>>>()))
                .ReturnsAsync((ServiceAccounts?)null);

            Func<Task> act = async () => await _useCase.ExecuteAsync(request);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Service account not found.");

            _accountServiceRepositoryMock.Verify(x => x.GetSingleAsync(It.IsAny<Expression<Func<ServiceAccounts, bool>>>()), Times.Once);
            _passwordHasherMock.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Throw_UnauthorizedAccessException_When_Secret_Is_Invalid()
        {
            var clientId = Guid.NewGuid();
            var clientSecret = "wrong_secret";
            var hashedSecret = "real_hashed_secret";

            var request = new AuthenticateDto(clientId, clientSecret);

            var serviceAccount = new ServiceAccounts(
                new Name("TestService"),
                clientId,
                hashedSecret
            );

            _accountServiceRepositoryMock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<ServiceAccounts, bool>>>()))
                .ReturnsAsync(serviceAccount);

            _passwordHasherMock.Setup(x => x.VerifyPassword(clientSecret, hashedSecret))
                .Returns(false);

            Func<Task> act = async () => await _useCase.ExecuteAsync(request);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid client secret.");

            _accountServiceRepositoryMock.Verify(x => x.GetSingleAsync(It.IsAny<Expression<Func<ServiceAccounts, bool>>>()), Times.Once);
            _passwordHasherMock.Verify(x => x.VerifyPassword(clientSecret, hashedSecret), Times.Once);
            _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }
    }
}