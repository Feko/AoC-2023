
namespace AOC2023.Day21;

public partial class TestPart2
{
    public record TileCoords(int RowStart, int RowFinish, int ColumnStart, int ColumnFinish);
    static (int Height, int Width) MazeSize = default;
    static int ExpandFactor = 5;
    static int FinalSteps = 26501365;


    [Fact]
    public void Part2()
    {
        var lines = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day21\\input.txt").Select(s => s.ToCharArray()).ToArray(); 
        MazeSize = (lines.Length, lines[0].Length);

        // 625587097150084 = FREAKING FINALLY
        // 625582802182788 = Wrong again
        // 625582764959680 = Too low
        // 625579767694401 = Too low
        // 625579767678813 = Too low
        // 625579730455705 = even lower

        long result = PerformHugeAssAmountOfSteps(lines);
        Assert.Equal(1, result);
    }

    private long PerformHugeAssAmountOfSteps(char[][] array)
    {
        HashSet<(int row, int column)> positions = new();
        (int row,int col) start = GetStartPosition(array);
        positions.Add(start);
        int steps = (MazeSize.Height * ExpandFactor) + (FinalSteps % MazeSize.Height);

        // Right, this will run for a few seconds and map all the possible positions after repeating for (MAZE_SIZE * 5) steps.
        // This should consider a grid with 11 x 11 tiles
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

        // So we have two types of "inner" tiles, even and odd. Let's count the amount of possible positions for each one.
        // I may have swapped even/odd
        int countEven = CountPositionsForTile(GetTileAt(ExpandFactor, ExpandFactor), positions);
        int countOdd = CountPositionsForTile(GetTileAt(ExpandFactor, ExpandFactor + 1), positions);

        // We also have 4 corners, I'm counting them individually
        int cornerTop = CountPositionsForTile(GetTileAt(0, ExpandFactor), positions);
        int cornerLeft = CountPositionsForTile(GetTileAt(ExpandFactor, 0), positions);
        int cornerBottom = CountPositionsForTile(GetTileAt(ExpandFactor*2, ExpandFactor), positions);
        int cornerRight = CountPositionsForTile(GetTileAt(ExpandFactor, ExpandFactor*2), positions);

        // Now, we also have some Diagonals which we should consider
        int diagonalTopLeft = CountPositionsForTile(GetTileAt(ExpandFactor -1, 1), positions);
        int diagonalBottomLeft = CountPositionsForTile(GetTileAt(ExpandFactor +1, 1), positions);
        int diagonalTopRight = CountPositionsForTile(GetTileAt(ExpandFactor - 1, ExpandFactor * 2 -1), positions);
        int diagonalBottomRight = CountPositionsForTile(GetTileAt(ExpandFactor + 1, ExpandFactor * 2 -1), positions);

        // Sanity Check: Just to ensure the diagonals are equal
        int diagonalTopLeft2 = CountPositionsForTile(GetTileAt(ExpandFactor - 2, 2), positions);
        bool equal = diagonalTopLeft == diagonalTopLeft2;

        // Sanity Check: These small corners should be empty:
        // Update: These are NOT SUPPOSED to be zero. I was expanding to exact MazeSise, but the steps has additional + 65... I'm so stupid
        int intersectionTopLeft = CountPositionsForTile(GetTileAt(ExpandFactor - 1, 0), positions); // top/left
        int intersectionBottomLeft = CountPositionsForTile(GetTileAt(ExpandFactor + 1, 0), positions); // bottom/left
        int intersectionBottomRight = CountPositionsForTile(GetTileAt(ExpandFactor + 1, ExpandFactor * 2), positions); // bottom/right
        int intersectionTopRight = CountPositionsForTile(GetTileAt(ExpandFactor - 1, ExpandFactor * 2), positions); // top/right


        // OK, now to some calculations
        int totalWidth = FinalSteps / MazeSize.Width - 1;
        double totalOdd = Math.Pow((totalWidth / 2 * 2 + 1), 2);
        long totalOddTiles = Convert.ToInt64(totalOdd);
        double totalEven = Math.Pow(((totalWidth + 1) / 2 * 2), 2);
        long totalEvenTiles = Convert.ToInt64(totalEven);

        long odds = (totalOddTiles * countOdd);
        long evens = (totalEvenTiles * countEven);
        long corners = (cornerBottom + cornerTop + cornerLeft + cornerRight);
        long diagonals = diagonalTopLeft + diagonalBottomLeft + diagonalTopRight + diagonalBottomRight;
        diagonals = diagonals * totalWidth;
        long intersections = ((totalWidth + 1) * (intersectionTopLeft + intersectionBottomLeft + intersectionBottomRight + intersectionTopRight));

        return odds + evens + corners + diagonals + intersections;
    }

    private int CountPositionsForTile(TileCoords tileCoords, HashSet<(int row, int column)> positions)
    {
        return positions.Count(p => tileCoords.RowStart <= p.row && tileCoords.RowFinish >= p.row 
            && tileCoords.ColumnStart <= p.column && tileCoords.ColumnFinish >= p.column );
    }

    private (int, int) GetStartPosition(char[][] lines)
    {
        for(int i=0; i < lines.Length; i++)
            if(Array.IndexOf(lines[i], 'S') >= 0)
                return (i, Array.IndexOf(lines[i], 'S'));
        throw new Exception("Start position not found");
    }
   
    private List<(int,int)> GetNeighbours((int Row, int Column) position, char[][] array)
    {
        var neighbours = new List<(int row, int column)> { (position.Row +1, position.Column),
            (position.Row -1, position.Column), (position.Row, position.Column + 1), (position.Row, position.Column -1) };
        return neighbours.Where(x => IsWalkable(array, x)).ToList();
    }

    private bool IsWalkable(char[][] array, (int row, int column) x)
    {
        int row = CycleRow(x.row);
        int col = CycleColumn(x.column);
        return array[row][col] != '#';
    }

    public TileCoords GetTileAt(int tileRow, int tileColumn)
    {
        int start = MazeSize.Width * -1 * ExpandFactor;
        int startRow = start + (tileRow * MazeSize.Height);
        int startColumn = start + (tileColumn * MazeSize.Width);
        return new TileCoords(startRow, startRow + MazeSize.Height - 1, startColumn, startColumn + MazeSize.Width - 1);
    }

    public int CycleRow(int row) => row % MazeSize.Height >= 0 ? row % MazeSize.Height : MazeSize.Height + (row % MazeSize.Height);
    public int CycleColumn(int column) => column % MazeSize.Width >= 0 ? column % MazeSize.Width : MazeSize.Width + (column % MazeSize.Width);
}

