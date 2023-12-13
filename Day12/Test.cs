using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace AOC2023.Day12;

public class Test
{
    public static ConcurrentDictionary<string, long> _memo = new();

    [Fact]
    public void Part1()
    {
        // 9023 = Too high
        // 8454 = Too high
        // 7537 = Too high
        // 6875 = Too high
        // 6827 = Bingo
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day12/input.txt");
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day12/sample.txt");
        long result = FindAllPossibilities(lines).Sum();
        Assert.Equal(21, result);
    }

    [Fact]
    public void Part2()
    {
        // 21382178983 = Too low 
        // 1537505634471 = Bingo
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day12/input.txt");
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day12/sample.txt");
        long result = FindAllPossibilitiesUnfolding(lines).Sum();
        Assert.Equal(525152, result);
    }

    private List<long> FindAllPossibilities(string[] lines)
    {
        var result = new List<long>();
        foreach (var line in lines) 
        {
            var parts = line.Split(' ');
            var numbers = parts[1].Split(',').Select(x => Convert.ToInt32(x)).ToList();
            result.Add(CountPossibilities(parts[0], numbers));
        }
        return result;
    }

    private List<long> FindAllPossibilitiesUnfolding(string[] lines)
    {
        var result = new ConcurrentBag<long>();
        Parallel.ForEach(lines,
             new ParallelOptions { MaxDegreeOfParallelism = 16 },
             line => result.Add(CountPossibilityInLine(line))
            );
        return result.ToList();
    }

    private long CountPossibilityInLine(string line)
    {
        var parts = line.Split(' ');
        var springs = $"{parts[0]}?{parts[0]}?{parts[0]}?{parts[0]}?{parts[0]}";
        var nums = $"{parts[1]},{parts[1]},{parts[1]},{parts[1]},{parts[1]}";

        var numbers = nums.Split(',').Select(x => Convert.ToInt32(x)).ToList();
        var possibilities = CountPossibilities(springs, numbers);
        return possibilities;
    }

    private long CountPossibilities(string str, List<int> numbers)
    {
        if (!numbers.Any())
            return str.IndexOf('#') >= 0 ? 0 : 1;

        string key = str + "|" + string.Join(',', numbers);
        if(_memo.TryGetValue(key, out var value))
            return value;

        var amount = numbers.First();
        var re = new Regex($"[?#]{{{amount}}}");
        var match = re.Match(str);
        long count = 0;
        int idxMatch = 0;
        string newstr;
        if (match.Success)
        {
            // is a valid match?
            idxMatch = match.Groups[0].Index;
            if ((str.Length == idxMatch + amount || str[idxMatch + amount] != '#') && str.Substring(0, idxMatch).IndexOf('#') < 0)
            {
                // Continues
                int substringIndex = idxMatch + amount + 1 > str.Length ? str.Length : idxMatch + amount + 1;
                newstr = str.Substring(substringIndex);
                var newNumbers = numbers.Skip(1).ToList();
                count = count + CountPossibilities(newstr, newNumbers);
            }
        }
        // Ignore this index
        if (str.Length > idxMatch)
        {
            newstr = str.Substring(idxMatch + 1);

            if (newstr.Length >= amount && newstr.Length > 0 && str.Substring(0, idxMatch + 1).IndexOf('#') < 0)
                count = count + CountPossibilities(newstr, numbers);
        }
        _memo[key] = count;
        return count;
    }
}

