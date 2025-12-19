namespace VisualSploit.Core;

internal static class Shellcode
{
    public static byte[] Parse(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Shellcode file not found: {path}");

        var content = File.ReadAllText(path).Trim();
        byte[] shellcode;

        if (content.StartsWith("0x") || IsHexString(content.Replace(" ", "").Replace("\r", "").Replace("\n", "")))
        {
            var hex = content.Replace("0x", "").Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace(",", "");
            shellcode = Convert.FromHexString(hex);
        }
        else
        {
            shellcode = File.ReadAllBytes(path);
        }

        return AppendExit(shellcode);
    }

    static byte[] AppendExit(byte[] shellcode)
    {
        byte[] exitStub = [0x48, 0x83, 0xC4, 0x38, 0xC3];

        var result = new byte[shellcode.Length + exitStub.Length];
        shellcode.CopyTo(result, 0);
        exitStub.CopyTo(result, shellcode.Length);
        return result;
    }

    static bool IsHexString(string str) =>
        str.Length > 0 && str.Length % 2 == 0 && str.All(c => Uri.IsHexDigit(c));
}
