namespace AOC2023.Day22;

public record Position(int X, int Y, int Z)
{
    public (int X, int Y, int Z) AsTuple() => (X, Y, Z);
    public Position ModifyAxis(Axis direction, int newValue)
    {
        return direction switch
        {
            Axis.XStretch => new Position(newValue, Y, Z),
            Axis.YStretch => new Position(X, newValue, Z),
            Axis.ZStretch => new Position(X, Y, newValue),
            _ => throw new Exception("No 4D available")
        };
    }

    public int GetAxis(Axis direction)
    {
         return direction switch
        {
            Axis.XStretch => X,
            Axis.YStretch => Y,
            Axis.ZStretch => Z,
            _ => throw new Exception("No 4D available")
        };
    }

    public Position Lower() => new Position(X, Y, Z-1);
    public static Position Parse(string pos)
    {
        var ints = pos.Split(',').Select(p => Convert.ToInt32(p)).ToList();
        return new Position(ints[0], ints[1], ints[2]);
    }
};

public enum Axis 
{
    XStretch,
    YStretch,
    ZStretch,
    Unitary
}

public class Brick
{
    public Position StartPosition;
    public Position EndPosition;
    public Axis Direction;
    public List<(int x, int y, int z)> Area = new();

    public Brick(string line)
    {
        var parts = line.Split('~');
        StartPosition = Position.Parse(parts[0]);
        EndPosition = Position.Parse(parts[1]);
        Direction = GetDirection();
        CalculateArea();
    }
    private Brick(){}

    public bool GoDown(List<Brick> otherBricks)
    {
        if(MinAxis(Axis.ZStretch) == 1) //Already on the ground
            return false;
        
        int thisZ = MinAxis(Axis.ZStretch);
        if(otherBricks.Any(b => b.MaxAxis(Axis.ZStretch) == thisZ -1 && b.Supports(this))) // Can't go down, there's a brick below
            return false;
        
        Area = GetLoweringArea();
        StartPosition = StartPosition.Lower();
        EndPosition = EndPosition.Lower();
        return true;
    }

    public Brick Clone()
    {
        return new Brick()
        {
            StartPosition = this.StartPosition with {},
            EndPosition = this.EndPosition with {},
            Direction = this.Direction,
            Area = this.Area.Select(x => x).ToList()
        };
    }

    // Returns TRUE if this brick supports another brick
    public bool Supports(Brick anotherBrick) => anotherBrick != this && anotherBrick.GetLoweringArea().Intersect(Area).Any(); 
    
    private List<(int,int,int)> GetLoweringArea() => Area.Select(point => (point.x, point.y, point.z -1)).ToList();

    private void CalculateArea()
    {
        if(Direction == Axis.Unitary)
            Area.Add(StartPosition.AsTuple());
        else
        {
            (int min, int max) = (MinAxis(Direction), MaxAxis(Direction));
            Area = Enumerable.Range(min, max - min + 1).Select(i => StartPosition.ModifyAxis(Direction, i).AsTuple()).ToList();
        }
    }

    private Axis GetDirection()
    {
        if(StartPosition.X != EndPosition.X)
            return Axis.XStretch;
        
        if(StartPosition.Y != EndPosition.Y)
            return Axis.YStretch;
        
        if(StartPosition.Z != EndPosition.Z)
            return Axis.ZStretch;
        return Axis.Unitary;
    }

    public int MinAxis(Axis direction) => Math.Min(StartPosition.GetAxis(direction), EndPosition.GetAxis(direction));
    private int MaxAxis(Axis direction) => Math.Max(StartPosition.GetAxis(direction), EndPosition.GetAxis(direction));
}


public partial class BrickTests
{
    [Fact]
    public void Supports_WhenTheresBrickAbove_ShouldBeTrue()
    {
        var brickC = new Brick("0,2,3~2,2,3");
        var brickD = new Brick("0,0,4~0,2,4");
        
        bool result = brickC.Supports(brickD);
        Assert.True(result);
    }

    [Fact]
    public void Supports_WhenBricksAreNotExactlyOnTopOfEachOther_ShouldBeFalse()
    {
        var brickC = new Brick("0,2,3~2,2,3");
        var brickB = new Brick("0,0,2~2,0,2");
        
        bool result = brickB.Supports(brickC);
        Assert.False(result);
    }

    [Fact]
    public void Supports_BrickCantSupportItself()
    {
        var brick = new Brick("0,0,2~0,0,4");
        
        bool result = brick.Supports(brick);
        Assert.False(result);
    }
}