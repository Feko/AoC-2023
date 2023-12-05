
using System.Data;
using System.Text.RegularExpressions;

namespace AOC2023.Day05;

public class Test
{   
    Regex _numbersRegExp = new Regex(@"[0-9]+", RegexOptions.Compiled); 

    [Fact]
    public void Part1()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day05/input.txt");
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day05/sample.txt");
        long result = FindLowestLocation(lines);
        Assert.Equal(35, result);
    }

    private long FindLowestLocation(string[] lines)
    {
        List<long> locations = new();
        var seeds = GetNumbers(lines.First());
        var maps = ParseMaps(lines);
        foreach(var seed in seeds)
        {
            long val = seed;
            foreach(var map in maps)
                val = Remap(map, val);
            locations.Add(val);
        }

        return locations.Min();
    }

    private long Remap(List<(long,long,long)> maps, long val)
    {
        foreach((long to, long from, long range) map in maps)
        {
            if(val >= map.from && val < map.from + map.range)
                return map.to + (val - map.from);
        }
        return val;
    }

    private List<long> GetNumbers(string line)
    {
        return _numbersRegExp.Matches(line).Select(x => Convert.ToInt64(x.ToString().Trim())).ToList();
    }

    private List<List<(long,long,long)>> ParseMaps(string[] lines)
    {
        List<List<(long,long,long)>> result = new();
        List<(long,long,long)> current = new();
        
        foreach(var line in lines.Skip(1))
        {
            if(string.IsNullOrEmpty(line.Trim()))
                continue;
            
            if(line.Contains(" map"))
            {
                result.Add(current);
                current = new();
                continue;
            }
            var nums = GetNumbers(line);
            current.Add((nums[0], nums[1], nums[2]));
        }
        if(current.Any())
            result.Add(current);
        
        return result;
    }
}