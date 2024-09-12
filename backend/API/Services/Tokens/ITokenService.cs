namespace API.Services.Tokens
{
    public interface ITokenService
    {
        string GenerateToken(string username);
        //TODO for future implementation
        //Task<string> RefreshTokenAsync(string token, CancellationToken ct);
    }
}
