namespace VisualSploit.Core;

[Flags]
internal enum Methods
{
    None = 0,
    Xor = 1,
    Junk = 2
}

internal record Config(
    bool Encrypt = false,
    Methods Methods = Methods.None,
    int XorRounds = 3,
    int? Seed = null
)
{
    public bool Has(Methods m) => (Methods & m) != 0;
}
