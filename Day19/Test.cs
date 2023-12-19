namespace AOC2023.Day19;

public partial class Test
{
    [Fact]
    public void Part1()
    {
        var text = File.ReadAllText("/home/feko/src/dotnet/aoc2023/AoC-2023/Day19/sample.txt");
        //var text = File.ReadAllText("/home/feko/src/dotnet/aoc2023/AoC-2023/Day19/input.txt");
        long result = SumApprovedPieces(text);
        Assert.Equal(19114, result);
    }

    [Fact]
    public void Part2()
    {
        var text = File.ReadAllText("/home/feko/src/dotnet/aoc2023/AoC-2023/Day19/sample.txt");
        //var text = File.ReadAllText("/home/feko/src/dotnet/aoc2023/AoC-2023/Day19/input.txt");
        long result = CountPossibilities(text);
        Assert.Equal(167409079868000, result);
    }

    private long SumApprovedPieces(string text)
    {
        var parts = text.Split("\n\n");
        var rules = BuildRules(parts[0].Split('\n'));
        var pieces = BuildParts(parts[1].Split('\n'));

        List<Piece> approved = FilterApprovedPieces(rules, pieces);
        return approved.Sum(piece => piece.Sum());
    }

    private long CountPossibilities(string text)
    {
        var parts = text.Split("\n\n");
        var rules = BuildRules(parts[0].Split('\n'));
        var listApproved = new List<Range>();

        Traverse(rules, "in", new Range(), listApproved);

        var listPossibilitiesSum = listApproved.Select(range => range.SumPossibilities()).ToList();
        return listPossibilitiesSum.Sum();
    }

    private void Traverse(Dictionary<string, List<Rule>> allWorkflows, string key, Range range, List<Range> listApproved)
    {
        if(key == "A")
            listApproved.Add(range);
        if (key == "A" || key == "R")
            return;

        var rules = allWorkflows[key];
        Range chainedRange = range.Clone();

        foreach (var rule in rules)
        {
            var thisRange = chainedRange.Clone();
            var result = thisRange.Applicable(rule);
            if (result.applicable)
            {
                if (result.requiresNarrowing)
                    thisRange.Narrow(rule);
                Traverse(allWorkflows, rule.Destination, thisRange, listApproved);
                chainedRange.Negate(rule);
            }
        }

    }

    private List<Piece> FilterApprovedPieces(Dictionary<string, List<Rule>> rules, List<Piece> pieces)
    {
        List<Piece> approved = new(pieces.Count);
        foreach(var piece in pieces)
        {
            var workflow = "in";
            while(workflow != "A" && workflow != "R")
            {
                var workflowRules = rules[workflow];
                foreach(var rule in workflowRules)
                {
                    if(rule.Applies(piece))
                    {
                        workflow = rule.Destination;
                        break;
                    }
                }
            }
            
            if(workflow == "A")
                approved.Add(piece);
        }

        return approved;
    }

    private Dictionary<string, List<Rule>> BuildRules(string[] lines)
    {
        Dictionary<string, List<Rule>> result = new(lines.Length);
        foreach(var line in lines)
        {
            var parts = line.Replace("}","").Split('{');
            var rules = parts[1].Split(',').Select(x => new Rule(x)).ToList();
            result.Add(parts[0], rules);
        }
        return result;
    }

    private List<Piece> BuildParts(string[] strings)
    {
        return strings.Select(x => new Piece(x)).ToList();
    }
}
