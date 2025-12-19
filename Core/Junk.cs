using System.Text;

namespace VisualSploit.Core;

internal static class Junk
{
    static readonly string[] EnvProps = [
        "ProcessorCount", "Is64BitOperatingSystem", "Is64BitProcess", "TickCount",
        "CurrentManagedThreadId", "WorkingSet", "HasShutdownStarted", "UserInteractive"
    ];

    public static string Generate(Naming naming, Random rng, int count)
    {
        if (count <= 0) return "";

        var sb = new StringBuilder();
        for (int i = 0; i < count; i++)
        {
            var v = naming.Next();
            string code = (rng.Next(5)) switch
            {
                0 => $"{Utils.Indent}var {v} = System.Environment.{EnvProps[rng.Next(EnvProps.Length)]};",
                1 => $"{Utils.Indent}var {v} = System.Math.Sqrt({rng.Next(1, 1000)});",
                2 => $"{Utils.Indent}var {v} = \"{RandStr(rng, 3, 7)}\".ToUpper();",
                3 => $"{Utils.Indent}var {v} = new[] {{ {string.Join(", ", Enumerable.Range(0, rng.Next(2, 6)).Select(_ => rng.Next(100)))} }}.Length;",
                _ => $"{Utils.Indent}var {v} = System.DateTime.Now.Ticks;"
            };
            sb.AppendLine(code);
        }
        return sb.ToString();
    }

    public static string Get(Config cfg, Naming naming, Random rng) =>
        cfg.Has(Methods.Junk)
            ? Generate(naming, rng, rng.Next(1, 4)) + "\n"
            : "";

    static string RandStr(Random rng, int min, int max)
    {
        var chars = new char[rng.Next(min, max)];
        for (int i = 0; i < chars.Length; i++)
            chars[i] = (char)('a' + rng.Next(26));
        return new string(chars);
    }
}
