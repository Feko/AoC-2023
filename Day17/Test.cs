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
        int resultA = SolveMinimumHeatLoss(array, Direction.Right, (0,0));

        // Now try going down
        int resultB = SolveMinimumHeatLoss(array, Direction.Down, (0,0));

        return Math.Min(resultA, resultB);
    }

    public int SolveMinimumHeatLoss(int[][] array, Direction direction,  (int row, int col) position)
    {
        PriorityQueue<((int row, int col) position, Direction direction, int accumulatedHeat, int stepsInThisDirection), int> queue = new();
        HashSet<(int,int,int,int,Direction)> memo = new();
        // HashSet<(int,int,int,Direction)> memo = new();
        queue.Enqueue((position, direction, 0, 1), 0);

        int minHeat = int.MaxValue;
        while(queue.Count > 0)
        {
            var current = queue.Dequeue();

            if(IsOutbounds(current.position, array))
                continue;

            if(array.Length -1 == current.position.row && array[0].Length -1 == current.position.col)
            {
                minHeat = current.accumulatedHeat + array[current.position.row][current.position.col];
                break;
            }

            var key = (current.position.row, current.position.col, current.stepsInThisDirection, current.accumulatedHeat, current.direction);
            //var key = (current.position.row, current.position.col, current.stepsInThisDirection, current.direction);
            if(memo.Contains(key))
                continue;
            memo.Add(key);

            int heat = current.accumulatedHeat + array[current.position.row][current.position.col];
            if(current.stepsInThisDirection < 3)
            {
                var next = (GetNextPosition(current.position, current.direction), current.direction, heat, current.stepsInThisDirection + 1);
                queue.Enqueue(next, heat);
            }

            var nextDirectionClockwise = GetClockwise(current.direction);
            var nextClockwise = (GetNextPosition(current.position, nextDirectionClockwise), nextDirectionClockwise, heat, 1);
            queue.Enqueue(nextClockwise, heat);

            var nextDirectionAntiClockwise = GetAntiClockwise(current.direction);
            var nextAntiClockwise = (GetNextPosition(current.position, nextDirectionAntiClockwise), nextDirectionAntiClockwise, heat, 1);
            queue.Enqueue(nextAntiClockwise, heat);
        }

        return minHeat;
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