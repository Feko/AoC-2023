using System.Data;
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
        private char _type;

        public Pipe((int line, int column) pos, char type)
        {
            _type = type;
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

    Regex _numbersRegExp = new Regex(@"-?[0-9]+", RegexOptions.Compiled); 

    [Fact]
    public void Part1()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day10/input.txt"); char startType = '-';
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day10/sample.txt"); char startType = 'F';
        var pipeMesh = Parse(lines, startType, out var startPosition);
        int result = FindCyleLength(pipeMesh, startPosition);
        Assert.Equal(8, result/2);
    }

    private int FindCyleLength(Dictionary<(int line, int column), Pipe> pipeMesh, (int line, int column) startPosition)
    {
        int size = 1;
        var current = startPosition;
        var next = pipeMesh[startPosition].DirectionA;
        while(next != startPosition)
        {
            var temp = pipeMesh[next].DirectionA == current ? pipeMesh[next].DirectionB : pipeMesh[next].DirectionA; 
            current = next;
            next = temp;
            size++;
        }

        return size;
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