
using System.Data;

namespace AOC2023.Day07;

public class Test
{   
    
    public class Hand
    {
        private List<char> _allCards;
        public int Bid;
        private string Cards;

        public Hand(string line, string allCards)
        {
            _allCards = allCards.ToCharArray().ToList();
            var parts = line.Split(' ');
            Cards = parts[0];
            Bid = Int32.Parse(parts[1]);
        }

        public long GetHandScore() => GetHandType(Cards) * 1_000_000_000 + GetCardsScore();
        public long GetHandScoreUsingJokers()
        {
            if(Cards.IndexOf('J') == -1)
                return GetHandScore();

            int amountJokers = Cards.Count(x => x == 'J');
            int amountOtherCards = Cards.Replace("J", "").ToCharArray().ToHashSet().Count();

            long highestHand = 0;
            if(amountJokers >= 4 || amountOtherCards == 1)
                highestHand = 7; // It's already a Five-of-a-kind, or can make one
            else if(amountJokers == 3)
                highestHand = 6; // More than two different kinds of cards, but three jokers: Four of a kind possible
            else if(amountJokers == 2)
            {
                // If got here, no five-of-a-kind possible, but maybe four?
                if(amountOtherCards == 2)
                    highestHand = 6;
                else
                    // three assorted, different cards. No four-of-a-kind possible, neither full-house. Three-of-a-kind is the best
                    highestHand = 4; // Three-of-a-kind
            }
            else
            {
                // Just one joker, let's calculate all the possibilities
                foreach(char c in _allCards)
                {
                    var possibility = Cards.Replace('J',c);
                    highestHand = Math.Max(highestHand, GetHandType(possibility));
                }
            }
            return highestHand * 1_000_000_000 + GetCardsScore();
        }

        public long GetHandType(string cards)
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
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day07/sample.txt");
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day07/input.txt");
        string cards = "AKQT98765432J";
        var hands = lines.Select(line => new Hand(line, cards)).ToList();
        hands = hands.OrderBy(hand => hand.GetHandScoreUsingJokers()).ToList();
        long result = 0;
        for(int i =0; i < hands.Count(); i++)
            result += (i+1) * hands[i].Bid;
        Assert.Equal(5905, result);
    }
}