
namespace AOC2023.Day21;

public partial class Test
{
    public enum SignalStrength { Low, High }
    public record Signal(string From, string To, SignalStrength Strength);

    [Fact]
    public void Part1()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day21/sample.txt")
            .Select(s => s.ToCharArray()).ToArray(); long expected = 16; int steps = 6;
        // var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day21/input.txt")
        //     .Select(s => s.ToCharArray()).ToArray(); long expected = 16; int steps = 64;
        
        int result = CountPossibleMoves(lines, steps);
        Assert.Equal(expected, result);
    }

    private int CountPossibleMoves(char[][] array, int steps)
    {
        HashSet<(int row, int column)> positions = new();
        (int,int) start = GetStartPosition(array);
        positions.Add(start);

        for(int i=0; i<steps; i++ )
        {
            HashSet<(int row, int column)> newPositions = new();
            foreach(var position in positions)
            {
                foreach(var neighbour in GetNeighbours(position, array))
                    newPositions.Add(neighbour);
            }
            positions = newPositions;
        }

        return positions.Count;
    }

    private (int, int) GetStartPosition(char[][] lines)
    {
        for(int i=0; i < lines.Length; i++)
            if(Array.IndexOf(lines[i], 'S') >= 0)
                return (i, Array.IndexOf(lines[i], 'S'));
        throw new Exception("Start position not found");
    }

    private bool IsOutbounds((int Row, int Column) position, char[][] array)
    {
        if(position.Row < 0 || position.Column < 0 || position.Row >= array.Length || position.Column >= array[0].Length)
            return true;
        return false;
    }

    private List<(int,int)> GetNeighbours((int Row, int Column) position, char[][] array)
    {
        var neighbours = new List<(int row, int column)> { (position.Row +1, position.Column),
            (position.Row -1, position.Column), (position.Row, position.Column + 1), (position.Row, position.Column -1) }; 
        return neighbours.Where( x=> !IsOutbounds(x,array) && array[x.row][x.column] != '#' ).ToList();
    }
}
