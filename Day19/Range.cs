namespace AOC2023.Day19;

public partial class Test
{
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
}
