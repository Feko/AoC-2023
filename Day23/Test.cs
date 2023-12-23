namespace AOC2023.Day23;

public class Test
{   
    public class Node
    {
        public (int row, int column) Position {get; set;}
        public List<(int Weight, Node Node)> Paths {get; set;} = new();

        public void AddNode(Node node, int weight)
        {
            if(Paths.Any(x => x.Node == node))
                return;

            Paths.Add((weight, node));
        }
    }

    static Dictionary<char, (int, int)> Slopes = new()
    {
        {'^', (-1, 0)},
        {'>', (0, 1)},
        {'v', (1, 0)},
        {'<', (0, -1)},
    };

    private bool FollowSlopes = true;
    private (int row, int column) TargetPosition;

    [Fact]
    public void Part1()
    {
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day23/sample.txt")
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day23/input.txt")
            .Select(line => line.ToCharArray()).ToArray();
        int result = FindLongestPath(lines);
        Assert.Equal(94, result);
    }

    [Fact]
    public void Part2()
    {
        int expected = 154; var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day23/sample.txt")
        //int expected = 6534; var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day23/input.txt")
            .Select(line => line.ToCharArray()).ToArray();
        FollowSlopes = false;
        TargetPosition = (lines.Length -1, lines[0].Length -2);

        var node = CompactGraph(lines);
        int result = FindLongestPath(node);
        Assert.Equal(expected, result);
    }

    private int FindLongestPath(Node node)
    {
        return FindLongestPath(node, new(), 0);
    }

    private int FindLongestPath(Node node, Stack<(int,int)> path, int steps)
    {
        if(node.Position == TargetPosition)
            return steps;

        path.Push(node.Position);
        int biggest = 1;

        foreach(var p in node.Paths)
        {
            if(path.Contains(p.Node.Position))
                continue;

            int attempt = FindLongestPath(p.Node, path, steps + p.Weight);
            biggest = Math.Max(biggest, attempt);
        }

        path.Pop();
        return biggest;
    }

    private int FindLongestPath(char[][] lines)
    {
        (int row, int column) start = (0, 1);
        return FindLongestPath(lines, start, new(), 0);
    }

    private int FindLongestPath(char[][] array, (int row, int column) position, Stack<(int, int)> path, int steps)
    {
        if(path.Contains(position)) // Same step, nope
            return 1;

        if(position == (array.Length -1, array[0].Length -2)) // Found it
            return steps;

        path.Push(position);
        int biggest = 1;

        List<(int row, int column)> neighbours = GetNeighbours(array, position, (-1, -1));
        foreach(var neighbour in neighbours)
            biggest = Math.Max(biggest, FindLongestPath(array, neighbour, path, steps + 1));

        path.Pop();
        return biggest;
    }


    private Node CompactGraph(char[][] array)
    {
        List<Node> allNodes = new();
        (int row, int column) startPosition = (0,1);
        (int row, int column) nextStep = (1,1);
        Node startNode = new(){Position = startPosition};
        allNodes.Add(startNode);

        Queue<(Node node, (int row, int column) nextPosition)> queue = new();
        queue.Enqueue((startNode, nextStep));

        while(queue.Any())
        {
            var current = queue.Dequeue();
            int steps = 1;
            var previous = current.node.Position;
            var position = current.nextPosition;
            var neighbours = GetNeighbours(array, position, previous);
            while(neighbours.Count == 1)
            {
                steps++;
                previous = position;
                position = neighbours.First();
                if(position == TargetPosition) // Reached destination
                    break;
                neighbours = GetNeighbours(array, position, previous);
            }

            // Right - We are out of the while, which means either we hit de destination, or we reached a fork
            // Do we already have a node for this position?
            if(allNodes.Any(x => x.Position == position))
            {
                var forkNode = allNodes.First(x => x.Position == position);
                current.node.AddNode(forkNode, steps);
                forkNode.AddNode(current.node, steps);
            }
            else
            {
                // New node
                var forkNode = new Node(){Position = position};
                allNodes.Add(forkNode);
                current.node.AddNode(forkNode, steps);
                forkNode.AddNode(current.node, steps);
                if(position != TargetPosition)
                    foreach(var n in neighbours)
                        queue.Enqueue((forkNode, n));
            }
        }
        return startNode;
    }

    private List<(int row, int column)> GetNeighbours(char[][] array, (int row, int column) position, (int row, int column) previous)
    {
        if(FollowSlopes && Slopes.ContainsKey(array[position.row][position.column])) // It's a slope, can only go one dication
            return new(){ SumTuples(position, Slopes[array[position.row][position.column]]) };
        
        return Slopes.Values.Select(x => SumTuples(position, x)).Where(x => !IsOutbounds(x, array))
            .Where(p => array[p.row][p.column] != '#').Where(p => p != previous).ToList();
    }

    private bool IsOutbounds((int Row, int Column) position, char[][] array)
    {
        if(position.Row < 0 || position.Column < 0 || position.Row >= array.Length || position.Column >= array[0].Length)
            return true;
        return false;
    }

    public (int row, int column) SumTuples((int row, int column) a, (int row, int column) b) => (a.row + b.row, a.column + b.column);
}