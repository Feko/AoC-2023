
using System.Collections.Concurrent;
using System.Data;
using System.Text.RegularExpressions;

namespace AOC2023.Day06;

public class Test
{   
    Regex _numbersRegExp = new Regex(@"[0-9]+", RegexOptions.Compiled); 

    [Fact]
    public void Part1()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day06/input.txt");
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day06/sample.txt");
        long result = CountPossibleWaysToWin(lines);
        Assert.Equal(288, result);
    }

    [Fact]
    public void Part2()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day06/input.txt");
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day06/sample.txt");
        long result = CountSingleRace(lines);
        Assert.Equal(71503, result);
    }

    private long CountSingleRace(string[] lines)
    {
        var time = Convert.ToInt64(lines[0].Split(':').Last().Replace(" ", ""));
        var distance = Convert.ToInt64(lines[1].Split(':').Last().Replace(" ", ""));

        return CountMoves(time, distance);
    }

    private long CountPossibleWaysToWin(string[] lines)
    {
        List<long> winTimes = new();
        var times = GetNumbers(lines[0]);
        var distances = GetNumbers(lines[1]);
        
        for(int i = 0; i < times.Count; i++)
        {
            long winMoves = CountMoves(times[i], distances[i]);
            
            if(winMoves > 0)
                winTimes.Add(winMoves);
        }
        return winTimes.Aggregate((a, x) => a * x);
    }

    private long CountMoves(long time, long distance)
    {
        long winMoves = 0;
        int speed = 0;

        for(int duration = 1; duration < time; duration++)
        {
            speed++;
            if((time - duration)*speed > distance)
                winMoves++;
        }
        return winMoves;
    }

    private List<long> GetNumbers(string line)
    {
        return _numbersRegExp.Matches(line).Select(x => Convert.ToInt64(x.ToString().Trim())).ToList();
    }
}