using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Xunit.v3;

namespace VisualSploit.Tests;

public class LoaderTests
{
    static readonly byte[] Shellcode = { 0xfc, 0x48, 0x83, 0xe4, 0xf0, 0xe8, 0xcc, 0x00, 0x00, 0x00 };

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
        var cfg = Cfg(seed, junk);
        var naming = new Naming(cfg.Seed);
        var inlineCode = Loader.Generate(Shellcode, cfg, naming);

        var source = $$"""
            class C
            {
            {{inlineCode}}
            }
            """;

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
}
