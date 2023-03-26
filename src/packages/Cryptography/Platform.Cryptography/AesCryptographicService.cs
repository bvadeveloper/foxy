using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;

namespace Platform.Cryptography;

/// <summary>
/// Use singleton scope for this instance
/// </summary>
public class AesCryptographicService : ICryptographicService
{
    private static readonly ECDiffieHellman Ecdh = ECDiffieHellman.Create();

    private readonly ILogger _logger;

    public AesCryptographicService(ILogger<AesCryptographicService> logger) => _logger = logger;

    private static byte[] PublicKey => Ecdh.PublicKey.ExportSubjectPublicKeyInfo();

    private static byte[] PrivateKey => Ecdh.ExportPkcs8PrivateKey();
    
    private static string PublicKeyBase64 => PublicKey.ToBase64String();

    private static string PrivateKeyBase64 => PrivateKey.ToBase64String();

    
    public ValueTask<byte[]> Encrypt(byte[] data, byte[] publicKey, out byte[] iv)
    {
        throw new NotImplementedException();
    }

    public ValueTask<byte[]> Decrypt(byte[] data, byte[] publicKey, byte[] iv)
    {
        throw new NotImplementedException();
    }

    public bool TrySetKeyPair(string keyPair)
    {
        var span = keyPair.AsSpan();
        var delimiter = span.IndexOf(':');
        var publicKeyBase64 = span[..delimiter].ToString();
        var privateKeyBase64 = span[(delimiter + 1)..].ToString();

        var publicKeySpan = new Span<byte>();
        var privateKeySpan = new Span<byte>();

        if (Convert.TryFromBase64String(publicKeyBase64, publicKeySpan, out _) && Convert.TryFromBase64String(privateKeyBase64, privateKeySpan, out _))
        {
            _logger.Info("Key pair successfully set");
            Ecdh.ImportSubjectPublicKeyInfo(publicKeySpan, out _);
            Ecdh.ImportPkcs8PrivateKey(privateKeySpan, out _);

            return true;
        }

        _logger.Error($"Cannot set key pair '{keyPair}'");

        return false;
    }

    public string GetKeyPair() => $"{PublicKeyBase64}:{PrivateKeyBase64}";
    public byte[] GetPublicKey() => Ecdh.PublicKey.ExportSubjectPublicKeyInfo();

    public byte[] MakeDerivedKey(byte[] publicKey)
    {
        using var ecdh = ECDiffieHellman.Create();
        ecdh.ImportSubjectPublicKeyInfo(publicKey, out _);
        return Ecdh.DeriveKeyFromHash(ecdh.PublicKey, HashAlgorithmName.SHA256);
    }

    public void Dispose() => Ecdh.Dispose();
}


// using var ecdh = ECDiffieHellman.Create();
//
// var privateKey = ecdh.ExportPkcs8PrivateKey();
// var publicKey = ecdh.PublicKey.ExportSubjectPublicKeyInfo();
//
// var encodedPrivateKey = Convert.ToBase64String(privateKey);
// var encodedPublicKey = Convert.ToBase64String(publicKey);
//
// Console.WriteLine($"Private key: {encodedPrivateKey}");
// Console.WriteLine($"Public key: {encodedPublicKey}");
//
// var importedPrivateKey = Convert.FromBase64String(encodedPrivateKey);
// var importedPublicKey = Convert.FromBase64String(encodedPublicKey);
//
// using var ecdh2 = ECDiffieHellman.Create();
// using var ecdh3 = ECDiffieHellman.Create();
//
// try
// {
//     // import the private key using the ImportECPrivateKey method
//     ecdh2.ImportPkcs8PrivateKey(importedPrivateKey, out _);
//
//     // import the public key using the ImportSubjectPublicKeyInfo method
//     ecdh3.ImportSubjectPublicKeyInfo(importedPublicKey, out _);
//
//     // generate the shared secret key using the imported public key and private key
//     byte[] sharedSecret = ecdh2.DeriveKeyFromHash(ecdh3.PublicKey, HashAlgorithmName.SHA256);
//
//     // use the shared secret key for encryption or other purposes
//     Console.WriteLine($"Secret: {Convert.ToBase64String(sharedSecret)}");
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"Error importing keys or deriving shared secret: {ex.Message}");
// }