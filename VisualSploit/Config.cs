namespace VisualSploit;

internal enum TargetPlatform
{
    Windows,
    Linux
}

internal record Config(
    string TargetPath,
    string ShellcodePath,
    string? OutputPath,
    int XorRounds,
    int? Seed,
    TargetPlatform Platform,
    bool NoBackup,
    bool DryRun,
    bool Verbose)
{
    public const int MaxXorRounds = 5;
}
