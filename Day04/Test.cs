
using System.Data;
using System.Text.RegularExpressions;

namespace AOC2023.Day04;

public class Test
{   
    Regex _numbersRegExp = new Regex(@"([\s]{1}[0-9]{2}|[\s]{2}[0-9]{1})", RegexOptions.Compiled); 

    [Fact]
    public void Part1()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day04/sample.txt");
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day04/input.txt");
        int result = lines.Select(x => GetCardScore(x)).Sum();
        Assert.Equal(13, result);
    }

    private int GetCardScore(string line)
    {
        var parts = line.Split(" |");
        var luckyNumbers = _numbersRegExp.Matches(parts[0].Split(':').Last()).Select(x => Convert.ToInt32(x.ToString().Trim())).ToList();
        var myNumbers = _numbersRegExp.Matches(parts[1]).Select(x => Convert.ToInt32(x.ToString().Trim())).ToList();
        int result = 1 << (myNumbers.Intersect(luckyNumbers).Count());
        return result >> 1;
    }
}