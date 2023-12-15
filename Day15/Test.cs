
namespace AOC2023.Day15;

public class Test
{
    [Fact]
    public void Part1()
    {
        int test = Hash("HASH");
        Assert.Equal(52, test);

        var text = string.Join("", File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day15/sample.txt"));
        //var text = string.Join("", File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day15/input.txt"));
        long result = text.Split(',').Select(str => Hash(str)).Sum();
        Assert.Equal(1320, result);

    }

    [Fact]
    public void Part2()
    {
        var text = string.Join("", File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day15/sample.txt"));
        //var text = string.Join("", File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day15/input.txt"));
        var boxes = GetLensBoxes(text.Split(','));
        long result = SumPower(boxes);
        Assert.Equal(145, result);

    }

    private long SumPower(List<List<(string label, int power)>> boxes)
    {
        long result = 0;
        for(int box = 0; box < boxes.Count; box++)
        {
            for(int lens = 0; lens < boxes[box].Count; lens++)
            {
                result += (box + 1) * (lens + 1) * boxes[box][lens].power;
            }
        }
        return result;
    }

    private List<List<(string label, int power)>> GetLensBoxes(string[] strings)
    {
        List<List<(string label, int power)>> boxes = new(256);
        for(int i =0; i< 256; i++)
            boxes.Add(new());

        foreach(var str in strings)
        {
            var parts = str.Split('-', '=');
            string label = parts[0];
            int box = Hash(label);
            if(string.IsNullOrEmpty(parts[1])) // Removing
            {
                boxes[box] = boxes[box].Where(x => x.label != label).ToList();
            }
            else // setting
            {
                int val = Convert.ToInt32(parts[1]);
                if(boxes[box].Any(x => x.label == label))
                {
                    int index = boxes[box].IndexOf(boxes[box].First(x => x.label == label));
                    boxes[box][index] = (label, val);
                }
                else
                    boxes[box].Add((label, val));
            }
                
        }

        return boxes;
    }

    private int Hash(string str)
    {
        int current = 0;
        foreach(char c in str)
        {
            current += c;
            current = (current * 17) % 256;
        }
        return current;
    }
}
