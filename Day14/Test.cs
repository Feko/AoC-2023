using System.Data;

namespace AOC2023.Day14;

public class Test
{
    [Fact]
    public void Part1()
    {
        //var array = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day14\\input.txt").Select(l => l.ToCharArray()).ToArray();
        var array = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day14\\sample.txt").Select(l => l.ToCharArray()).ToArray();
        TiltNorth(array);
        long result = CalculateWeight(array);
        Assert.Equal(136, result);
    }

    [Fact]
    public void Part2()
    {
        //var array = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day14\\input.txt").Select(l => l.ToCharArray()).ToArray();
        var array = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day14\\sample.txt").Select(l => l.ToCharArray()).ToArray();
        long result = PerformCycles(array); 
        Assert.Equal(64, result);
    }

    private long PerformCycles(char[][] array)
    {
        long totalCycles = 1000000000;
        int probeAmount = 5000;
        long[] weights = new long[probeAmount];
        for(int i =0; i < probeAmount; i++)
        {  
            TiltNorth(array); // North
            RotateMatrix(array);
            TiltNorth(array); // West
            RotateMatrix(array);
            TiltNorth(array); // South
            RotateMatrix(array);
            TiltNorth(array); // East
            RotateMatrix(array);
            weights[i] = CalculateWeight(array);
        }

        (int loopSize, int loopIndex) = FindLoop(weights);
        long loopCounts = (totalCycles - loopIndex) / loopSize;
        long index = totalCycles - (loopCounts * loopSize) - 1;
        return weights[index];
    }

    private (int loopSize, int loopIndex) FindLoop(long[] weights)
    {
        // There must be a loop here somewhere. I'll count it as a loop if I have 25 consecutive weights
        int start = 2500;
        int probeSize = 25;

        long[] subArray = weights[start..(start+probeSize)];
        int loopSize = 0;
        for (int i = start -1; i > 0; i--)
        {
            if (weights[i..(i + probeSize)].SequenceEqual(subArray))
            {
                loopSize = start - i; break;
            }
        }
        if (loopSize == 0)
            throw new Exception("No loop found");

        // Right, now where does the loop starts?
        int loopIndex = start - loopSize;
        while (weights[loopIndex] == weights[loopIndex + loopSize])
            loopIndex--;

        return (loopSize, loopIndex);
    }

    private void TiltNorth(char[][] array)
    {
        for (int column = 0; column < array[0].Length; column++)
        {
            int nextFreeLine = -1;
            for (int line = 0; line < array.Length; line++)
            {
                char current = array[line][column];
                
                if (current == '.' && nextFreeLine < 0)
                   nextFreeLine = line;

                if (current == '#')
                    nextFreeLine = -1;

                if (current == 'O')
                {
                    if (nextFreeLine < 0) // already in place
                        continue;
                    array[nextFreeLine][column] = 'O';
                    array[line][column] = '.';
                    line = nextFreeLine;
                    nextFreeLine = -1;
                }
            }
        }
    }

    private void RotateMatrix(char[][] array)
    {
        int length = array.Length;
        for (int i = 0; i < length / 2; i++)
        {
            for (int j = i; j < length - i - 1; j++)
            {

                char temp = array[i][j];
                array[i][j] = array[length - 1 - j][i];
                array[length - 1 - j][i] = array[length - 1 - i][length - 1 - j];
                array[length - 1 - i][length - 1 - j] = array[j][length - 1 - i];
                array[j][length - 1 - i] = temp;
            }
        }
    }

    private long CalculateWeight(char[][] array)
    {
        long result = 0;
        for (int line = 0; line < array.Length; line++)
            result = result + (array[line].Count(c => c == 'O') * (array.Length - line));
        return result;
    }

}
