namespace AOC2023.Day24;

public class Test
{
    [Fact]
    public void Part1()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day24/input.txt"); int expected = 2; double min = 200000000000000; double max = 400000000000000;
        // var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day24/sample.txt"); int expected = 2; double min = 7; double max = 27;
        List<Hail> hails = lines.Select(l => new Hail(l)).ToList();
        long result = GetPermutations(hails).Where(pair => IntersectsInRange(pair.left, pair.right, min, max)).Count();
        Assert.Equal(2, result);
    }

    private IEnumerable<(Hail left, Hail right)> GetPermutations(List<Hail> hails)
    {
        for(int i = 0; i < hails.Count; i++)
            for(int j = i + 1; j < hails.Count; j++)
                yield return (hails[i], hails[j]);
    }

    private bool IntersectsInRange(Hail left, Hail right, double min, double max)
    {
        var intersection = left.CalculateIntersection(right);
        return intersection.Intersects
            && (min <= intersection.coords.X && intersection.coords.X <= max )
            && (min <= intersection.coords.Y && intersection.coords.Y <= max );
    }

}