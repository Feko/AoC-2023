
namespace AOC2023.Day08;

public class Test
{   
    static string moves = string.Empty;
    private int moveCounter = 0;

    [Fact]
    public void Part1()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day08/input.txt");
        // var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day08/sample.txt");
        moves = lines.First();
        var graph = Parse(lines);
        long result = GoFromTo("AAA", "ZZZ", graph);
        Assert.Equal(6, result);
    }

    private long GoFromTo(string from, string to, Dictionary<string, (string left, string right)> graph)
    {
        string current = from;
        while(current != to)
        {
            char nextMove = moves[moveCounter++ % moves.Length];
            current = nextMove == 'L' ? graph[current].left : graph[current].right;
        }

        return moveCounter;
    }

    private Dictionary<string, (string left, string right)> Parse(string[] lines)
    {
        Dictionary<string, (string, string)> result = new(lines.Length -2);
        for(int i = 2; i < lines.Length; i++)
        {
            var line = lines[i].Replace(" ","").Replace("(","").Replace(")","");
            var parts = line.Split('=');
            var directions = parts[1].Split(',');
            result[parts[0]] = (directions[0], directions[1]);
        }
        return result;
    }
}