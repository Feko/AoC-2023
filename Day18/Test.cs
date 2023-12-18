using System.Data;

namespace AOC2023.Day18;

public class Test
{
    public class Vertex
    {
        public int X; 
        public int Y;

        public (int X, int Y) StartPosition;
        public string Direction;
        public Vertex((int x, int y) startPosition, string line)
        {
            StartPosition = startPosition;
            var parts = line.Split(' ');
            Direction = parts[0];
            (X, Y) = GetNewPosition(Direction, Convert.ToInt32(parts[1]));
        }

        private (int X, int Y) GetNewPosition(string direction, int amount)
        {
            return direction switch
            {
                "U" => (StartPosition.X, StartPosition.Y - amount),
                "L" => (StartPosition.X - amount, StartPosition.Y),
                "D" => (StartPosition.X, StartPosition.Y + amount),
                "R" => (StartPosition.X + amount, StartPosition.Y),
                _ => throw new Exception("Something quite unexpected happened")
            };
        }

        public int Length() => Math.Abs(StartPosition.X - X) + Math.Abs(StartPosition.Y - Y);
        public (int X, int Y) Position => (X, Y);
    }

    [Fact]
    public void Part1()
    {
        // 59500 = too low
        // 61661 = Bingo
        //var lines = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day18\\input.txt").ToList();
        var lines = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day18\\sample.txt").ToList();
        long result = FindArea(lines);
        Assert.Equal(62, result);
    }

    private long FindArea(List<string> lines)
    {
        var startPosition = (0, 0);
        List<Vertex> vertices = new List<Vertex>(lines.Count);

        foreach (var line in lines) 
        {
            vertices.Add(new Vertex(startPosition, line));
            startPosition = vertices.Last().Position;
        }

        long area = GetArea(vertices);
        long perimeter = vertices.Sum(x => x.Length());
        long result = area + (perimeter / 2) + 1;
        return result;
    }


    static long GetArea(List<Vertex> vertices)
    {
        long sum1 = 0;
        long sum2 = 0;

        for (int i = 0; i < vertices.Count - 1; i++)
        {
            sum1 = sum1 + vertices[i].X * vertices[i + 1].Y;
            sum2 = sum2 + vertices[i].Y * vertices[i + 1].X;
        }
        sum1 = sum1 + vertices.Last().X * vertices.First().Y;
        sum2 = sum2 + vertices.First().X * vertices.Last().Y;


        long area = Math.Abs(sum1 - sum2) / 2;
        return area;
    }
}
