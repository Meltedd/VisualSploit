using System.Text;
using VisualSploit.Core;

namespace VisualSploit.Crypto;

internal static class Xor
{

    public static (byte[] Data, byte[][] Keys) Encrypt(byte[] buf, int rounds, Random? rng = null)
    {
        if (rounds < 1 || rounds > 10)
            throw new ArgumentException("rounds must be 1-10");

        var keys = new byte[rounds][];
        var result = (byte[])buf.Clone();

        for (int i = 0; i < rounds; i++)
        {
            keys[i] = new byte[32];
            if (rng != null) rng.NextBytes(keys[i]);
            else System.Security.Cryptography.RandomNumberGenerator.Fill(keys[i]);

            for (int j = 0; j < result.Length; j++)
                result[j] ^= keys[i][j % 32];
        }

        return (result, keys);
    }

    public static string Routine(byte[] data, byte[][] keys)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Utils.Indent}var buffer = new byte[] {{ {Utils.Hex(data)} }};");;
        sb.AppendLine();

        for (int i = keys.Length - 1; i >= 0; i--)
            sb.AppendLine($"{Utils.Indent}var t{i} = new byte[] {{ {Utils.Hex(keys[i])} }};");
        sb.AppendLine();

        for (int i = keys.Length - 1; i >= 0; i--)
        {
            sb.AppendLine($"{Utils.Indent}for (int n{i} = 0; n{i} < buffer.Length; n{i}++)");
            sb.AppendLine($"{Utils.Indent}    buffer[n{i}] ^= t{i}[n{i} % 32];");
            if (i > 0) sb.AppendLine();
        }

        return sb.ToString();
    }
}
