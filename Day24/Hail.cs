namespace AOC2023.Day24;

public record Coordinates(double X, double Y, double Z)
{
    public static Coordinates Parse(string line)
    {
        var ints = line.Split(", ").Select(x => Convert.ToDouble(x)).ToList();
        return new Coordinates(ints[0], ints[1], ints[2]);
    }

    public Coordinates Multiply(double factor) => new(X * factor, Y * factor, Z * factor);
    public Coordinates Sum(Coordinates other) => new(X + other.X, Y + other.Y, Z + other.Z);
}

public record IntersectionResult(bool Intersects, Coordinates? coords);

public class Hail
{
    public Coordinates Position;
    public Coordinates Velocity;

    public Hail(string line)
    {
        var parts = line.Split(" @ ");
        Position = Coordinates.Parse(parts[0]);
        Velocity = Coordinates.Parse(parts[1]);
    }

    public IntersectionResult CalculateIntersection(Hail anotherHail)
    {
        double denominator = (anotherHail.Velocity.X * Velocity.Y) - (anotherHail.Velocity.Y * Velocity.X);
        if(denominator == 0)
            return new IntersectionResult(false, default(Coordinates));

        double timeA = ((Velocity.Y * (Position.X - anotherHail.Position.X)) - (Velocity.X * (Position.Y - anotherHail.Position.Y))) / denominator;
        double timeB = -((anotherHail.Velocity.Y * (anotherHail.Position.X - Position.X)) - (anotherHail.Velocity.X * (anotherHail.Position.Y - Position.Y))) / denominator;

        if(timeA < 0 || timeB < 0)
            return new IntersectionResult(false, default(Coordinates));

        return new IntersectionResult(true, GetPositionAtTIme(timeB));
    }

    private Coordinates GetPositionAtTIme(double time)
    {
        var vel = Velocity.Multiply(time);
        return Position.Sum(vel);
    }
}

public class HailTests
{
    [Fact]
    public void Given_two_hails_should_intersect()
    {
        var hailA = new Hail("19, 13, 30 @ -2,  1, -2");
        var hailB = new Hail("18, 19, 22 @ -1, -1, -2");

        var result = hailA.CalculateIntersection(hailB);

        Assert.True(result.Intersects);
        Assert.Equal(14.333333, result.coords.X, 0.001);
        Assert.Equal(15.333333, result.coords.Y, 0.001);
    }

    [Fact]
    public void Given_two_hails_which_intersected_in_the_past_should_be_false()
    {
        var hailA = new Hail("19, 13, 30 @ -2,  1, -2");
        var hailB = new Hail("20, 19, 15 @ 1, -5, -3");

        var result = hailA.CalculateIntersection(hailB);

        Assert.False(result.Intersects);
    }

    [Fact]
    public void Given_two_hails_which_never_intersect_should_be_false()
    {
        var hailA = new Hail("18, 19, 22 @ -1, -1, -2");
        var hailB = new Hail("20, 25, 34 @ -2, -2, -4");

        var result = hailA.CalculateIntersection(hailB);

        Assert.False(result.Intersects);
    }
}