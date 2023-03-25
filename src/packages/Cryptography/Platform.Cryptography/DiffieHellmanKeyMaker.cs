using System.Security.Cryptography;

namespace Platform.Cryptography;

public class DiffieHellmanKeyMaker : IDisposable
{
    private readonly ECDiffieHellman _ecdh;

    public byte[] PublicKey => _ecdh.PublicKey.ExportSubjectPublicKeyInfo();

    public DiffieHellmanKeyMaker() => _ecdh = ECDiffieHellman.Create();

    public void Dispose() => _ecdh.Dispose();

    public byte[] MakeDerivedSecret(byte[] publicKey)
    {
        using var ecdh = ECDiffieHellman.Create();
        ecdh.ImportSubjectPublicKeyInfo(publicKey, out _);
        return _ecdh.DeriveKeyFromHash(ecdh.PublicKey, HashAlgorithmName.SHA256);
    }
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