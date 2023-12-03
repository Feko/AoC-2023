
using System.Data;
using Newtonsoft.Json.Linq;

namespace AOC2023.Day03;

public class Test
{
    [Fact]
    public void Part1()
    {
        var matrix = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day03/sample.txt").Select(x => x.ToCharArray()).ToArray();
        // var matrix = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day03/input.txt").Select(x => x.ToCharArray()).ToArray();
        int result = FindPartNumbers(matrix).Sum();
        Assert.Equal(4361, result);
    }

    private List<int> FindPartNumbers(char[][] matrix)
    {
        List<int> partNumberrs = new();
        for(int line = 0; line < matrix.Length; line++)
        {
            int numberStart = -1;
            for(int column = 0; column < matrix[line].Length; column++)
            {
                if(char.IsDigit(matrix[line][column]) && numberStart < 0)
                    numberStart = column;
                if(!char.IsDigit(matrix[line][column]) && numberStart >= 0)
                {
                    if(SurroundedBySymbol(matrix, line, numberStart, column-1))
                    {
                        partNumberrs.Add(Convert.ToInt32(new string(matrix[line][numberStart..column])));
                    }
                    numberStart = -1;
                }                
            }
            if(numberStart > 0 && SurroundedBySymbol(matrix, line, numberStart, matrix[line].Length -1))
            {
                int numberEnd = matrix[line].Length;
                partNumberrs.Add(Convert.ToInt32(new string(matrix[line][numberStart..numberEnd])));
            }
        }
        return partNumberrs;
    }

    private bool SurroundedBySymbol(char[][] matrix, int line, int numberStart, int numberEnd)
    {
        // Check Sides
        if(IsSymbol(matrix, line, numberStart-1) || IsSymbol(matrix, line, numberEnd+1))
            return true;

        foreach(int row in new[]{line-1, line+1})
        {
            for(int column = numberStart-1; column < numberEnd+2; column++)
            {
                if(IsSymbol(matrix, row, column))
                    return true;
            }            
        }
        return false;

    }
    private bool IsSymbol(char[][] matrix, int line, int column)
    {
        if(Outbounds(matrix, line, column))
            return false;
        return matrix[line][column] != '.' && !char.IsDigit(matrix[line][column]); 
    }

    private bool Outbounds(char[][] matrix, int line, int column)
    {
        return line < 0 || column < 0 || line >= matrix.Length || column >= matrix[line].Length;
    }
}