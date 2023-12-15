namespace AOC2023.Day15;

public class Test
{
    [Fact]
    public void Part1()
    {
        int test = Hash("HASH");
        Assert.Equal(52, test);

        var text = string.Join("", File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day15/sample.txt"));
        //var text = string.Join("", File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day15/input.txt"));
        long result = text.Split(',').Select(str => Hash(str)).Sum();
        Assert.Equal(1320, result);

    }

    private int Hash(string str)
    {
        int current = 0;
        foreach(char c in str)
        {
            current += c;
            current = (current * 17) % 256;
        }
        return current;
    }
}
