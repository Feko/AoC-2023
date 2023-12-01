namespace AOC2023.Day01;

public class Test
{
    [Fact]
    public void Part1()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day01/sample.txt");
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day01/input.txt");
        int result = lines.Select(x => ExtractValue(x)).Sum();
        Assert.Equal(142, result);
    }


    private int ExtractValue(string str)
    {
        var digits = str.ToCharArray().Where(c => char.IsDigit(c));
        return Int32.Parse($"{digits.First()}{digits.Last()}");
    }
}