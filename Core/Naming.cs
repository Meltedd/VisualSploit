namespace VisualSploit.Core;

internal class Naming
{
    static readonly HashSet<string> Reserved = new()
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

    public Naming(int? seed = null) => _rng = Utils.CreateRng(seed);

    public string Next()
    {
        string name;
        do
        {
            var len = _rng.Next(4, 8);
            var chars = new char[len];
            for (int i = 0; i < len; i++)
                chars[i] = (i % 2 == 0 ? "bcdfghjklmnpqrstvwxyz" : "aeiou")[_rng.Next(i % 2 == 0 ? 21 : 5)];
            name = new string(chars);
        }
        while (_used.Contains(name) || Reserved.Contains(name));

        _used.Add(name);
        return name;
    }
}
