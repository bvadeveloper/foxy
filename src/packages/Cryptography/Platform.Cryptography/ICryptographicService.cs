namespace Platform.Cryptography;

public interface ICryptographicService : IDisposable
{
    /// <summary>
    /// Encrypt data with generated Diffie-Hellman's derived secret key
    /// </summary>
    /// <param name="data"></param>
    /// <param name="publicKey"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    ValueTask<byte[]> Encrypt(byte[] data, byte[] publicKey, out byte[] iv);

    ValueTask<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv);

    bool TrySetKeyPair(string keyPair);

    string GetKeyPair();
    
    byte[] GetPublicKey();
}