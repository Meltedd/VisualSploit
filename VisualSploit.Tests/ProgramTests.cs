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

    string RunWithOutputFile(params string[] extraArgs)
    {
        var target = Path.Combine(_dir, $"{Guid.NewGuid():N}.csproj");
        var shellcode = Path.Combine(_dir, $"{Guid.NewGuid():N}.bin");
        var output = Path.Combine(_dir, $"{Guid.NewGuid():N}.csproj");

        File.WriteAllText(target, """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
            </Project>
            """);
        File.WriteAllBytes(shellcode, [0xC3]);

        var args = new List<string> { target, shellcode, "--output", output, "-s", "42" };
        args.AddRange(extraArgs);

        var exitCode = Program.Main(args.ToArray());
        Assert.True(exitCode == 0,
            $"Expected CLI to succeed. Exit code: {exitCode}. Args: {string.Join(" ", args)}. Output: {output}");

        return File.ReadAllText(output);
    }
}
