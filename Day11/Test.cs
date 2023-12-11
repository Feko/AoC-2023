

namespace AOC2023.Day11;

public class Test
{
    [Fact]
    public void Part1()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day11/input.txt");
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day11/sample.txt");
        var constelations = GetConstelations(lines);
        var pairs = GeneratePairs(constelations);
        long result = GetDistances(pairs).Sum();
        Assert.Equal(374, result);
    }

    [Fact]
    public void Part2()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day11/input.txt"); int expandFactor = 1_000_000;
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day11/sample.txt"); int expandFactor = 10;
        var constelations = GetConstelations(lines, expandFactor);
        var pairs = GeneratePairs(constelations);
        long result = GetDistances(pairs).Sum();
        Assert.Equal(1030, result); 
    }

    private List<long> GetDistances(List<((int row, int column) constelationA, (int row, int column) constelationB)> pairs)
    {
        List<long> result = new();
        foreach (var pair in pairs)
        {
            long distance = Math.Abs(pair.constelationA.row - pair.constelationB.row) + Math.Abs(pair.constelationA.column - pair.constelationB.column);
            result.Add(distance);
        }
        return result;
    }

    private List<((int row, int column) constelationA, (int row, int column) constelationB)> GeneratePairs(List<(int row, int column)> constelations)
    {
        var result = new List<((int row, int column) constelationA, (int row, int column) constelationB)>();

        for (int i = 0; i < constelations.Count(); i++)
        {
            for (int j = i + 1; j < constelations.Count(); j++)
                result.Add((constelations[i], constelations[j]));
        }
        return result;
    }

    private List<(int row, int column)> GetConstelations(string[] lines, int expandFactor = 1)
    {
        List<(int row, int column)> result = new();
        int[] rowDistances = new int[lines.Length];
        int[] columnDistances = new int[lines[0].Length];

        Expand(rowDistances, columnDistances, lines, expandFactor);
        for (int line = 0; line < lines.Length; line++)
        {
            for (int column = 0; column < lines[0].Length; column++)
            {
                if (lines[line][column] == '#')
                    result.Add((rowDistances[line], columnDistances[column]));
            }
        }

        return result;
    }

    private static void Expand(int[] rowDistances, int[] columnDistances, string[] lines, int expandFactor)
    {
        for (int i = 0; i < rowDistances.Length; i++)
            rowDistances[i] = i;

        for (int i = 0; i < columnDistances.Length; i++)
            columnDistances[i] = i;

        expandFactor = expandFactor > 1 ? expandFactor - 1: 1;
        // Expand rows if empty
        for (int line = 0; line < rowDistances.Length; line++)
        {
            if (lines[line].IndexOf('#') >= 0)
                continue;

            for (int i = line; i < rowDistances.Length; i++)
            {
                rowDistances[i] = rowDistances[i] + expandFactor;
            }
        }

        // Expand columns if empty
        for (int column = 0; column < columnDistances.Length; column++)
        {
            if (lines.Any(line => line[column] == '#'))
                continue;

            for (int i = column; i < columnDistances.Length; i++)
            {
                columnDistances[i] = columnDistances[i] + expandFactor;
            }
        }
    }
}

