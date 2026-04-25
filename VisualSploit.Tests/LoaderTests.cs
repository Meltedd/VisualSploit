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

    static Config Cfg(int? seed, bool junk) =>
        new(TargetPath: "/unused",
            ShellcodePath: "/unused",
            OutputPath: null,
            XorRounds: 3,
            Seed: seed,
            Junk: junk,
            NoBackup: true);

    [Theory]
    [InlineData(null, false)]
    [InlineData(42, false)]
    [InlineData(42, true)]
    [InlineData(1, true)]
    public void Generated_loader_parses_without_syntax_errors(int? seed, bool junk)
    {
        var source = Source(seed, junk);

        var ct = TestContext.Current.CancellationToken;
        var tree = CSharpSyntaxTree.ParseText(source, cancellationToken: ct);
        var errors = tree.GetDiagnostics(ct)
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        Assert.True(errors.Count == 0,
            $"Generated loader has {errors.Count} syntax error(s):\n" +
            string.Join("\n", errors.Select(e => $"  {e.Location.GetLineSpan().StartLinePosition}: {e.GetMessage()}")) +
            "\n\nSource:\n" + source);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(42, false)]
    [InlineData(42, true)]
    [InlineData(1, true)]
    public void Generated_loader_has_no_compile_diagnostics(int? seed, bool junk)
    {
        var ct = TestContext.Current.CancellationToken;
        var tree = CSharpSyntaxTree.ParseText(Source(seed, junk), cancellationToken: ct);

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
            string.Join("\n", errors.Select(e => $"  {e.Location.GetLineSpan().StartLinePosition}: {e.GetMessage()}")));
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(42, false)]
    [InlineData(42, true)]
    [InlineData(1, true)]
    public void Generated_DllImports_specify_EntryPoint(int? seed, bool junk)
    {
        var ct = TestContext.Current.CancellationToken;
        var tree = CSharpSyntaxTree.ParseText(Source(seed, junk), cancellationToken: ct);

        var imports = tree.GetRoot(ct)
            .DescendantNodes()
            .OfType<AttributeSyntax>()
            .Where(a => a.Name.ToString().EndsWith("DllImport"))
            .ToList();

        Assert.NotEmpty(imports);
        foreach (var attr in imports)
        {
            var hasEntryPoint = attr.ArgumentList?.Arguments
                .Any(a => a.NameEquals?.Name.Identifier.ValueText == "EntryPoint") == true;
            Assert.True(hasEntryPoint, $"DllImport missing EntryPoint: {attr}");
        }
    }

    static string Source(int? seed, bool junk)
    {
        var cfg = Cfg(seed, junk);
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
