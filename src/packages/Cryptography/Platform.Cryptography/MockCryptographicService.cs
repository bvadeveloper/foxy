namespace Platform.Cryptography;

public class MockCryptographicService : ICryptographicService
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public ValueTask<(byte[] encryptedData, byte[] iv)> Encrypt(byte[] data, byte[] publicKeyBob)
    {
        throw new NotImplementedException();
    }

    public ValueTask<byte[]> Decrypt(byte[] data, byte[] alicePublicKey, byte[] iv)
    {
        throw new NotImplementedException();
    }

    public bool TrySetKeyPair(string keyPair)
    {
        throw new NotImplementedException();
    }

    public string GetKeyPair()
    {
        throw new NotImplementedException();
    }

    public byte[] GetPublicKey()
    {
        throw new NotImplementedException();
    }
}