using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;

namespace Platform.Cryptography;

/// <summary>
/// Use singleton scope for this instance
/// </summary>
public class DiffieHellmanKeyMaker : IDisposable
{
    private static readonly ECDiffieHellman Ecdh = ECDiffieHellman.Create();

    private readonly ILogger _logger;

    public DiffieHellmanKeyMaker(ILogger<DiffieHellmanKeyMaker> logger)
    {
        _logger = logger;
    }

    public byte[] PublicKey => Ecdh.PublicKey.ExportSubjectPublicKeyInfo();

    public byte[] PrivateKey => Ecdh.ExportPkcs8PrivateKey();

    public string PublicKeyBase64 => Convert.ToBase64String(PublicKey, Base64FormattingOptions.None);

    public string PrivateKeyBase64 => Convert.ToBase64String(PrivateKey, Base64FormattingOptions.None);

    public string KeyPairBase64 => $"{PublicKeyBase64}:{PrivateKeyBase64}";


    public byte[] MakeDerivedSecret(byte[] publicKey)
    {
        using var ecdh = ECDiffieHellman.Create();
        ecdh.ImportSubjectPublicKeyInfo(publicKey, out _);
        return Ecdh.DeriveKeyFromHash(ecdh.PublicKey, HashAlgorithmName.SHA256);
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
        
        return true;
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