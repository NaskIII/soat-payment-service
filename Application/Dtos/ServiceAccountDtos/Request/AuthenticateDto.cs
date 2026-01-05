namespace Application.Dtos.ServiceAccountDtos.Request
{
    public record AuthenticateDto(
        Guid ClientId,
        string ClientSecret
    );
}
