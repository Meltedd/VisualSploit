using System.Xml.Linq;
using Xunit;

namespace VisualSploit.Tests;

public class MSBuildTests : IDisposable
{
    readonly string _dir;

    public MSBuildTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), $"vs-msb-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); }
        catch { }
    }

    Config Cfg(string targetPath, int? seed = 42, string? output = null, bool noBackup = true) =>
        new(TargetPath: targetPath,
            ShellcodePath: "/unused",
            OutputPath: output,
            XorRounds: 3,
            Seed: seed,
            Junk: false,
            NoBackup: noBackup);

    const string Inline = "public override bool Execute() { return true; }";

    void Inject(Config cfg) => MSBuild.Inject(Inline, cfg, new Naming(cfg.Seed));

    [Fact]
    public void Synthesises_new_file_with_empty_namespace()
    {
        var target = Path.Combine(_dir, "Directory.Build.props");
        Inject(Cfg(target));

        var raw = File.ReadAllText(target);
        Assert.DoesNotContain("xmlns=", raw);

        var doc = XDocument.Load(target);
        Assert.Equal("Project", doc.Root!.Name.LocalName);
        Assert.Equal("", doc.Root.Name.NamespaceName);
    }

    [Fact]
    public void Merges_into_existing_csproj_preserving_property_and_item_groups()
    {
        var target = Path.Combine(_dir, "Existing.csproj");
        File.WriteAllText(target, """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
                <AssemblyName>Keep</AssemblyName>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="13.0.0" />
              </ItemGroup>
            </Project>
            """);

        Inject(Cfg(target));

        var doc = XDocument.Load(target);
        var root = doc.Root!;

        Assert.NotNull(root.Elements().FirstOrDefault(e => e.Name.LocalName == "PropertyGroup"));
        Assert.Equal("Keep",
            root.Descendants().First(e => e.Name.LocalName == "AssemblyName").Value);
        Assert.NotNull(root.Descendants().FirstOrDefault(e =>
            e.Name.LocalName == "PackageReference" &&
            e.Attribute("Include")?.Value == "Newtonsoft.Json"));

        Assert.Contains(root.Elements(), e => e.Name.LocalName == "UsingTask");
        Assert.Contains(root.Elements(), e => e.Name.LocalName == "Target");
    }

    [Fact]
    public void Appends_generated_target_after_existing_InitialTargets()
    {
        var target = Path.Combine(_dir, "Ordered.csproj");
        File.WriteAllText(target, """
            <Project Sdk="Microsoft.NET.Sdk" InitialTargets="Restore;Build">
              <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
            </Project>
            """);

        Inject(Cfg(target));

        var doc = XDocument.Load(target);
        var initial = doc.Root!.Attribute("InitialTargets")!.Value;
        var parts = initial.Split(';', StringSplitOptions.RemoveEmptyEntries);

        Assert.StartsWith("Restore;Build;", initial);
        Assert.Equal(3, parts.Length);
    }

    [Fact]
    public void Does_not_write_utf8_bom()
    {
        var target = Path.Combine(_dir, "Directory.Build.props");
        Inject(Cfg(target));

        var bytes = File.ReadAllBytes(target);
        Assert.True(bytes.Length >= 3);
        Assert.False(bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF,
            "Output file should not begin with a UTF-8 BOM");
    }

    [Fact]
    public void Creates_bak_when_output_points_to_existing_file()
    {
        var target = Path.Combine(_dir, "Source.csproj");
        var output = Path.Combine(_dir, "Victim.csproj");
        File.WriteAllText(target, """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
            </Project>
            """);
        var originalVictim = "<Project><PropertyGroup><AssemblyName>Victim</AssemblyName></PropertyGroup></Project>";
        File.WriteAllText(output, originalVictim);

        Inject(Cfg(target, output: output, noBackup: false));

        var bak = $"{output}.bak";
        Assert.True(File.Exists(bak), $"Expected {bak} to exist");
        Assert.Equal(originalVictim, File.ReadAllText(bak));
    }

    [Fact]
    public void Throws_when_csproj_target_does_not_exist()
    {
        var target = Path.Combine(_dir, "missing.csproj");
        Assert.Throws<FileNotFoundException>(() => Inject(Cfg(target)));
    }

    [Fact]
    public void Throws_when_existing_target_root_is_not_Project()
    {
        var target = Path.Combine(_dir, "Invalid.csproj");
        var original = "<NotProject><PropertyGroup /></NotProject>";
        File.WriteAllText(target, original);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            Inject(Cfg(target, noBackup: false)));

        Assert.Contains("root must be <Project>", ex.Message);
        Assert.Equal(original, File.ReadAllText(target));
        Assert.False(File.Exists($"{target}.bak"));
    }

    [Fact]
    public void Synthesises_new_Directory_Build_targets()
    {
        var target = Path.Combine(_dir, "Directory.Build.targets");
        Inject(Cfg(target));

        var doc = XDocument.Load(target);
        Assert.Equal("Project", doc.Root!.Name.LocalName);
        Assert.Contains(doc.Root.Elements(), e => e.Name.LocalName == "UsingTask");
        Assert.Contains(doc.Root.Elements(), e => e.Name.LocalName == "Target");
    }

    [Fact]
    public void Allows_repeated_injection()
    {
        var target = Path.Combine(_dir, "Already.csproj");
        File.WriteAllText(target, """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
            </Project>
            """);

        Inject(Cfg(target, seed: 42));
        Inject(Cfg(target, seed: 43));

        var doc = XDocument.Load(target);
        var root = doc.Root!;

        var taskNames = root.Elements()
            .Where(e => e.Name.LocalName == "UsingTask")
            .Select(e => e.Attribute("TaskName")!.Value)
            .ToArray();
        var targetNames = root.Elements()
            .Where(e => e.Name.LocalName == "Target")
            .Select(e => e.Attribute("Name")!.Value)
            .ToArray();

        Assert.Equal(2, taskNames.Length);
        Assert.Equal(2, taskNames.Distinct().Count());
        Assert.Equal(2, targetNames.Length);
        Assert.Equal(2, targetNames.Distinct().Count());

        var initialTargets = root.Attribute("InitialTargets")!.Value
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Assert.Contains(targetNames[0], initialTargets);
        Assert.Contains(targetNames[1], initialTargets);
    }

    [Fact]
    public void Allows_existing_RoslynCodeTaskFactory_task()
    {
        var target = Path.Combine(_dir, "InlineTask.csproj");
        File.WriteAllText(target, """
            <Project Sdk="Microsoft.NET.Sdk">
              <UsingTask TaskName="Existing" TaskFactory="RoslynCodeTaskFactory" AssemblyFile="Existing.dll">
                <Task />
              </UsingTask>
            </Project>
            """);

        Inject(Cfg(target));

        var doc = XDocument.Load(target);
        var taskNames = doc.Root!.Elements()
            .Where(e => e.Name.LocalName == "UsingTask")
            .Select(e => e.Attribute("TaskName")!.Value)
            .ToArray();

        Assert.Equal(2, taskNames.Length);
        Assert.Contains("Existing", taskNames);
    }

    [Fact]
    public void Preserves_existing_bak_across_repeated_injection()
    {
        var target = Path.Combine(_dir, "Stable.csproj");
        var original = """<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup /></Project>""";
        File.WriteAllText(target, original);

        Inject(Cfg(target, seed: 42, noBackup: false));
        var firstBak = File.ReadAllText($"{target}.bak");

        Inject(Cfg(target, seed: 43, noBackup: false));
        var secondBak = File.ReadAllText($"{target}.bak");

        Assert.Equal(original, firstBak);
        Assert.Equal(original, secondBak);
    }

    [Fact]
    public void Emitted_UsingTask_carries_expected_attributes()
    {
        var target = Path.Combine(_dir, "Directory.Build.props");
        Inject(Cfg(target));

        var doc = XDocument.Load(target);
        var usingTask = doc.Root!.Elements().Single(e => e.Name.LocalName == "UsingTask");

        Assert.Equal("RoslynCodeTaskFactory", usingTask.Attribute("TaskFactory")?.Value);
        Assert.Equal(@"$(MSBuildToolsPath)\Microsoft.Build.Tasks.Core.dll",
            usingTask.Attribute("AssemblyFile")?.Value);
        Assert.NotNull(usingTask.Attribute("TaskName")?.Value);
    }

    [Fact]
    public void Injects_namespaced_elements_into_namespaced_project()
    {
        var target = Path.Combine(_dir, "Namespaced.csproj");
        File.WriteAllText(target, """
            <Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <PropertyGroup />
            </Project>
            """);

        Inject(Cfg(target));

        var doc = XDocument.Load(target);
        var ns = doc.Root!.Name.Namespace;

        Assert.Contains(doc.Root.Elements(), e =>
            e.Name == ns + "UsingTask");
        Assert.Contains(doc.Root.Elements(), e =>
            e.Name == ns + "Target");
    }

    [Fact]
    public void Shared_naming_between_Loader_and_MSBuild_produces_deterministic_output()
    {
        var shellcode = new byte[] { 0xfc, 0x48, 0x83, 0xe4, 0xf0, 0xe8 };

        byte[] Run(string subdir)
        {
            var dir = Path.Combine(_dir, subdir);
            Directory.CreateDirectory(dir);
            var target = Path.Combine(dir, "Directory.Build.props");
            var cfg = Cfg(target, seed: 123);
            var naming = new Naming(cfg.Seed);
            var inline = Loader.Generate(shellcode, cfg, naming);
            MSBuild.Inject(inline, cfg, naming);
            return File.ReadAllBytes(target);
        }

        Assert.Equal(Run("first"), Run("second"));
    }
}
