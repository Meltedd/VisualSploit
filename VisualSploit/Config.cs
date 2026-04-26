namespace VisualSploit;

internal record Config(
    string TargetPath,
    string ShellcodePath,
    string? OutputPath,
    int XorRounds,
    int? Seed,
    bool NoBackup,
    bool DryRun,
    bool Verbose)
{
    public const int MaxXorRounds = 5;
}
