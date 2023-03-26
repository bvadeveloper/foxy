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


    public async ValueTask<(byte[] encryptedData, byte[] iv)> Encrypt(byte[] data, byte[] publicKey)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = MakeDerivedKey(publicKey);
        aesAlg.GenerateIV();

        using var memoryStream = new MemoryStream();
        await using (var cryptoStream = new CryptoStream(memoryStream, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
        {
            await cryptoStream.WriteAsync(data, 0, data.Length);
            await cryptoStream.FlushFinalBlockAsync();
        }

        return (memoryStream.ToArray(), aesAlg.IV);
    }

    public async ValueTask<byte[]> Decrypt(byte[] data, byte[] publicKey, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = MakeDerivedKey(publicKey);
        aes.IV = iv;

        using var memoryStream = new MemoryStream(data);
        await using var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        var decryptedBytes = new byte[data.Length];
        var decryptedByteCount = await cryptoStream.ReadAsync(decryptedBytes, 0, decryptedBytes.Length);
        Array.Resize(ref decryptedBytes, decryptedByteCount);

        return decryptedBytes;
    }

    public bool TrySetKeyPair(string keyPair)
    {
        var span = keyPair.AsSpan();
        var delimiter = span.IndexOf(':');
        var publicKeyBase64 = span[..delimiter].ToString();
        var privateKeyBase64 = span[(delimiter + 1)..].ToString();

        var publicKeySpan = new Span<byte>();
        var privateKeySpan = new Span<byte>();

        if (Convert.TryFromBase64String(publicKeyBase64, publicKeySpan, out _)
            && Convert.TryFromBase64String(privateKeyBase64, privateKeySpan, out _))
        {
            _logger.Info("Key pair successfully set");
            Ecdh.ImportSubjectPublicKeyInfo(publicKeySpan, out _);
            Ecdh.ImportPkcs8PrivateKey(privateKeySpan, out _);

            return true;
        }

        _logger.Error($"Cannot set key pair, value exist: '{!string.IsNullOrEmpty(keyPair)}'");

        return false;
    }

    public string GetKeyPair() => $"{PublicKeyBase64}:{PrivateKeyBase64}";
    public byte[] GetPublicKey() => Ecdh.PublicKey.ExportSubjectPublicKeyInfo();

    private byte[] MakeDerivedKey(byte[] publicKey)
    {
        using var ecdh = ECDiffieHellman.Create();
        ecdh.ImportSubjectPublicKeyInfo(publicKey, out _);
        return Ecdh.DeriveKeyFromHash(ecdh.PublicKey, HashAlgorithmName.SHA256);
    }

    public void Dispose() => Ecdh.Dispose();
}