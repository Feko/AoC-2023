
namespace AOC2023.Day22;

public class Test
{   
    [Fact]
    public void Part1()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day22/input.txt"); int expected = 416;
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day22/sample.txt"); int expected = 5;
        
        List<Brick> bricks = BuildBricks(lines);
        BringDownThemBricks(bricks);
        var dataset = BuildSupportationDataset(bricks);

        int amount = CountBricksForElimination(dataset);

        Assert.Equal(expected, amount);
    }

    [Fact]
    public void Part2()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day22/input.txt"); int expected = 416;
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day22/sample.txt"); int expected = 7;
        
        List<Brick> bricks = BuildBricks(lines);
        BringDownThemBricks(bricks);

        long amount = CountChainReactionElimination(bricks);

        Assert.Equal(expected, amount);
    }

    private long CountChainReactionElimination(List<Brick> bricks)
    {
        bricks = bricks.OrderBy(x => x.MinAxis(Axis.ZStretch)).ToList();
        long total = 0;
        foreach(var eliminated in bricks)
        {
            var remaining = bricks.Where(b => b != eliminated)
                .Select(b => b.Clone()).OrderBy(x => x.MinAxis(Axis.ZStretch)).ToList();

            foreach(var brick in remaining)
            {
                bool wentDown = brick.GoDown(remaining);
                if(wentDown)
                    total++;
            }

        }
        return total;
    }

    private int CountBricksForElimination(Dictionary<Brick, (List<Brick> supportedBy, List<Brick> supports)> dataset)
    {
        HashSet<Brick> toEliminate = new();

        var wellSupportedBricks = dataset.Where(x => x.Value.supportedBy.Count > 1);
        foreach(var wellSupported in wellSupportedBricks)
        {
            foreach(var supporter in wellSupported.Value.supportedBy)
            {
                if(dataset[supporter].supports.All(b => dataset[b].supportedBy.Count > 1)) // All the bricks this supporter supports are well supported
                    toEliminate.Add(supporter);
            }
        }

        var notSupportAnyone = dataset.Where(x => x.Value.supports.Count == 0);
        foreach(var notSupporter in notSupportAnyone)
            toEliminate.Add(notSupporter.Key);

        return toEliminate.Count;
    }

    private Dictionary<Brick, (List<Brick> supportedBy, List<Brick> supports)> BuildSupportationDataset(List<Brick> bricks)
    {
        // Dict with a brick as key, and the bricks that supports it as value
        Dictionary<Brick, (List<Brick> supportedBy, List<Brick> supports)> result = new(bricks.Count);

        foreach(var brick in bricks)
            result[brick] = (bricks.Where(b => b.Supports(brick)).ToList(), bricks.Where(b => brick.Supports(b)).ToList());

        return result;
    }

    private void BringDownThemBricks(List<Brick> bricks)
    {
        bool shouldContinue = true;
        while(shouldContinue)
        {
            shouldContinue = false;
            var sortedBricks = bricks.OrderBy(x => x.MinAxis(Axis.ZStretch)).ToList();
            foreach(var brick in sortedBricks)
            {
                bool wentDown = brick.GoDown(bricks);
                shouldContinue = shouldContinue || wentDown;
            }
        }
    }

    private List<Brick> BuildBricks(string[] lines) => lines.Select(l => new Brick(l)).ToList();
}