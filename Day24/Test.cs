using Microsoft.Z3;

namespace AOC2023.Day24;

public class Test
{
    [Fact]
    public void Part1()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day24/input.txt"); int expected = 2; double min = 200000000000000; double max = 400000000000000;
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day24/sample.txt"); int expected = 2; double min = 7; double max = 27;
        List<Hail> hails = lines.Select(l => new Hail(l)).ToList();
        long result = GetPermutations(hails).Where(pair => IntersectsInRange(pair.left, pair.right, min, max)).Count();
        Assert.Equal(2, result);
    }

    [Fact]
    public void Part2()
    {
        //var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day24/input.txt"); long expected = 695832176624149; 
        var lines = File.ReadAllLines("/home/feko/src/dotnet/aoc2023/AoC-2023/Day24/sample.txt"); long expected = 47;
        List<Hail> hails = lines.Select(l => new Hail(l)).ToList();
        var result = SolveWithZ3(hails).Sum();
        Assert.Equal(expected, result);
    }

    private List<long> SolveWithZ3(List<Hail> hails)
    {
        var context = new Context(new Dictionary<string, string>() {{ "proof", "true" }});
        var solver = context.MkSolver();
        
        (var posx, var posy, var posz) = (context.MkIntConst("posx"), context.MkIntConst("posy"), context.MkIntConst("posz"));
        (var velx, var vely, var velz) = (context.MkIntConst("velx"), context.MkIntConst("vely"), context.MkIntConst("velz"));

        for(int i = 0; i < 3; i++)
        {
            Hail hail = hails[i];
            var time = context.MkIntConst($"t{i}");

            solver.Add( context.MkEq(context.MkAdd(posx, context.MkMul(velx, time)), 
                        context.MkAdd(context.MkInt(Convert.ToInt64(hail.Position.X)), 
                        context.MkMul(context.MkInt(Convert.ToInt64(hail.Velocity.X)), time))));

            solver.Add( context.MkEq(context.MkAdd(posy, context.MkMul(vely, time)), 
                        context.MkAdd(context.MkInt(Convert.ToInt64(hail.Position.Y)), 
                        context.MkMul(context.MkInt(Convert.ToInt64(hail.Velocity.Y)), time))));

            solver.Add( context.MkEq(context.MkAdd(posz, context.MkMul(velz, time)), 
                        context.MkAdd(context.MkInt(Convert.ToInt64(hail.Position.Z)), 
                        context.MkMul(context.MkInt(Convert.ToInt64(hail.Velocity.Z)), time))));
        }

        var status = solver.Check();
        var evaluation = solver.Model.Eval(context.MkAdd(posx, context.MkAdd(posy, posz)), false); 

        var values = solver.Model.Consts.Where(c => c.Key.Name.ToString().StartsWith("pos")).Select(c => c.Value).ToList();
        return values.Select(x => Convert.ToInt64(x.ToString())).ToList();
    }

    private IEnumerable<(Hail left, Hail right)> GetPermutations(List<Hail> hails)
    {
        for(int i = 0; i < hails.Count; i++)
            for(int j = i + 1; j < hails.Count; j++)
                yield return (hails[i], hails[j]);
    }

    private bool IntersectsInRange(Hail left, Hail right, double min, double max)
    {
        var intersection = left.CalculateIntersection(right);
        return intersection.Intersects
            && (min <= intersection.coords.X && intersection.coords.X <= max )
            && (min <= intersection.coords.Y && intersection.coords.Y <= max );
    }
}