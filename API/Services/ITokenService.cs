namespace API.Services
{
    public interface ITokenService
    {
        string GenerateTokenAsync(string username);
        //TODO for future implementation
        //Task<string> RefreshTokenAsync(string token, CancellationToken ct);
    }
}
