using System.Text;

namespace VisualSploit;

internal static class Shellcode
{
    public static byte[] Parse(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Shellcode file not found: {path}");

        var bytes = File.ReadAllBytes(path);
        if (bytes.Length == 0)
            throw new InvalidDataException($"Shellcode file is empty: {path}");

        if (!IsHexText(bytes))
            return bytes;

        var cleaned = Clean(Encoding.UTF8.GetString(bytes));
        if (cleaned.Length == 0 || cleaned.Length % 2 != 0 || !cleaned.All(Uri.IsHexDigit))
            throw new InvalidDataException($"Invalid hex shellcode: {path}");

        return Convert.FromHexString(cleaned);
    }

    static string Clean(string input)
    {
        var sb = new StringBuilder(input.Length);
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsWhiteSpace(c) || c == ',' || c == '\uFEFF')
                continue;
            if (c == '0' && i + 1 < input.Length && (input[i + 1] == 'x' || input[i + 1] == 'X'))
            {
                i++;
                continue;
            }
            sb.Append(c);
        }
        return sb.ToString();
    }

    static bool IsHexText(byte[] bytes)
    {
        foreach (var b in bytes)
        {
            bool ok = b switch
            {
                (byte)'\t' or (byte)'\n' or (byte)'\r' or (byte)' ' or (byte)',' => true,
                (byte)'x' or (byte)'X' => true,
                0xEF or 0xBB or 0xBF => true,
                _ when b <= 0x7F && Uri.IsHexDigit((char)b) => true,
                _ => false,
            };
            if (!ok) return false;
        }
        return true;
    }
}
