namespace AOC2023.Day19;

public partial class Test
{
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
}
