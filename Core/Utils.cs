namespace VisualSploit.Core;

internal static class Utils
{
    public const string Indent = "                  ";

    public static string Hex(byte[] data) =>
        string.Join(", ", data.Select(x => $"0x{x:X2}"));

    public static Random CreateRng(int? seed) =>
        seed.HasValue ? new Random(seed.Value) : new Random();

    public static string Bytes(string s) =>
        Hex(System.Text.Encoding.UTF8.GetBytes(s));
}
