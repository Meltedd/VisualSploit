using System.Security.Cryptography;
using System.Text;

namespace VisualSploit.Crypto;

internal record AesResult(string Data, byte[] Iv, byte[] Salt, byte[] Key);

internal static class Aes
{
    public static AesResult Encrypt(string code)
    {
        var salt = RandomNumberGenerator.GetBytes(32);
        var pwd = RandomNumberGenerator.GetBytes(32);
        var iv = RandomNumberGenerator.GetBytes(16);

        byte[] key;
        using (var kdf = new Rfc2898DeriveBytes(pwd, salt, 100000))
            key = kdf.GetBytes(32);

        using var aes = System.Security.Cryptography.Aes.Create();
        aes.KeySize = 256;
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var enc = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
        {
            var bytes = Encoding.UTF8.GetBytes(code);
            cs.Write(bytes, 0, bytes.Length);
        }

        return new AesResult(Convert.ToBase64String(ms.ToArray()), iv, salt, pwd);
    }
}
