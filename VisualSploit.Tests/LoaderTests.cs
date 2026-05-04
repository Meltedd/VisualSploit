using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using Xunit.v3;

namespace VisualSploit.Tests;

public class LoaderTests
{
    static readonly byte[] Shellcode = { 0xfc, 0x48, 0x83, 0xe4, 0xf0, 0xe8, 0xcc, 0x00, 0x00, 0x00 };

    static readonly MetadataReference[] RuntimeRefs =
        ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToArray();

    static Config Cfg(int? seed, TargetPlatform platform = TargetPlatform.Windows) =>
        new(TargetPath: "/unused",
            ShellcodePath: "/unused",
            OutputPath: null,
            XorRounds: 3,
            Seed: seed,
            Platform: platform,
            ShellcodeFormat: ShellcodeFormat.Auto,
            Condition: null,
            NoBackup: true,
            DryRun: false,
            Verbose: false);

    [Theory]
    [InlineData(null, nameof(TargetPlatform.Windows))]
    [InlineData(42, nameof(TargetPlatform.Windows))]
    [InlineData(null, nameof(TargetPlatform.Linux))]
    [InlineData(42, nameof(TargetPlatform.Linux))]
    public void Generated_loader_has_no_compile_diagnostics(int? seed, string platformName)
    {
        var platform = Enum.Parse<TargetPlatform>(platformName);
        var ct = TestContext.Current.CancellationToken;
        var source = Source(seed, platform);
        var tree = CSharpSyntaxTree.ParseText(source, cancellationToken: ct);

        var compilation = CSharpCompilation.Create(
            assemblyName: "GeneratedLoader",
            syntaxTrees: [tree],
            references: RuntimeRefs,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var errors = compilation.GetDiagnostics(ct)
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        Assert.True(errors.Count == 0,
            $"Generated loader has {errors.Count} compile error(s):\n" +
            string.Join("\n", errors.Select(e => $"  {e.Location.GetLineSpan().StartLinePosition}: {e.GetMessage()}")) +
            "\n\nSource:\n" + source);
    }

    [Fact]
    public void Generated_windows_loader_imports_expected_symbols() =>
        AssertImports(TargetPlatform.Windows, "kernel32",
            ["VirtualAlloc", "CreateThread", "WaitForSingleObject"]);

    [Fact]
    public void Generated_linux_loader_imports_expected_symbols() =>
        AssertImports(TargetPlatform.Linux, "libc",
            ["mmap", "pthread_create", "pthread_join"]);

    [Fact]
    public void Generated_linux_loader_uses_linux_mmap_constants()
    {
        var source = Source(seed: 42, TargetPlatform.Linux);

        Assert.Contains("(System.UIntPtr)(uint)", source);
        Assert.Contains(", 7,", source);
        Assert.Contains(", 7, 0x22, -1,", source);
        Assert.Contains(", -1,", source);
    }

    static void AssertImports(TargetPlatform platform, string library, string[] entryPoints)
    {
        var ct = TestContext.Current.CancellationToken;
        var tree = CSharpSyntaxTree.ParseText(Source(seed: 42, platform), cancellationToken: ct);

        var imports = tree.GetRoot(ct)
            .DescendantNodes()
            .OfType<AttributeSyntax>()
            .Where(a => a.Name.ToString().EndsWith("DllImport"))
            .ToList();

        Assert.Equal(3, imports.Count);

        var seenEntryPoints = new List<string>();
        foreach (var attr in imports)
        {
            var args = attr.ArgumentList!.Arguments;

            var libArg = args[0];
            Assert.Equal(library, ((LiteralExpressionSyntax)libArg.Expression).Token.ValueText);

            var entryPointArg = args.Single(a =>
                a.NameEquals?.Name.Identifier.ValueText == "EntryPoint");
            seenEntryPoints.Add(((LiteralExpressionSyntax)entryPointArg.Expression).Token.ValueText);
        }

        Assert.Equal(entryPoints, seenEntryPoints);
    }

    static string Source(int? seed, TargetPlatform platform = TargetPlatform.Windows)
    {
        var cfg = Cfg(seed, platform);
        var naming = new Naming(cfg.Seed);
        var inlineCode = Loader.Generate(Shellcode, cfg, naming);

        return $$"""
            abstract class TaskBase
            {
                public abstract bool Execute();
            }

            class C : TaskBase
            {
            {{inlineCode}}
            }
            """;
    }
}
