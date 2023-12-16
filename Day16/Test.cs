namespace AOC2023.Day16;

public class Test
{
    public enum Direction
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    public class Beam
    {
        public Beam(Direction direction, (int,int) position)
        {
            Direction = direction;
            Position = position;
        }

        public Direction Direction {get;set;}
        public (int Row, int Column) Position {get;set;}
        public bool IsVertical => Direction == Direction.Up || Direction == Direction.Down;
        public bool IsHorizontal => !IsVertical;

        public void Move()
        {
            Position = Direction switch 
            {
                
                Direction.Up => (Position.Row -1, Position.Column),
                Direction.Down => (Position.Row +1, Position.Column),
                Direction.Left => (Position.Row, Position.Column -1),
                Direction.Right => (Position.Row, Position.Column +1)
            };
        }

        private void TiltClockwise() => Direction = (Direction)((((int)Direction) + 3) % 4);
        private void TiltAntiClockwise() => Direction = (Direction)((((int)Direction) + 1) % 4);

        public Beam? HandlePiece(char piece)
        {
            if(piece == '.' || (piece == '-' && IsHorizontal) || (piece == '|' && IsVertical))
                return null;

            if(piece == '-' || piece == '|') // Split
            {
                TiltClockwise();
                var newDirection = (Direction)((((int)Direction) + 2) % 4);
                return new Beam(newDirection, Position);
            }

            if((piece == '/' && IsVertical) || (piece == '\\' && IsHorizontal)) //ClockWise
                TiltClockwise();
            else
                TiltAntiClockwise();
            return null;
        }
    }


    [Fact]
    public void Part1()
    {
        // 5935 = Too low
        // 7034 = Bingo
        //var array = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day16/input.txt").Select(l => l.ToCharArray()).ToArray();
        var array = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day16/sample.txt").Select(l => l.ToCharArray()).ToArray();
        var path = GetEnergizedPath(array, (0, -1), Direction.Right);
        Assert.Equal(46, path.Count);
    }

    [Fact]
    public void Part2()
    {
        //var array = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day16/input.txt").Select(l => l.ToCharArray()).ToArray();
        var array = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day16/sample.txt").Select(l => l.ToCharArray()).ToArray();
        var maxEnergize = TryAllPossibilities(array);
        Assert.Equal(51, maxEnergize);
    }

    private int TryAllPossibilities(char[][] array)
    {
        List<int> result = new(array.Length * 4);
        for(int i =0; i < array.Length; i++)
        {
            result.Add(GetEnergizedPath(array, (i, -1), Direction.Right).Count);
            result.Add(GetEnergizedPath(array, (-1, i), Direction.Down).Count);
            result.Add(GetEnergizedPath(array, (i, array.Length), Direction.Left).Count);
            result.Add(GetEnergizedPath(array, (array.Length, i), Direction.Up).Count);

        }
        return result.Max();
    }

    private HashSet<(int,int)> GetEnergizedPath(char[][] array, (int Row, int Column) startPosition, Direction startDirection)
    {
        var path = new HashSet<(int,int)>();
        var visited = new HashSet<(int,int, Direction)>();
        (int NumRows, int NumColumns) roomSize = (array.Length, array[0].Length);

        Beam firstBeam = new(startDirection, startPosition);
        var queue = new Queue<Beam>();
        queue.Enqueue(firstBeam);

        while(queue.Any())
        {
            var beam = queue.Dequeue();
            beam.Move();
            if(IsOutbounds(beam.Position, roomSize))
                continue;
            
            if(visited.Contains((beam.Position.Row, beam.Position.Column, beam.Direction)))
                continue;
            visited.Add((beam.Position.Row, beam.Position.Column, beam.Direction));

            path.Add(beam.Position);
            var splitted = beam.HandlePiece(array[beam.Position.Row][beam.Position.Column]);
            if(splitted != null)
                queue.Enqueue(splitted);
            queue.Enqueue(beam);
        }

        return path;
    }

    private bool IsOutbounds((int Row, int Column) position, (int NumRows, int NumColumns) roomSize)
    {
        if(position.Row < 0 || position.Column < 0 || position.Row >= roomSize.NumRows || position.Column >= roomSize.NumColumns)
            return true;
        return false;
    }
}