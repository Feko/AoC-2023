namespace AOC2023.Day17;

public class Test
{
    public enum Direction
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    [Fact]
    public void Part1()
    {
        var array = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day17/sample.txt")
            .Select(l => l.ToCharArray().Select(c => c - '0').ToArray()).ToArray();
        var heatLoss = SolveMinimumHeatLoss(array);
        Assert.Equal(102, heatLoss);
    }

    public int SolveMinimumHeatLoss(int[][] array)
    {
        // From (0,0) try start going right
        int resultA = SolveMinimumHeatLoss(array, accumulatedHeat: 0, stepsInThisDirection: 1, Direction.Right, (0,0), new(), new());

        // Now try going down
        int resultB = SolveMinimumHeatLoss(array, accumulatedHeat: 0, stepsInThisDirection: 1, Direction.Down, (0,0), new(), new());

        return Math.Min(resultA, resultB);
    }

    public int SolveMinimumHeatLoss(int[][] array, int accumulatedHeat, int stepsInThisDirection, Direction direction, 
        (int row, int col) position, Stack<(int,int)> thisPath, Dictionary<(int,int, int, Direction), int> memo )
    {
        if(IsOutbounds(position, array)) // Is this position outbounds?
            return int.MaxValue;

        if(thisPath.Contains(position)) // Are we in a loop?
            return int.MaxValue;

        (int,int, int, Direction) key = (position.row, position.col, stepsInThisDirection, direction);
        //if(memo.ContainsKey(key)) // Have we already hit this position
        //    return memo[key];

        accumulatedHeat += array[position.row][position.col];
        if(array.Length -1 == position.row && array[0].Length -1 == position.col) // Is this the desired location?
            return accumulatedHeat;

        int min = int.MaxValue;
        thisPath.Push(position);

        int result = 0;
        Direction nextDirection = direction;
        if(stepsInThisDirection < 3) // Can I still go in this direction?
        {
            result = SolveMinimumHeatLoss(array, accumulatedHeat, stepsInThisDirection + 1, direction, GetNextPosition(position, direction), thisPath, memo);
            min = Math.Min(min, result);
        }

        // Try going clockwise
        nextDirection = GetClockwise(direction);
        result = SolveMinimumHeatLoss(array, accumulatedHeat, 1, nextDirection, GetNextPosition(position, nextDirection), thisPath, memo);
        min = Math.Min(min, result);
        
        // Try going anti-clockwise
        nextDirection = GetAntiClockwise(direction);
        result = SolveMinimumHeatLoss(array, accumulatedHeat, 1, nextDirection, GetNextPosition(position, nextDirection), thisPath, memo);
        min = Math.Min(min, result);

        thisPath.Pop();
        memo[key] = min;
        return min;
    }

    private Direction GetClockwise(Direction d) => (Direction)((((int)d) + 3) % 4);
    private Direction GetAntiClockwise(Direction d) => (Direction)((((int)d) + 1) % 4);

    public (int Row, int Column) GetNextPosition((int Row, int Column) Position, Direction Direction)
    {
        return Direction switch 
        {
            
            Direction.Up => (Position.Row -1, Position.Column),
            Direction.Down => (Position.Row +1, Position.Column),
            Direction.Left => (Position.Row, Position.Column -1),
            Direction.Right => (Position.Row, Position.Column +1)
        };
    }

    private bool IsOutbounds((int Row, int Column) position, int[][] array)
    {
        if(position.Row < 0 || position.Column < 0 || position.Row >= array.Length || position.Column >= array[0].Length)
            return true;
        return false;
    }
}