using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace AOC2023.Day10;

public class Test
{   
    public static class Directions
    {
        public static (int line, int column) North((int line, int column) position) => (position.line -1, position.column);
        public static (int line, int column) South((int line, int column) position) => (position.line +1, position.column);
        public static (int line, int column) East((int line, int column) position) => (position.line, position.column +1);
        public static (int line, int column) West((int line, int column) position) => (position.line, position.column -1);
    }

    public class Pipe
    {
        public (int line, int column) DirectionA;
        public (int line, int column) DirectionB;
        public char _type {get;set;}
        public (int line, int column) Position;

        public Pipe((int line, int column) pos, char type)
        {
            _type = type;
            Position = pos;
            DirectionA = type switch {
              '|' => Directions.North(pos),
              '-' => Directions.East(pos),
              'L' => Directions.North(pos),
              'J' => Directions.North(pos),
              '7' => Directions.South(pos),
              'F' => Directions.South(pos),
              '.' => (-1, -1), // Do I even need this?
              _ => throw new Exception("How would this happen?")
            };

            DirectionB = type switch {
              '|' => Directions.South(pos),
              '-' => Directions.West(pos),
              'L' => Directions.East(pos),
              'J' => Directions.West(pos),
              '7' => Directions.West(pos),
              'F' => Directions.East(pos),
              '.' => (-1, -1), // Do I even need this?
              _ => throw new Exception("How would this happen?")
            };
        }
    }

    [Fact]
    public void Part1()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day10/input.txt"); char startType = '-';
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day10/sample.txt"); char startType = 'F';
        var pipeMesh = Parse(lines, startType, out var startPosition);
        int result = FindCylePath(pipeMesh, startPosition).Count();
        Assert.Equal(8, result/2);
    }

    [Fact]
    public void Part2()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day10/input.txt"); char startType = '-'; int expect = 1;
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day10/sample2.txt"); char startType = 'F'; int expect = 8;
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day10/sample3.txt"); char startType = '7'; int expect = 10;
        var pipeMesh = Parse(lines, startType, out var startPosition);
        int result = CountPiecesInsideLoop2(pipeMesh, startPosition);
        Assert.Equal(expect, result);
    }

    private List<(int line, int column)> FindCylePath(Dictionary<(int line, int column), Pipe> pipeMesh, (int line, int column) startPosition)
    {
        List<(int line, int column)> loopPositions = new(10_000);
        loopPositions.Add(startPosition);

        var current = startPosition;
        var next = pipeMesh[startPosition].DirectionA;
        while(next != startPosition)
        {
            loopPositions.Add(next);            
            var temp = pipeMesh[next].DirectionA == current ? pipeMesh[next].DirectionB : pipeMesh[next].DirectionA; 
            current = next;
            next = temp;
        }

        return loopPositions;
    }

    private int CountPiecesInsideLoop2(Dictionary<(int line, int column), Pipe> pipeMesh, (int line, int column) startPosition)
    {
        // https://en.wikipedia.org/wiki/Even%E2%80%93odd_rule
        // |, FJ, L7
        int amount = 0;
        var loopPositions = FindCylePath(pipeMesh, startPosition);
        int height = loopPositions.Max(x => x.line);
        int width = loopPositions.Max(x => x.column);

        // Build a map with only the loop tiles
        List<string> map = new(height+1);
        for(int line = 0; line < height; line++)
        {
            var sb = new StringBuilder();
            for(int column = 0; column < width; column++)
            {
                char current = loopPositions.Contains((line,column)) ? pipeMesh[(line,column)]._type : ' ';
                sb.Append(current);
            }
            map.Add(sb.ToString());
        }

        Regex strokesRegEx = new Regex(@"(\||F-*J|L-*7)", RegexOptions.Compiled); 

        for(int line = 0; line < height; line++)
        {
            for(int column = 1; column < width; column++)
            {
                if(loopPositions.Contains((line,column)))
                    continue;
                
                var trailing = map[line].Substring(0, column);
                var strokes = strokesRegEx.Matches(trailing);
                if(strokes.Count % 2 == 1)
                    amount++;
            }
        }

        return amount;
    }


    private Dictionary<(int line, int column), Pipe> Parse(string[] input, char startType, out (int line, int column) startPosition)
    {
        Dictionary<(int, int), Pipe> result = new(input.Length * input[0].Length);
        startPosition = default;

        for(int line = 0; line < input.Length; line++)
        {
            for(int column = 0; column < input[line].Length; column++)
            {
                char type = input[line][column] == 'S' ? startType : input[line][column];
                (int,int) pos = (line,column);

                if(input[line][column] == 'S')
                    startPosition = pos;
                result[pos] = new(pos, type);
            }
        }
        return result;
    }
}