using System.Data;
using System.Text.RegularExpressions;

namespace AOC2023.Day09;

public class Test
{   
    Regex _numbersRegExp = new Regex(@"-?[0-9]+", RegexOptions.Compiled); 

    [Fact]
    public void Part1()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day09/sample.txt");
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day09/input.txt");
        long result = lines.Select(l => PredictNext(l)).Sum();
        Assert.Equal(114, result);
    }

    [Fact]
    public void Part2()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day09/sample.txt");
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day09/input.txt");
        long result = lines.Select(l => PredictFirst(l)).Sum();
        Assert.Equal(2, result);
    }

    private int PredictNext(string line)
    {
        List<List<int>> allSequences = GenerateSequences(line);

        for(int i = allSequences.Count() -2; i>=0; i--)
            allSequences[i].Add( allSequences[i+1].Last() + allSequences[i].Last());

        return allSequences.First().Last();
    }

    private int PredictFirst(string line)
    {
        List<List<int>> allSequences = GenerateSequences(line);
        allSequences.ForEach(list => list.Reverse());

        for(int i = allSequences.Count() -2; i>=0; i--)
            allSequences[i].Add(allSequences[i].Last() - allSequences[i+1].Last());

        return allSequences.First().Last();
    }

    private List<List<int>> GenerateSequences(string line)
    {
        List<List<int>> allSequences = new();
        var numbers = GetNumbers(line);
        allSequences.Add(numbers);

        do{
            numbers = numbers.Zip(numbers.Skip(1), (left, right) => right - left).ToList();
            allSequences.Add(numbers);

        }while(numbers.Any(n => n != 0));
        return allSequences;
    }

    private List<int> GetNumbers(string line)
    {
        return _numbersRegExp.Matches(line).Select(x => Convert.ToInt32(x.ToString().Trim())).ToList();
    }
}