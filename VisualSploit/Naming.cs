namespace VisualSploit;

internal class Naming
{
    const string Consonants = "bcdfghjklmnpqrstvwxyz";
    const string Vowels = "aeiou";

    internal static readonly HashSet<string> Reserved = new(StringComparer.Ordinal)
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

    readonly Random _rng;
    readonly HashSet<string> _used = new();

    public Naming(int? seed = null) =>
        _rng = seed.HasValue ? new Random(seed.Value) : new Random();

    /// <summary>
    /// Shared RNG stream. Xor.Encrypt draws from this directly; every Naming.Next() call
    /// consumes the same stream. A fixed --seed yields byte-identical output end-to-end
    /// only because all consumers agree on the draw order.
    /// </summary>
    public Random Rng => _rng;

    public string Next()
    {
        string name;
        do
        {
            var len = _rng.Next(4, 8);
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                var alphabet = i % 2 == 0 ? Consonants : Vowels;
                chars[i] = alphabet[_rng.Next(alphabet.Length)];
            }
            name = new string(chars);
        }
        while (Reserved.Contains(name) || !_used.Add(name));

        return name;
    }
}
