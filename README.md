# VisualSploit

Weaponizes an MSBuild project file to run embedded shellcode. Supply a `.csproj`, `.vbproj`, or `Directory.Build.props/targets` and a shellcode blob. The shellcode runs whenever someone builds, restores, or opens the project in Visual Studio.

## How it works

MSBuild lets a project declare an [inline task](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-roslyncodetaskfactory): a chunk of C# that `RoslynCodeTaskFactory` compiles and runs during the build. The `InitialTargets` attribute on `<Project>` names targets that fire before anything else when MSBuild evaluates the project. Per Microsoft's guidance, design-time builds are equivalent to full execution.

VisualSploit writes a `<UsingTask>` holding a self-decrypting shellcode loader, adds a `<Target>` that invokes it, and appends that target to `InitialTargets`. Any evaluation of the project triggers it: `dotnet build`, `dotnet restore`, Visual Studio opening the folder, or any IDE that runs MSBuild for IntelliSense. The emitted C# XOR-decrypts the embedded payload, allocates an RWX page through a dynamically resolved `VirtualAlloc`, and hands control to the shellcode via `CallWindowProcA`.

Cloned files carry no [MOTW](https://learn.microsoft.com/en-us/windows/win32/secauthz/mark-of-the-web), so Visual Studio's "trust this project?" prompt never fires on `git clone`. Cloning a backdoored repo and opening it in VS is enough to run the payload.

## Trigger surfaces

| Target file                 | Fires when                                               |
|-----------------------------|----------------------------------------------------------|
| `*.csproj` / `*.vbproj`     | The project is opened or built                           |
| `Directory.Build.props`     | Any project in the directory or below is opened or built |
| `Directory.Build.targets`   | Any project in the directory or below is built           |

`Directory.Build.props` and `.targets` are imported implicitly for every project beneath them, so a single injected file at a repo root compromises the whole subtree.

## Usage

```
visualsploit <target> <shellcode> [options]

-o, --output <path>   Write to a different path (default: in-place)
-r, --rounds <n>      XOR rounds 1-5 (default 3)
-s, --seed <n>        RNG seed for reproducible output
    --junk            Interleave junk code to vary emitted bytes
    --no-backup       Skip .bak when writing over an existing file
```

Shellcode input accepts raw binary, `0x`-prefixed hex, comma-separated hex, or plain hex (whitespace ignored). The target is modified in place unless `--output` is passed. A `.bak` of whatever was overwritten is left alongside it.

```bash
visualsploit project.csproj shellcode.bin
visualsploit repo/Directory.Build.props shellcode.bin
visualsploit repo/Directory.Build.targets shellcode.bin --junk -s 42
```

## Shellcode constraints

- Bitness must match the MSBuild host. Use x64 shellcode for x64 MSBuild, x86 for x86.
- Must be position-independent. The loader allocates a page at a system-chosen address and calls into it through `CallWindowProcA`, which invokes the shellcode as `WNDPROC(hWnd=0, Msg=0, wParam=0, lParam=0)`.
- The page is mapped `PAGE_EXECUTE_READWRITE`, so self-modifying stagers like reflective loaders or metasploit `migrate` run without extra protection flips.

## Detection surface

Execution stays inside MSBuild. The parent is `devenv.exe` or `dotnet.exe`/`MSBuild.exe`, which loads the inline task assembly and invokes it without spawning a child.

`VirtualAlloc` and `CallWindowProcA` are resolved at runtime via `LoadLibrary` and `GetProcAddress`, so neither appears as a static P/Invoke declaration. API name strings appear inline; earlier char-array wrappers were dropped since Roslyn const-folds them. Module names, delegate types, variable names, and the task and target names are drawn from a seeded RNG and differ per run unless `--seed` is passed.

## Build

```bash
dotnet build -c Release
dotnet test
```

Self-contained single-file build:

```bash
dotnet publish -c Release -r <rid> --self-contained -p:PublishSingleFile=true
```

Requires .NET 10 SDK.

## License

MIT.
