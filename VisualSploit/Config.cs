namespace VisualSploit;

internal enum TargetPlatform
{
    Windows,
    Linux
}

internal enum ShellcodeFormat
{
    Auto,
    Raw,
    Hex
}

internal record Config(
    string TargetPath,
    string ShellcodePath,
    string? OutputPath,
    int XorRounds,
    int? Seed,
    TargetPlatform Platform,
    ShellcodeFormat ShellcodeFormat,
    string? Condition,
    bool NoBackup,
    bool DryRun,
    bool Verbose)
{
    public const int MaxXorRounds = 5;
}
