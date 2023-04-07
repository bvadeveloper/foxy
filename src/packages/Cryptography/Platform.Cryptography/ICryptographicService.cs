namespace Platform.Cryptography;

public interface ICryptographicService : IDisposable
{
    /// <summary>
    /// Encrypt data with generated Diffie-Hellman's derived secret key
    /// </summary>
    /// <param name="data"></param>
    /// <param name="publicKeyBob"></param>
    /// <returns></returns>
    ValueTask<(byte[] encryptedData, byte[] iv)> Encrypt(byte[] data, byte[] publicKeyBob);

    /// <summary>
    /// Decrypt data with generated Diffie-Hellman's derived secret key
    /// </summary>
    /// <param name="data"></param>
    /// <param name="alicePublicKey"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    ValueTask<byte[]> Decrypt(byte[] data, byte[] alicePublicKey, byte[] iv);

    bool TrySetKeyPair(string keyPair);

    string GetKeyPair();
    
    byte[] GetPublicKey();
}