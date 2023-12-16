

using System.ComponentModel;

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
        var array = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day16/input.txt").Select(l => l.ToCharArray()).ToArray();
        // var array = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day16/sample.txt").Select(l => l.ToCharArray()).ToArray();
        var path = GetEnergizedPath(array);
        Assert.Equal(46, path.Count);
    }

    private HashSet<(int,int)> GetEnergizedPath(char[][] array)
    {
        var path = new HashSet<(int,int)>();
        (int Row, int Column) startPosition = (0, -1);
        (int NumRows, int NumColumns) roomSize = (array.Length, array[0].Length);

        Beam firstBeam = new(Direction.Right, startPosition);
        var queue = new Queue<Beam>();
        queue.Enqueue(firstBeam);

        // The beams enter a loop, so it's neverending spliting.
        // I'm circunventing it by adding a threshold: If the path doesn't change after X cycles, it's a loop and we're out of the loop
        (int thresold, int lastPathLength, int countPathUnchanged) = (10_000_000, 0, 0);

        while(queue.Any() && countPathUnchanged < (queue.Count * 5))
        {
            var beam = queue.Dequeue();
            beam.Move();
            if(IsOutbounds(beam.Position, roomSize))
                continue;
            
            path.Add(beam.Position);
            var splitted = beam.HandlePiece(array[beam.Position.Row][beam.Position.Column]);
            if(splitted != null)
                queue.Enqueue(splitted);
            queue.Enqueue(beam);

            // Handle loop
            if(path.Count == lastPathLength)
                countPathUnchanged++;
            else
            {
                lastPathLength = path.Count;
                countPathUnchanged = 0;
            }
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