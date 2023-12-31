
using System.Data;

namespace AOC2023.Day07;

public class Test
{   
    
    public class Hand
    {
        protected List<char> _allCards;
        public int Bid;
        protected string Cards;

        public Hand(string line, string allCards)
        {
            _allCards = allCards.ToCharArray().ToList();
            var parts = line.Split(' ');
            Cards = parts[0];
            Bid = Int32.Parse(parts[1]);
        }

        public virtual long GetHandScore() => GetHandType(Cards) * 1_000_000_000 + GetCardsScore();

        public virtual long GetHandType(string cards)
        {
            // 7 = Five of a Kind, 1 = High Card
            var grouped = cards.ToCharArray().GroupBy(x => x);
            if(grouped.Count() == 1)
                return 7; // Five of a Kind
            
            if(grouped.Count() == 2 && grouped.Any(x => x.Count() == 4))
                return 6; // Four of a Kind

            if(grouped.Count() == 2 && grouped.Any(x => x.Count() == 3))
                return 5; // Full House

            if(grouped.Count() == 3 && grouped.Any(x => x.Count() == 3))
                return 4; // Three of a Kind

            if(grouped.Count() == 3 && grouped.Any(x => x.Count() == 2))
                return 3; // Two pairs
            
            if(grouped.Count() == 4)
                return 2; // Pairs

            return 1; // High card
        }

        public long GetCardsScore()
        {
            long score = 0;
            int[] multipliers = {12_000_000, 800_000, 60_000, 4_000, 100};
            for(int i = 0; i < Cards.Length; i++)
            {
                score += multipliers[i] * (_allCards.Count() - _allCards.IndexOf(Cards[i]));
            }
            return score;
        }
    }

    public class JokerHand : Hand
    {
        public JokerHand(string line, string allCards) : base(line, allCards)
        {
        }

        public override long GetHandType(string cards)
        {
            if(cards.IndexOf('J') == -1 || cards == "JJJJJ")
                return base.GetHandType(cards);
            
            char mostFrequentNonJokerCard = cards.Replace("J","").ToCharArray().GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
            return base.GetHandType(cards.Replace('J', mostFrequentNonJokerCard));
        }
    }

    [Fact]
    public void Part1()
    {
        // 250369992 = Too low
        // 250294748 <- Wrong, even lower
        // 250370104

        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day07/sample.txt");
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day07/input.txt");
        string cards = "AKQJT98765432";
        var hands = lines.Select(line => new Hand(line, cards)).ToList();
        hands = hands.OrderBy(hand => hand.GetHandScore()).ToList();
        long result = 0;
        for(int i =0; i < hands.Count(); i++)
            result += (i+1) * hands[i].Bid;
        Assert.Equal(6440, result);
    }

    [Fact]
    public void Part2()
    {
        //251735672
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day07/sample.txt");
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day07/input.txt");
        string cards = "AKQT98765432J";
        var hands = lines.Select(line => new JokerHand(line, cards)).ToList();
        hands = hands.OrderBy(hand => hand.GetHandScore()).ToList();
        long result = 0;
        for(int i =0; i < hands.Count(); i++)
            result += (i+1) * hands[i].Bid;
        Assert.Equal(5905, result);
    }
}