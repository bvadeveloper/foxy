namespace Platform.Cryptography;

public class AesCryptographicService : ICryptographicService
{
    private readonly DiffieHellmanKeyMaker _diffieHellmanKeyMaker;

    public AesCryptographicService(DiffieHellmanKeyMaker diffieHellmanKeyMaker)
    {
        _diffieHellmanKeyMaker = diffieHellmanKeyMaker;
    }

    public ValueTask<byte[]> Encrypt(byte[] data, byte[] publicKey, byte[] iv)
    {
        throw new NotImplementedException();
    }

    public ValueTask<byte[]> Decrypt(byte[] data, byte[] publicKey, byte[] iv)
    {
        throw new NotImplementedException();
    }
}