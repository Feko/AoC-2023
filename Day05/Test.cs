
using System.Collections.Concurrent;
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

    [Fact]
    public void Part2()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day05/input.txt");
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day05/sample.txt");
        long result = FindLowestLocationAllSeeds(lines);
        Assert.Equal(46, result);
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

    private long FindLowestLocationAllSeeds(string[] lines)
    {
        ConcurrentBag<long> locations = new();
        var seeds = GetNumbers(lines.First());
        var maps = ParseMaps(lines);
        List<Thread> threads = new();
        for(int i = 0; i < seeds.Count; i = i + 2)
        {
            (long startSeed, long endSeed) = (seeds[i], seeds[i] + seeds[i+1]);
            Thread t = new Thread(() => FindLowestLocationConcurrent(maps, locations, startSeed, endSeed));
            t.Start();
            threads.Add(t);
        }
        foreach(var thread in threads)
            thread.Join();

        return locations.Min();
    }

    private void FindLowestLocationConcurrent(List<List<(long,long,long)>> maps, ConcurrentBag<long> locations, long startSeed, long endSeed)
    {
        long lowestLocation = long.MaxValue;
        for(long seed = startSeed; seed < endSeed; seed++)
        {
            long val = seed;
            foreach(var map in maps)
                val = Remap(map, val);
            lowestLocation = Math.Min(lowestLocation, val);
        }
        locations.Add(lowestLocation);
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