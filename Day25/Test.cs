
using System.Text;

namespace AOC2023.Day25;

public class Test
{   
    public class Node
    {
        public bool Written { get; set; }
        public string Name { get; set; }
        public List<Node> Nodes {get;set;} = new();

        public string GetString()
        {
            if(Written)
                return Name;
            Written = true;
            return $"{Name}[{Name}]";
        }
    }

    Dictionary<string, Node> allNodes = new();

    [Fact]
    public void Part1()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day25/input.txt"); int expected = 5;
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day25/input_pruned.txt"); int expected = 5;
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day25/sample_pruned.txt"); int expected = 5;
        
        BuildGraph(lines);        

        var left = Traverse(allNodes["znv"], new());
        var right = Traverse(allNodes["ddj"], new());

        var result = left * right;
    }

    private void BuildGraph(string[] lines)
    {
        foreach(var line in lines)
        {
            var parts = line.Split(": ");
            var links = parts[1].Split(" ");

            var thisNode = GetNode(parts[0]);
            foreach(var p in links)
            {
                var node = GetNode(p);
                node.Nodes.Add(thisNode);
                thisNode.Nodes.Add(node);
            }
        }
    }

    public int Traverse(Node node, HashSet<string> visited)
    {
        int count = 0;
        if(!visited.Contains(node.Name))
        {
            visited.Add(node.Name);
            count++;
        }

        foreach(var n in node.Nodes)
        {
            if(!visited.Contains(n.Name))
                count += Traverse(n, visited);
        }

        return count;
    }

    private void WriteGraphViz()
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph G {");
        foreach(var item in allNodes)
        {
            foreach(var connection in item.Value.Nodes)
            {
                sb.AppendLine($"    {item.Value.Name} -- {connection.Name};");
            }
        }
        sb.AppendLine("}");

        File.WriteAllText("/home/feko/src/dotnet/aoc2023/AoC-2023/Day25/graphviz.txt", sb.ToString());
    }

    private Node GetNode(string name)
    {
        if(allNodes.TryGetValue(name, out var node))
            return node;

        Node n = new Node(){ Name = name};
        allNodes[name] = n;
        return n;
    }
}