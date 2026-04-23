using System.Text;

namespace VisualSploit;

internal static class Junk
{
    static readonly string[] EnvProps = [
        "ProcessorCount", "Is64BitOperatingSystem", "Is64BitProcess", "TickCount",
        "CurrentManagedThreadId", "WorkingSet", "HasShutdownStarted", "UserInteractive"
    ];

    static readonly Func<Random, string>[] Shapes =
    [
        rng => $"_ = System.Environment.{EnvProps[rng.Next(EnvProps.Length)]};",
        rng => $"_ = System.Math.Sqrt({rng.Next(1, 1000)});",
        rng => $"_ = \"{RandStr(rng, 3, 7)}\".ToUpper();",
        rng => $"_ = new[] {{ {string.Join(", ", Enumerable.Range(0, rng.Next(2, 6)).Select(_ => rng.Next(100)))} }}.Length;",
        rng => $"_ = System.DateTime.Now.Ticks;",
    ];

    public static string Emit(Config cfg, Naming naming)
    {
        if (!cfg.Junk) return "";

        var rng = naming.Rng;
        var count = rng.Next(1, 4);
        var sb = new StringBuilder();

        for (int i = 0; i < count; i++)
            sb.AppendLine(Shapes[rng.Next(Shapes.Length)](rng));

        return sb.ToString();
    }

    static string RandStr(Random rng, int min, int max)
    {
        var chars = new char[rng.Next(min, max)];
        for (int i = 0; i < chars.Length; i++)
            chars[i] = (char)('a' + rng.Next(26));
        return new string(chars);
    }
}
