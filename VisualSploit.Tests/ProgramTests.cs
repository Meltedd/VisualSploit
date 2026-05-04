using Xunit;

namespace VisualSploit.Tests;

public class ProgramTests : IDisposable
{
    readonly string _dir;

    public ProgramTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), $"vs-cli-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); }
        catch { }
    }

    [Fact]
    public void Output_uses_windows_loader_by_default()
    {
        var output = RunWithOutputFile();

        Assert.Contains("kernel32", output);
        Assert.Contains("VirtualAlloc", output);
        Assert.Contains("CreateThread", output);
        Assert.Contains("WaitForSingleObject", output);
        Assert.DoesNotContain("pthread_create", output);
    }

    [Fact]
    public void Output_uses_linux_loader_when_platform_is_linux()
    {
        var output = RunWithOutputFile("--platform", "linux");

        Assert.Contains("libc", output);
        Assert.Contains("mmap", output);
        Assert.Contains("pthread_create", output);
        Assert.Contains("pthread_join", output);
        Assert.Contains(", 7, 0x22, -1, System.IntPtr.Zero);", output);
        Assert.DoesNotContain("kernel32", output);
    }

    [Fact]
    public void Shellcode_format_raw_keeps_hex_looking_input_as_bytes()
    {
        var autoOutput = RunWithOutputFile(shellcode: "C3"u8.ToArray());
        var rawOutput = RunWithOutputFile(shellcode: "C3"u8.ToArray(), "--shellcode-format", "raw");

        Assert.Equal(1, EmbeddedShellcodeByteCount(autoOutput));
        Assert.Equal(2, EmbeddedShellcodeByteCount(rawOutput));
    }

    [Fact]
    public void Output_includes_condition_when_configured()
    {
        var output = RunWithOutputFile("--condition", "'$(Configuration)' == 'Release'");

        Assert.Contains("Condition=\"'$(Configuration)' == 'Release'\"", output);
    }

    [Fact]
    public void Rejects_empty_condition()
    {
        var (target, shellcodePath) = WriteInputs([0xC3]);

        var exitCode = Program.Main([target, shellcodePath, "--condition", " "]);

        Assert.NotEqual(0, exitCode);
    }

    string RunWithOutputFile(params string[] extraArgs) =>
        RunWithOutputFile(shellcode: [0xC3], extraArgs);

    string RunWithOutputFile(byte[] shellcode, params string[] extraArgs)
    {
        var (target, shellcodePath) = WriteInputs(shellcode);
        var output = Path.Combine(_dir, $"{Guid.NewGuid():N}.csproj");

        var args = new List<string> { target, shellcodePath, "--output", output, "-s", "42" };
        args.AddRange(extraArgs);

        var exitCode = Program.Main(args.ToArray());
        Assert.True(exitCode == 0,
            $"Expected CLI to succeed. Exit code: {exitCode}. Args: {string.Join(" ", args)}. Output: {output}");

        return File.ReadAllText(output);
    }

    (string target, string shellcodePath) WriteInputs(byte[] shellcode)
    {
        var target = Path.Combine(_dir, $"{Guid.NewGuid():N}.csproj");
        var shellcodePath = Path.Combine(_dir, $"{Guid.NewGuid():N}.bin");

        File.WriteAllText(target, """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
            </Project>
            """);
        File.WriteAllBytes(shellcodePath, shellcode);

        return (target, shellcodePath);
    }

    static int EmbeddedShellcodeByteCount(string output)
    {
        var start = output.IndexOf("new byte[] { ", StringComparison.Ordinal);
        Assert.NotEqual(-1, start);
        start += "new byte[] { ".Length;

        var end = output.IndexOf(" }", start, StringComparison.Ordinal);
        Assert.NotEqual(-1, end);

        return output[start..end].Split(", ", StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
