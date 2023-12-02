
using Newtonsoft.Json.Linq;

namespace AOC2023.Day02;

public class Test
{
    [Fact]
    public void Part1()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day02/sample.txt");
        // var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day02/input.txt");
        Dictionary<string, int> constraints = new()
        {
            {"red", 12},
            {"green", 13},
            {"blue", 14},
        };

        int result = lines.Select(x => IdGamePossible(x, constraints)).Sum();
        Assert.Equal(8, result);
    }

    [Fact]
    public void Part2()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day02/sample.txt");
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day02/input.txt");
       
        int result = lines.Select(x => FindGamePower(x)).Sum();
        Assert.Equal(2286, result);
    }

   
    private int IdGamePossible(string line, Dictionary<string, int> constraints)
    {
        var parts = line.Split(':');
        int gameId = Convert.ToInt32(parts.First().Split(' ').Last());
        foreach(var game in parts.Last().Split(';'))
        {
            foreach(var cubeset in game.Split(','))
            {
                var valuepair = cubeset.Trim().Split(' ');
                if(constraints[valuepair.Last()] < Convert.ToInt32(valuepair.First()))
                    return 0;
            }
        }
        return gameId;
    }

    private int FindGamePower(string line)
    {
        Dictionary<string, int> requiredAmounts = new()
        {
            {"red", 0},
            {"green", 0},
            {"blue", 0},
        };
        foreach(var game in line.Split(':').Last().Split(';'))
        {
            foreach(var cubeset in game.Split(','))
            {
                var valuepair = cubeset.Trim().Split(' ');
                requiredAmounts[valuepair.Last()] = Math.Max(requiredAmounts[valuepair.Last()], Convert.ToInt32(valuepair.First()));
            }
        }
        return requiredAmounts.Values.Aggregate(1, (x,y) => x * y);
    }

}