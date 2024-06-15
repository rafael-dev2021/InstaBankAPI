namespace AuthenticateAPI.Security;

public class JwtSettings
{
    public string? SecretKey { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int ExpirationTokenMinutes { get; set; }
    public int RefreshTokenExpirationMinutes { get; set; }
}