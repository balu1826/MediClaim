namespace MediClaim.Application.Common.Interfaces;

public interface IEncryptionService
{
    byte[] Encrypt(string plainText);

    string Decrypt(byte[] cipherText);
}