namespace Platform.Cryptography;

public interface ICryptographicService
{
    ValueTask<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv);

    ValueTask<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv);
}