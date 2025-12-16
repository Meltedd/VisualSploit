namespace VisualSploit.Obfuscation;

[Flags]
internal enum ObfuscationMethods
{
    None = 0,
    ShellcodeEncryption = 1,
    JunkCodeInjection = 2
}

internal record ObfuscationConfig(
    bool UseObfuscation = false,
    bool UseEncryptedLoader = false,
    ObfuscationMethods Methods = ObfuscationMethods.None,
    int XorRounds = 3,
    int? Seed = null,
    Dictionary<string, string>? VariableNames = null
)
{
    public bool HasMethod(ObfuscationMethods method) => (Methods & method) != 0;
}

internal static class StubVars
{
    public static Dictionary<string, string> Default => new()
    {
        ["enc"] = "data",
        ["iv"] = "nonce",
        ["key"] = "key",
        ["salt"] = "salt",
        ["code"] = "code",
        ["provider"] = "provider",
        ["compiler"] = "compiler",
        ["result"] = "result",
        ["assembly"] = "assembly",
        ["type"] = "type",
        ["method"] = "method"
    };
}
