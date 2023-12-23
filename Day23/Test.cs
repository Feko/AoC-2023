namespace AOC2023.Day23;

public class Test
{   
    static Dictionary<char, (int, int)> Slopes = new()
    {
        {'^', (-1, 0)},
        {'>', (0, 1)},
        {'v', (1, 0)},
        {'<', (0, -1)},
    };

    static Dictionary<(int,int), int> Memo = new();

    [Fact]
    public void Part1()
    {
        // var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day23/sample.txt")
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day23/input.txt")
            .Select(line => line.ToCharArray()).ToArray();
        int result = FindLongestPath(lines);
        Assert.Equal(94, result);
    }

    private int FindLongestPath(char[][] lines)
    {
        (int row, int column) start = (0, 1);
        Stack<(int, int)> path = new();
        int result = FindLongestPath(lines, start, path, 0);
        return result;
    }

    private int FindLongestPath(char[][] array, (int row, int column) position, Stack<(int, int)> path, int steps)
    {
        if(path.Contains(position)) // Same step, nope
            return 1;

        if(position == (array.Length -1, array[0].Length -2)) // Found it
            return steps;

        // if(Memo.ContainsKey(position))
        //     return Memo[position];
        
        path.Push(position);
        int biggest = 1;

        List<(int row, int column)> neighbours = GetNeighbours(array, position);
        foreach(var neighbour in neighbours)
            biggest = Math.Max(biggest, FindLongestPath(array, neighbour, path, steps + 1));

        //Memo[position] = biggest - steps;
        path.Pop();
        return biggest;
    }

    private List<(int row, int column)> GetNeighbours(char[][] array, (int row, int column) position)
    {
        if(Slopes.ContainsKey(array[position.row][position.column])) // It's a slope, can only go one dication
            return new(){ SumTuples(position, Slopes[array[position.row][position.column]]) };
        
        return Slopes.Values.Select(x => SumTuples(position, x)).Where(x => !IsOutbounds(x, array))
            .Where(p => array[p.row][p.column] != '#').ToList();
    }

    private bool IsOutbounds((int Row, int Column) position, char[][] array)
    {
        if(position.Row < 0 || position.Column < 0 || position.Row >= array.Length || position.Column >= array[0].Length)
            return true;
        return false;
    }

    public (int row, int column) SumTuples((int row, int column) a, (int row, int column) b) => (a.row + b.row, a.column + b.column);
}