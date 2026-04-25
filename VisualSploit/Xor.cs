using System.Text;

namespace VisualSploit;

internal static class Xor
{
    internal const int KeySize = 32;

    public static (byte[] Data, byte[][] Keys) Encrypt(byte[] buf, int rounds, Random? rng = null)
    {
        var keys = new byte[rounds][];
        var result = (byte[])buf.Clone();

        for (int i = 0; i < rounds; i++)
        {
            keys[i] = new byte[KeySize];
            if (rng != null) rng.NextBytes(keys[i]);
            else System.Security.Cryptography.RandomNumberGenerator.Fill(keys[i]);

            for (int j = 0; j < result.Length; j++)
                result[j] ^= keys[i][j % KeySize];
        }

        return (result, keys);
    }

    public static string Routine(string buf, byte[] data, byte[][] keys)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"var {buf} = new byte[] {{ {Hex(data)} }};");
        sb.AppendLine();

        for (int i = keys.Length - 1; i >= 0; i--)
            sb.AppendLine($"var {buf}_k{i} = new byte[] {{ {Hex(keys[i])} }};");
        sb.AppendLine();

        for (int i = keys.Length - 1; i >= 0; i--)
        {
            sb.AppendLine($"for (int {buf}_i{i} = 0; {buf}_i{i} < {buf}.Length; {buf}_i{i}++)");
            sb.AppendLine($"    {buf}[{buf}_i{i}] ^= {buf}_k{i}[{buf}_i{i} % {KeySize}];");
            if (i > 0) sb.AppendLine();
        }

        return sb.ToString();
    }

    static string Hex(byte[] data) =>
        string.Join(", ", data.Select(b => $"0x{b:X2}"));
}
