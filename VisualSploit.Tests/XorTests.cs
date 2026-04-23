using System.Text.RegularExpressions;
using Xunit;

namespace VisualSploit.Tests;

public class XorTests
{
    static byte[] ApplyKeys(byte[] data, byte[][] keys)
    {
        var r = (byte[])data.Clone();
        for (int i = 0; i < keys.Length; i++)
            for (int j = 0; j < r.Length; j++)
                r[j] ^= keys[i][j % Xor.KeySize];
        return r;
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Roundtrips_for_n_rounds(int rounds)
    {
        var plaintext = new byte[] { 0xfc, 0x48, 0x83, 0xe4, 0xf0, 0xe8, 0xcc };
        var (ct, keys) = Xor.Encrypt(plaintext, rounds);
        Assert.Equal(rounds, keys.Length);
        Assert.Equal(plaintext, ApplyKeys(ct, keys));
    }

    [Fact]
    public void Seeded_runs_are_byte_identical()
    {
        var plaintext = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        var (ct1, keys1) = Xor.Encrypt(plaintext, 3, new Random(42));
        var (ct2, keys2) = Xor.Encrypt(plaintext, 3, new Random(42));

        Assert.Equal(ct1, ct2);
        for (int i = 0; i < keys1.Length; i++)
            Assert.Equal(keys1[i], keys2[i]);
    }

    [Fact]
    public void Routine_emits_keys_in_reverse_order()
    {
        var plaintext = new byte[] { 0x11, 0x22, 0x33 };
        var (ct, keys) = Xor.Encrypt(plaintext, 3, new Random(7));

        var routine = Xor.Routine("buf", ct, keys);

        var order = Regex.Matches(routine, @"buf_k(\d+)")
            .Select(m => int.Parse(m.Groups[1].Value))
            .Distinct()
            .ToList();

        Assert.Equal(new[] { 2, 1, 0 }, order);
    }

    [Fact]
    public void Routine_uses_buffer_name_in_declarations_and_loops()
    {
        var (ct, keys) = Xor.Encrypt(new byte[] { 0x00 }, 2, new Random(1));
        var routine = Xor.Routine("mybuf", ct, keys);

        Assert.Contains("var mybuf =", routine);
        Assert.Contains("var mybuf_k0", routine);
        Assert.Contains("var mybuf_k1", routine);
        Assert.Contains("mybuf_i0", routine);
        Assert.Contains("mybuf_i1", routine);
    }
}
