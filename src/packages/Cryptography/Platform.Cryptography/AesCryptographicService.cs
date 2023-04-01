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
        using var aes = Aes.Create();
        aes.Key = MakeDerivedKey(publicKey);
        aes.GenerateIV();

        using var memoryStreamEncrypt = new MemoryStream();
        await using (var cryptoStream = new CryptoStream(memoryStreamEncrypt, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            await cryptoStream.WriteAsync(data, 0, data.Length);
        }

        return (memoryStreamEncrypt.ToArray(), aes.IV);
    }

    public async ValueTask<byte[]> Decrypt(byte[] data, byte[] publicKey, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = MakeDerivedKey(publicKey);
        aes.IV = iv;

        using var memoryStreamDecrypt = new MemoryStream(data);
        await using var cryptoStream = new CryptoStream(memoryStreamDecrypt, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var memoryStreamOutput = new MemoryStream();
        var buffer = new byte[data.Length];
        int bytesRead;
        while ((bytesRead = await cryptoStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await memoryStreamOutput.WriteAsync(buffer, 0, bytesRead);
        }

        return memoryStreamOutput.ToArray();
    }

    public bool TrySetKeyPair(string keyPair)
    {
        var keyPairSpan = keyPair.AsSpan();
        var delimiterIndex = keyPairSpan.IndexOf(':');
        var publicKeyBase64 = keyPairSpan[..delimiterIndex].ToString();
        var privateKeyBase64 = keyPairSpan[(delimiterIndex + 1)..].ToString();

        Span<byte> publicKeySpan = stackalloc byte[publicKeyBase64.Length];
        Span<byte> privateKeySpan = stackalloc byte[privateKeyBase64.Length];

        if (Convert.TryFromBase64String(publicKeyBase64, publicKeySpan, out _)
            && Convert.TryFromBase64String(privateKeyBase64, privateKeySpan, out _))
        {
            Ecdh.ImportSubjectPublicKeyInfo(publicKeySpan, out int publicKeyBytesRead);
            Ecdh.ImportPkcs8PrivateKey(privateKeySpan, out int privateKeyBytesRead);

            _logger.Info($"Key pair successfully set, public key bytes read '{publicKeyBytesRead}', private key bytes read '{privateKeyBytesRead}'");

            return true;
        }

        _logger.Error($"Cannot set key pair, is value exist: '{!string.IsNullOrEmpty(keyPair)}'");

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