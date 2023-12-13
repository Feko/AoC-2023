using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace AOC2023.Day13;

public class Test
{
    enum ReflectionType
    {
        Line,
        Column
    }

    [Fact]
    public void Part1()
    {
        var content = File.ReadAllText("C:\\DEV\\AoC-2023\\Day13\\input.txt"); int expected = 30575;
        //var content = File.ReadAllText("C:\\DEV\\AoC-2023\\Day13\\sample.txt"); int expected = 405;
        long result = GetReflectionsSummaries(content).Sum();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Part2()
    {
        // 38570 = Too high
        // 37478 = Bingo
        var content = File.ReadAllText("C:\\DEV\\AoC-2023\\Day13\\input.txt"); int expected = 30575;
        //var content = File.ReadAllText("C:\\DEV\\AoC-2023\\Day13\\sample.txt"); int expected = 400;
        long result = GetReflectionsSummaries(content, 1).Sum();
        Assert.Equal(expected, result);
    }

    private List<int> GetReflectionsSummaries(string content, int expectedDiffs = 0)
    {
        var patterns = content.Split("\n\n");
        var result = new List<int>();

        foreach (var pattern in patterns) 
        { 
            var reflection = GetReflectionPoint(pattern, expectedDiffs);
            result.Add(GetSummary(reflection));
        }
        return result;
    }

    private int GetSummary((int position, ReflectionType type) reflection)
    {
        return reflection.type == ReflectionType.Column
            ? reflection.position + 1
            : (reflection.position + 1) * 100;
    }

    private (int position, ReflectionType type) GetReflectionPoint(string pattern, int expectedDiffs = 0)
    {
        var lines = pattern.Split("\n");

        // Is it a line mirror?
        for (int i = 0; i < lines.Length - 1; i++)
        {
            int diffs = GetLineDifferences(lines, i, i + 1);
            if (diffs <= expectedDiffs)
                if (ProbeLineReflection(lines, i, expectedDiffs - diffs))
                    return (i, ReflectionType.Line);
        }

        // Is it a column reflection?
        for (int i = 0; i < lines[0].Length - 1; i++)
        {
            int diffs = GetColumnDifferences(lines, i, i + 1);
            if (diffs <= expectedDiffs)
            {
                if (ProbeColumnReflection(lines, i, expectedDiffs - diffs))
                    return (i, ReflectionType.Column);
            }                
        }
        throw new Exception("No reflections found");
    }

    private bool ProbeColumnReflection(string[] lines, int i, int expectedDiffs = 0)
    {
        if ((i == 0 || i == lines[0].Length -2) && expectedDiffs == 0)
            return true;
        int columnLeft = i - 1;
        int columnRight = i + 2;
        int totalDiffs = 0;

        while (columnLeft >= 0 && columnRight < lines[0].Length)
        {
            var diffs = GetColumnDifferences(lines, columnLeft, columnRight);
            if (diffs > expectedDiffs)
                return false;
            totalDiffs += diffs;
            columnLeft--;
            columnRight++;
        }
        return totalDiffs == expectedDiffs;
    }

    private bool ProbeLineReflection(string[] lines, int i, int expectedDiffs = 0)
    {
        if ((i == 0 || i == lines.Length -2) && expectedDiffs == 0)
            return true;
        int lineUp = i - 1;
        int lineDown = i + 2;
        int totalDiffs = 0;

        while (lineUp >= 0 && lineDown < lines.Length)
        {
            int diffs = GetLineDifferences(lines, lineUp, lineDown);
            if (diffs > expectedDiffs)
                return false;
            totalDiffs += diffs;
            lineUp--;
            lineDown++;
        }
        return totalDiffs == expectedDiffs;
    }

    private int GetColumnDifferences(string[] lines, int columnA, int columnB)
    {
        int differences = 0;
        foreach (var line in lines)
        {
            if (line[columnA] != line[columnB])
                differences++;
        }
        return differences;
    }

    private int GetLineDifferences(string[] lines, int lineA, int lineB)
    {
        int differences = 0;
        for (int i = 0; i < lines[lineA].Length; i++)
        {
            if (lines[lineA][i] != lines[lineB][i])
                differences++;
        }
      
        return differences;
    }
}

