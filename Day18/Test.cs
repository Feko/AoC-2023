namespace AOC2023.Day18;

public class Test
{
    public class Vertex
    {
        public long X; 
        public long Y;

        public (long X, long Y) StartPosition;
        private string _line;
        public Vertex((long x, long y) startPosition, string line)
        {
            StartPosition = startPosition;
            _line = line;
        }

        public Vertex Parse()
        {
            var parts = _line.Split(' ');
            (X, Y) = GetNewPosition(parts[0], Convert.ToInt32(parts[1]));
            return this;
        }

        public Vertex ParseHex()
        {
            string direction = _line[(_line.IndexOf('#') + 6)].ToString();
            string hexNumber = _line.Substring(_line.IndexOf('#')+1, 5);
            (X, Y) = GetNewPosition(direction, Convert.ToInt32(hexNumber, 16));
            return this;
        }

        private (long X, long Y) GetNewPosition(string direction, int amount)
        {
            return direction switch
            {
                "U" or "3" => (StartPosition.X, StartPosition.Y - amount),
                "L" or "2" => (StartPosition.X - amount, StartPosition.Y),
                "D" or "1" => (StartPosition.X, StartPosition.Y + amount),
                "R" or "0" => (StartPosition.X + amount, StartPosition.Y),
                _ => throw new Exception("Something quite unexpected happened")
            };
        }

        public long Length() => Math.Abs(StartPosition.X - X) + Math.Abs(StartPosition.Y - Y);
        public (long X, long Y) Position => (X, Y);
    }

    [Fact]
    public void Part1()
    {
        // 59500 = too low
        // 61661 = Bingo
        //var lines = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day18\\input.txt").ToList();
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day18/sample.txt").ToList();
        long result = FindArea(lines);
        Assert.Equal(62, result);
    }

    [Fact]
    public void Part2()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day18/sample.txt").ToList();
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day18/input.txt").ToList();
        long result = FindAreaFromHex(lines);
        Assert.Equal(952408144115, result);
    }

    private long FindArea(List<string> lines)
    {
        (long, long) startPosition = (0, 0);
        List<Vertex> vertices = new List<Vertex>(lines.Count);

        foreach (var line in lines) 
        {
            vertices.Add(new Vertex(startPosition, line).Parse());
            startPosition = vertices.Last().Position;
        }

        return Solve(vertices);
    }

    private long FindAreaFromHex(List<string> lines)
    {
        (long,long) startPosition = (0, 0);
        List<Vertex> vertices = new List<Vertex>(lines.Count);

        foreach (var line in lines) 
        {
            vertices.Add(new Vertex(startPosition, line).ParseHex());
            startPosition = vertices.Last().Position;
        }

        return Solve(vertices);
    }

    private long Solve(List<Vertex> vertices)
    {
        long area = GetArea(vertices);
        long perimeter = vertices.Sum(x => x.Length());
        long result = area + (perimeter / 2) + 1;
        return result;
    }

    private long GetArea(List<Vertex> vertices)
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
