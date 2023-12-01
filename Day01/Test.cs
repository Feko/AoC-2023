namespace AOC2023.Day01;

public class Test
{
    [Fact]
    public void Part1()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day01/sample.txt");
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day01/input.txt");
        int result = lines.Select(x => ExtractValue(x)).Sum();
        Assert.Equal(142, result);
    }


    private int ExtractValue(string str)
    {
        var digits = str.ToCharArray().Where(c => char.IsDigit(c));
        return Int32.Parse($"{digits.First()}{digits.Last()}");
    }

    [Fact]
    public void Part2()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day01/sample2.txt");
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day01/input.txt");
        int result = lines.Select(x => ExtractValueAnnoying(x)).Sum();
        Assert.Equal(281, result);
    }

    private int ExtractValueAnnoying(string str)
    {
        List<int> digits = new();
        for(int i =0; i<str.Length; i++)
        {
            foreach(var (k,v) in numbers)
            {
                if(i+k.Length <= str.Length && str.Substring(i, k.Length) == k)
                {
                    digits.Add(v);
                    break;
                }
            }
        }
        return Int32.Parse($"{digits.First()}{digits.Last()}");
    }

    private static Dictionary<string, int> numbers = new()
    {
        {"1", 1}, {"one", 1},
        {"2", 2}, {"two", 2},
        {"3", 3}, {"three", 3},
        {"4", 4}, {"four", 4},
        {"5", 5}, {"five", 5},
        {"6", 6}, {"six", 6},
        {"7", 7}, {"seven", 7},
        {"8", 8}, {"eight", 8},
        {"9", 9}, {"nine", 9},
    };
}