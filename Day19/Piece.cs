namespace AOC2023.Day19;

public partial class Test
{
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
}
