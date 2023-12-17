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
        // 1048 as result... I'm off with the first item (array[0][0]), somehow. So 1044 is my answer.
        //var array = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day17/input.txt")
        var array = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day17/sample.txt")
            .Select(l => l.ToCharArray().Select(c => c - '0').ToArray()).ToArray();
        var heatLoss = SolveMinimumHeatLoss(array);
        Assert.Equal(102, heatLoss);
    }

    [Fact]
    public void Part2()
    {
        // 1231 as result... Again I'm off with the first item (array[0][0]), somehow. So 1227 is my answer.
        var array = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day17/input.txt")
        //var array = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day17/sample.txt")
            .Select(l => l.ToCharArray().Select(c => c - '0').ToArray()).ToArray();
        var heatLoss = SolveMinimumHeatLossWithAtLeast4StepsAndAtMaximum10BecauseWhyNot(array);
        Assert.Equal(94, heatLoss);
    }

    public int SolveMinimumHeatLoss(int[][] array)
    {
        // From (0,0) try start going right
        int resultA = SolveMinimumHeatLoss(array, Direction.Right, (0,0));

        // Now try going down
        int resultB = SolveMinimumHeatLoss(array, Direction.Down, (0,0));

        return Math.Min(resultA, resultB);
    }

    public int SolveMinimumHeatLossWithAtLeast4StepsAndAtMaximum10BecauseWhyNot(int[][] array)
    {
        // From (0,0) try start going right
        int resultA = SolveMinimumHeatLossWithAtLeast4StepsAndAtMaximum10BecauseWhyNot(array, Direction.Right, (0,0));

        // Now try going down
        int resultB = SolveMinimumHeatLossWithAtLeast4StepsAndAtMaximum10BecauseWhyNot(array, Direction.Down, (0,0));

        return Math.Min(resultA, resultB);
    }

    public int SolveMinimumHeatLoss(int[][] array, Direction direction,  (int row, int col) position)
    {
        PriorityQueue<((int row, int col) position, Direction direction, int accumulatedHeat, int stepsInThisDirection), int> queue = new();
        HashSet<(int,int,int,Direction)> memo = new();
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

            //var key = (current.position.row, current.position.col, current.stepsInThisDirection, current.accumulatedHeat, current.direction);
            var key = (current.position.row, current.position.col, current.stepsInThisDirection, current.direction);
            if(memo.Contains(key))
                continue;
            memo.Add(key);

            int heat = current.accumulatedHeat + array[current.position.row][current.position.col];
            List<Direction> nextDirections = new List<Direction>{ GetClockwise(current.direction), GetAntiClockwise(current.direction) };

            if(current.stepsInThisDirection < 3)
                nextDirections.Add(current.direction);
            foreach(var nextDirection in nextDirections)
            {
                var next = (GetNextPosition(current.position, nextDirection), nextDirection, heat, nextDirection == current.direction ? current.stepsInThisDirection + 1 : 1);
                if(!IsOutbounds(next.Item1, array))
                {
                    var thisHeat = heat + array[next.Item1.Item1][next.Item1.Item2];
                    queue.Enqueue(next, thisHeat); 
                }
            }
        }

        return minHeat;
    }

    public int SolveMinimumHeatLossWithAtLeast4StepsAndAtMaximum10BecauseWhyNot(int[][] array, Direction direction,  (int row, int col) position)
    {
        PriorityQueue<((int row, int col) position, Direction direction, int accumulatedHeat, int stepsInThisDirection), int> queue = new();
        HashSet<(int,int,int,Direction)> memo = new();
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

            var key = (current.position.row, current.position.col, current.stepsInThisDirection, current.direction);
            if(memo.Contains(key))
                continue;
            memo.Add(key);

            int heat = current.accumulatedHeat + array[current.position.row][current.position.col];
            List<Direction> nextDirections = new List<Direction>();
            
            if(current.stepsInThisDirection < 4)
                nextDirections.Add(current.direction);
            else
            {
                if(current.stepsInThisDirection < 10)
                {
                    nextDirections.Add(current.direction);
                    nextDirections.Add(GetClockwise(current.direction));
                    nextDirections.Add(GetAntiClockwise(current.direction));
                }
                else
                {
                    nextDirections.Add(GetClockwise(current.direction));
                    nextDirections.Add(GetAntiClockwise(current.direction));
                }
            }
            foreach(var nextDirection in nextDirections)
            {
                var next = (GetNextPosition(current.position, nextDirection), nextDirection, heat, nextDirection == current.direction ? current.stepsInThisDirection + 1 : 1);
                if(!IsOutbounds(next.Item1, array))
                {
                    var thisHeat = heat + array[next.Item1.Item1][next.Item1.Item2];
                    queue.Enqueue(next, thisHeat); 
                }
            }
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