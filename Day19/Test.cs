using System.Security.Cryptography;
using Xunit.Sdk;

namespace AOC2023.Day19;

public class Test
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
   

    public class Piece
    {
        public Dictionary<char, int> Values = new();
        public Piece(string line)
        {
            line = line.Replace("}","").Replace("{","");
            var parts = line.Split(',');

            foreach(var part in parts)
            {
                Values.Add(part[0], Convert.ToInt32(part.Substring(part.IndexOf('=')+1)) );
            }
        }

        public long Sum() => Values.Values.Sum();
    }

    public class Rule
    {
        public string Destination;

        public bool IsGlobal = false;
        public char PieceKey;
        public char Signal;
        public int Number;
        private Func<Piece, bool> _func;
        public Rule(string str)
        {
            if(str.IndexOf(':') < 0)
            {
                IsGlobal = true;
                Destination = str;
            }
            else
            {
                var rule = str.Split(':')[0];
                Destination = str.Split(':')[1];
                PieceKey = rule[0];
                Number = Convert.ToInt32(rule.Split('<','>')[1]);
                Signal = rule[1];
                _func = Signal switch
                {
                    '>' => Greater,
                    '<' => Lower,
                    _ => throw new Exception("Oh boy, this is not supposed to happen")
                };
            }
        }

        public bool Applies(Piece piece) => IsGlobal || _func(piece);

        private bool Greater(Piece piece) => piece.Values[PieceKey] > Number;
        private bool Lower(Piece piece) => piece.Values[PieceKey] < Number;
    }

    public class Range
    {
        // Min/Max are INCLUSIVE
        private static char[] _keys = new char[] { 'x', 'm', 'a', 's' };
        private Dictionary<string, int> _ranges = new Dictionary<string, int>(8);

        public Range()
        {
            foreach (char c in _keys)
            {
                _ranges[$"min{c}"] = 1;
                _ranges[$"max{c}"] = 4000;
            }
        }

        public Range(Dictionary<string, int> ranges) => _ranges = ranges;

        public Range Clone() => new Range(new Dictionary<string, int>(_ranges));

        public (bool applicable, bool requiresNarrowing) Applicable(Rule rule)
        {
            if(rule.IsGlobal)
                return (true, false);

            int min = _ranges[$"min{rule.PieceKey}"];
            int max = _ranges[$"max{rule.PieceKey}"];
            if (min > max)
                throw new Exception("I don't suppose this is expected");

            if (rule.Signal == '<' && rule.Number < min) // impossible
                return (false, false);

            if (rule.Signal == '>' && rule.Number > max) // impossible
                return (false, false);

            if (rule.Signal == '<' && rule.Number > max) // possible, but no change required on range
                return (true, false);

            if (rule.Signal == '>' && rule.Number < min) // possible, but no change required on range
                return (true, false);

            if (!(rule.Number > min && rule.Number < max))
                throw new Exception("Not sure how this would happen, but okay I guess");
            
            return (true, true); // possible, but require narrowing the range
        }

        public void Narrow(Rule rule)
        {
            if (rule.Signal == '>')
                _ranges[$"min{rule.PieceKey}"] = rule.Number + 1;
            else
            {
                if (rule.Signal == '<')
                    _ranges[$"max{rule.PieceKey}"] = rule.Number - 1;
                else
                    throw new Exception("What cursed kind of signal do we have here?");
            }
        }

        public void Negate(Rule rule)
        {
            if (rule.IsGlobal) // can't negate that
                return;

            // This is very naive, I'm probably missing some corner case here
            if (rule.Signal == '>')
            {
                if (_ranges[$"max{rule.PieceKey}"] > rule.Number)
                    _ranges[$"max{rule.PieceKey}"] = rule.Number;
            }
            else
            {
                if (rule.Signal == '<')
                {
                    if (_ranges[$"min{rule.PieceKey}"] < rule.Number)
                        _ranges[$"min{rule.PieceKey}"] = rule.Number;
                }
                else
                    throw new Exception("What cursed kind of signal do we have here?");
            }
        }

        public long SumPossibilities()
        {
            long sum = 1;
            foreach (char c in _keys)
            {
                int min = _ranges[$"min{c}"];
                int max = _ranges[$"max{c}"];

                if (min == max)
                    continue;
                if (min > max)
                    throw new Exception("How?");
                long diff = (max - min) + 1;
                sum *= diff;
            }
            return sum;
        }
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
