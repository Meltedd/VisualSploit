using System.Text;

namespace VisualSploit.Obfuscation;

internal class NameGenerator
{
    private const int MinVariableNameLength = 4;
    private const int MaxVariableNameLength = 8;

    private static readonly HashSet<string> ReservedKeywords = new()
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char",
        "checked", "class", "const", "continue", "decimal", "default", "delegate",
        "do", "double", "else", "enum", "event", "explicit", "extern", "false",
        "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit",
        "in", "int", "interface", "internal", "is", "lock", "long", "namespace",
        "new", "null", "object", "operator", "out", "override", "params", "private",
        "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
        "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
        "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
        "unsafe", "ushort", "using", "var", "virtual", "void", "volatile", "while"
    };

    private readonly Random _random;
    private readonly HashSet<string> _usedNames = new();

    public NameGenerator(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public string GenerateVariableName()
    {
        string name;
        do
        {
            var length = _random.Next(MinVariableNameLength, MaxVariableNameLength);
            name = GenerateRandomString(length);
        }
        while (_usedNames.Contains(name) || IsReservedKeyword(name));

        _usedNames.Add(name);
        return name;
    }

    private string GenerateRandomString(int length)
    {
        const string consonants = "bcdfghjklmnpqrstvwxyz";
        const string vowels = "aeiou";

        var result = new char[length];

        for (int i = 0; i < length; i++)
        {
            if (i % 2 == 0)
                result[i] = consonants[_random.Next(consonants.Length)];
            else
                result[i] = vowels[_random.Next(vowels.Length)];
        }

        return new string(result);
    }

    private static bool IsReservedKeyword(string name) =>
        ReservedKeywords.Contains(name.ToLower());
}

internal static class StringObfuscator
{
    public static (byte[] Encrypted, byte Key) Obfuscate(string str, Random random)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        var key = (byte)random.Next(1, 256);
        var encrypted = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
            encrypted[i] = (byte)(bytes[i] ^ key);
        return (encrypted, key);
    }

    public static string GenerateDecodeCode(byte[] encrypted, byte key, string varName, string indent)
    {
        var sb = new StringBuilder();
        var hexBytes = string.Join(", ", encrypted.Select(b => $"0x{b:X2}"));
        var loopVar = $"{varName}_idx";
        sb.AppendLine($"{indent}var {varName}_bytes = new byte[] {{ {hexBytes} }};");
        sb.AppendLine($"{indent}for (int {loopVar} = 0; {loopVar} < {varName}_bytes.Length; {loopVar}++) {varName}_bytes[{loopVar}] ^= 0x{key:X2};");
        sb.AppendLine($"{indent}var {varName} = System.Text.Encoding.UTF8.GetString({varName}_bytes);");
        return sb.ToString();
    }
}

internal static class JunkCodeInjector
{
    private const string Indent = "                  ";

    private static readonly string[] EnvironmentProperties =
    {
        "ProcessorCount",
        "Is64BitOperatingSystem",
        "Is64BitProcess",
        "TickCount",
        "CurrentManagedThreadId",
        "WorkingSet",
        "HasShutdownStarted",
        "UserInteractive"
    };

    private static readonly string[] EnvironmentMethods =
    {
        "UserDomainName.Length",
        "MachineName.Length",
        "SystemPageSize"
    };

    public static string Generate(NameGenerator nameGen, Random random, int count)
    {
        if (count <= 0) return string.Empty;

        var sb = new StringBuilder();

        for (int i = 0; i < count; i++)
        {
            var strategy = random.Next(0, 3);
            string code = strategy switch
            {
                0 => GenerateEnvironmentRead(nameGen, random),
                1 => GenerateTrivialSleep(random),
                _ => GenerateEnvironmentMethodRead(nameGen, random)
            };
            sb.AppendLine(code);
        }

        return sb.ToString();
    }

    private static string GenerateEnvironmentRead(NameGenerator nameGen, Random random)
    {
        var varName = nameGen.GenerateVariableName();
        var prop = EnvironmentProperties[random.Next(EnvironmentProperties.Length)];
        return $"{Indent}var {varName} = System.Environment.{prop};";
    }

    private static string GenerateEnvironmentMethodRead(NameGenerator nameGen, Random random)
    {
        var varName = nameGen.GenerateVariableName();
        var method = EnvironmentMethods[random.Next(EnvironmentMethods.Length)];
        return $"{Indent}var {varName} = System.Environment.{method};";
    }

    private static string GenerateTrivialSleep(Random random)
    {
        return $"{Indent}System.Threading.Thread.Sleep({random.Next(0, 2)});";
    }
}
