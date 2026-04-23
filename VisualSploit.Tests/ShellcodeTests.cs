using Xunit;

namespace VisualSploit.Tests;

public class ShellcodeTests : IDisposable
{
    readonly string _dir;

    public ShellcodeTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), $"vs-sc-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); }
        catch { }
    }

    string Write(string content)
    {
        var path = Path.Combine(_dir, $"{Guid.NewGuid():N}.txt");
        File.WriteAllText(path, content);
        return path;
    }

    string WriteBytes(byte[] content)
    {
        var path = Path.Combine(_dir, $"{Guid.NewGuid():N}.bin");
        File.WriteAllBytes(path, content);
        return path;
    }

    [Fact]
    public void Parses_0x_prefixed_hex() =>
        Assert.Equal(new byte[] { 0xde, 0xad, 0xbe, 0xef },
            Shellcode.Parse(Write("0xde, 0xad, 0xbe, 0xef")));

    [Fact]
    public void Parses_comma_separated_hex() =>
        Assert.Equal(new byte[] { 0xde, 0xad, 0xbe, 0xef },
            Shellcode.Parse(Write("de,ad,be,ef")));

    [Fact]
    public void Parses_plain_hex_string() =>
        Assert.Equal(new byte[] { 0xde, 0xad, 0xbe, 0xef },
            Shellcode.Parse(Write("deadbeef")));

    [Fact]
    public void Handles_whitespace_and_newlines_in_hex() =>
        Assert.Equal(new byte[] { 0xde, 0xad, 0xbe, 0xef },
            Shellcode.Parse(Write("de ad\nbe\tef\r\n")));

    [Fact]
    public void Falls_back_to_raw_bytes_when_not_hex()
    {
        var raw = new byte[] { 0x90, 0xFF, 0x00, 0xAB, 0xCD };
        Assert.Equal(raw, Shellcode.Parse(WriteBytes(raw)));
    }

    [Fact]
    public void Throws_when_file_missing() =>
        Assert.Throws<FileNotFoundException>(() =>
            Shellcode.Parse(Path.Combine(_dir, $"missing-{Guid.NewGuid():N}")));

    [Fact]
    public void Parses_hex_with_leading_utf8_bom() =>
        Assert.Equal(new byte[] { 0xde, 0xad, 0xbe, 0xef },
            Shellcode.Parse(WriteBytes(
                new byte[] { 0xEF, 0xBB, 0xBF, 0x64, 0x65, 0x61, 0x64, 0x62, 0x65, 0x65, 0x66 })));

    [Fact]
    public void Throws_when_file_empty() =>
        Assert.Throws<InvalidDataException>(() => Shellcode.Parse(WriteBytes(Array.Empty<byte>())));

    [Fact]
    public void Treats_binary_with_high_bit_as_raw()
    {
        var raw = new byte[] { 0xfc, 0x48, 0x83, 0xe4, 0xf0, 0xe8 };
        Assert.Equal(raw, Shellcode.Parse(WriteBytes(raw)));
    }

    [Fact]
    public void Treats_binary_with_control_bytes_as_raw()
    {
        var raw = new byte[] { 0x00, 0x0B, 0x7F, 0xA1 };
        Assert.Equal(raw, Shellcode.Parse(WriteBytes(raw)));
    }

    [Fact]
    public void Throws_on_malformed_hex_text() =>
        Assert.Throws<InvalidDataException>(() => Shellcode.Parse(Write("0x")));
}
