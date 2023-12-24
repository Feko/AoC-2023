using Microsoft.Z3;

namespace AOC2023.Day24;

public class Test
{
    private Context _context = new();
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
        var result = SolveWithZ3(hails);
        Assert.Equal(expected, result);
    }

    private long SolveWithZ3(List<Hail> hails)
    {
        _context = new Context(new Dictionary<string, string>() {{ "proof", "true" }});
        var solver = _context.MkSolver();
        
        (var posx, var posy, var posz) = (_context.MkIntConst("posx"), _context.MkIntConst("posy"), _context.MkIntConst("posz"));
        (var velx, var vely, var velz) = (_context.MkIntConst("velx"), _context.MkIntConst("vely"), _context.MkIntConst("velz"));

        for(int i = 0; i < 3; i++)
        {
            Hail hail = hails[i];
            var time = _context.MkIntConst($"t{i}");

            solver.Add( _context.MkEq(_context.MkAdd(posx, _context.MkMul(velx, time)), 
                        _context.MkAdd(GetZ3Int(hail.Position.X), _context.MkMul(GetZ3Int(hail.Velocity.X), time))));

            solver.Add( _context.MkEq(_context.MkAdd(posy, _context.MkMul(vely, time)),
                        _context.MkAdd(GetZ3Int(hail.Position.Y), _context.MkMul(GetZ3Int(hail.Velocity.Y), time))));

            solver.Add( _context.MkEq(_context.MkAdd(posz, _context.MkMul(velz, time)), 
                        _context.MkAdd(GetZ3Int(hail.Position.Z), _context.MkMul(GetZ3Int(hail.Velocity.Z), time))));
        }

        solver.Check();
        var evaluation = solver.Model.Eval(_context.MkAdd(posx, _context.MkAdd(posy, posz)), false); 
        return Convert.ToInt64(evaluation.ToString());
    }

    private IEnumerable<(Hail left, Hail right)> GetPermutations(List<Hail> hails)
    {
        for(int i = 0; i < hails.Count; i++)
            for(int j = i + 1; j < hails.Count; j++)
                yield return (hails[i], hails[j]);
    }

    private IntNum GetZ3Int(double val) => _context.MkInt(Convert.ToInt64(val));

    private bool IntersectsInRange(Hail left, Hail right, double min, double max)
    {
        var intersection = left.CalculateIntersection(right);
        return intersection.Intersects
            && (min <= intersection.coords.X && intersection.coords.X <= max )
            && (min <= intersection.coords.Y && intersection.coords.Y <= max );
    }
}