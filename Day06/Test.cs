
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
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day06/input.txt");
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day06/sample.txt");
        long result = CountPossibleWaysToWin(lines);
        Assert.Equal(288, result);
    }

    private long CountPossibleWaysToWin(string[] lines)
    {
        List<int> winTimes = new();
        var times = GetNumbers(lines[0]);
        var distances = GetNumbers(lines[1]);
        
        for(int i = 0; i < times.Count; i++)
        {
            int winMoves = 0;
            int speed = 0;

            for(int duration = 1; duration < times[i]; duration++)
            {
                speed++;
                if((times[i] - duration)*speed > distances[i])
                    winMoves++;
            }
            if(winMoves > 0)
                winTimes.Add(winMoves);
        }
        return winTimes.Aggregate((a, x) => a * x);
    }

    private List<long> GetNumbers(string line)
    {
        return _numbersRegExp.Matches(line).Select(x => Convert.ToInt64(x.ToString().Trim())).ToList();
    }
}