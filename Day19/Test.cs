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

        private bool _isGlobal = false;
        private char _pieceKey;
        private int _number;
        private Func<Piece, bool> _func;
        public Rule(string str)
        {
            if(str.IndexOf(':') < 0)
            {
                _isGlobal = true;
                Destination = str;
            }
            else
            {
                var rule = str.Split(':')[0];
                Destination = str.Split(':')[1];
                _pieceKey = rule[0];
                _number = Convert.ToInt32(rule.Split('<','>')[1]);
                _func = rule[1] switch
                {
                    '>' => Greater,
                    '<' => Lower,
                    _ => throw new Exception("Oh boy, this is not supposed to happen")
                };
            }
        }

        public bool Applies(Piece piece) => _isGlobal || _func(piece);

        private bool Greater(Piece piece) => piece.Values[_pieceKey] > _number;
        private bool Lower(Piece piece) => piece.Values[_pieceKey] < _number;
    }

    private long SumApprovedPieces(string text)
    {
        var parts = text.Split("\n\n");
        var rules = BuildRules(parts[0].Split('\n'));
        var pieces = BuildParts(parts[1].Split('\n'));

        List<Piece> approved = FilterApprovedPieces(rules, pieces);
        return approved.Sum(piece => piece.Sum());
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
