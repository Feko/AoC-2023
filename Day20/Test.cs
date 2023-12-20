using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace AOC2023.Day20;

public partial class Test
{
    public enum SignalStrength { Low, High }
    public record Signal(string From, string To, SignalStrength Strength);

    [Fact]
    public void Part1()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day20/sample1.txt").ToList(); long expected = 32000000;
        //var lines = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day20\\sample2.txt").ToList(); long expected = 11687500;
        //var lines = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day20\\input.txt").ToList(); long expected = 11687500;
        var context = new MeshContext();
        PushButton(context, lines, 1000);
        Assert.Equal(expected, context.FactorSignals());
    }

    [Fact]
    public void Part2()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day20/input.txt").ToList(); 
        var context = new MeshContext();
        long result = PushButtonWithCallback(context, lines);
        Assert.Equal(231657829136023, result);
    }

    private long PushButtonWithCallback(MeshContext context, List<string> lines)
    {
        // Brute-forced for 5 hours, up to 3B with no luck
        // Inspecting my visual aid, I can see that it will only receive a low pulse if all the Conjunction above of "lg" output HIGH simultaneously.
        // Let's try something else

        context.Initialize(lines);
        int pushAmount = 0;

        // Get module name above RX
        var moduleName = lines.First(l => l.IndexOf(" -> rx") > 0).Split(" -> ")[0].Substring(1);

        // What we really need is the modules ABOVE the one above RX ("vc", "nb", "ls" and "vg" in my case)
        var aboveRx = (context.GetModule(moduleName) as Conjunction);
        var conjunctionKeys = aboveRx.Memo.Keys.ToList();

        
        Dictionary<string, List<int>> loopConjunctions = conjunctionKeys.ToDictionary(k => k, v => new List<int>());

        // Add some callbacks to find out WHEN those four modules actually send a "High" output.
        foreach (var c in conjunctionKeys)
        {
            context.ProcessSendCallback[c] = signal =>
            {
                if (signal.Strength == SignalStrength.High)
                {
                    loopConjunctions[signal.From].Add(pushAmount);
                }
            };
        }

        // Let it run for a while()
        for (int i = 0; i < 10_000; i++)
        {
            pushAmount++;
            context.PushButton();
            context.ProcessSignals();
        }

        // Factor the results - All prime numbers, so no need for LCM
        var loopSizes = loopConjunctions.Values.Select(x => x.First()).ToList();

        long result = 1;
        foreach (var d in loopSizes)
            result *= d;

        return result;
    }

    private void PushButton(MeshContext context, List<string> lines, int amount)
    {
        context.Initialize(lines);
        for (int i = 0; i < amount; i++)
        {
            context.PushButton();
            context.ProcessSignals();
        }
    }
}
