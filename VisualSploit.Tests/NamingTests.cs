using System.Text.RegularExpressions;
using Xunit;

namespace VisualSploit.Tests;

public class NamingTests
{
    static readonly Regex IdentRegex = new(@"^[a-z]+$", RegexOptions.Compiled);

    [Fact]
    public void Produces_10k_unique_valid_identifiers()
    {
        var naming = new Naming(seed: 12345);
        var seen = new HashSet<string>();

        for (int i = 0; i < 10_000; i++)
        {
            var name = naming.Next();
            Assert.True(IdentRegex.IsMatch(name), $"Not a valid identifier: {name}");
            Assert.DoesNotContain(name, Naming.Reserved);
            Assert.True(seen.Add(name), $"Duplicate name at draw {i}: {name}");
        }
    }

    [Fact]
    public void Same_seed_produces_identical_sequence()
    {
        var a = new Naming(seed: 99);
        var b = new Naming(seed: 99);
        for (int i = 0; i < 100; i++)
            Assert.Equal(a.Next(), b.Next());
    }
}
