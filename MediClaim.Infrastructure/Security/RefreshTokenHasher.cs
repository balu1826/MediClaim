using System.Security.Cryptography;
using System.Text;

namespace MediClaim.Infrastructure
    .Security;

public static class RefreshTokenHasher
{
    public static string Hash(
        string token)
    {
        var bytes =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    token));

        return Convert.ToBase64String(
            bytes);
    }
}