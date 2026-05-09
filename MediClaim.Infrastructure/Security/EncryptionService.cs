using MediClaim.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace MediClaim.Infrastructure.Security;

public class EncryptionService
    : IEncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(
        IConfiguration configuration)
    {
        _key = Encoding.UTF8.GetBytes(
            configuration["Encryption:Key"]!);
    }

    public byte[] Encrypt(string plainText)
    {
        using var aes = Aes.Create();

        aes.Key = _key;

        aes.GenerateIV();

        using var encryptor =
            aes.CreateEncryptor();

        var plainBytes =
            Encoding.UTF8.GetBytes(plainText);

        var encryptedBytes =
            encryptor.TransformFinalBlock(
                plainBytes,
                0,
                plainBytes.Length);

        return aes.IV
            .Concat(encryptedBytes)
            .ToArray();
    }

    public string Decrypt(byte[] cipherText)
    {
        using var aes = Aes.Create();

        aes.Key = _key;

        var iv =
            cipherText.Take(16).ToArray();

        var encrypted =
            cipherText.Skip(16).ToArray();

        aes.IV = iv;

        using var decryptor =
            aes.CreateDecryptor();

        var decryptedBytes =
            decryptor.TransformFinalBlock(
                encrypted,
                0,
                encrypted.Length);

        return Encoding.UTF8.GetString(
            decryptedBytes);
    }
}