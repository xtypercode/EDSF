using System.Security.Cryptography;
using System.Text;

namespace EDSF.Api.Services;

public class PasswordService(IConfiguration configuration)
{
    private readonly string _secret = configuration["Jwt:Key"] ?? "edsf-default-secret-change-me";

    public string Hash(string password)
    {
        var data = $"{password}|{_secret}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public bool Verify(string password, string hash) =>
        Hash(password) == hash;
}
