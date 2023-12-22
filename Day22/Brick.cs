namespace AOC2023.Day22;

public record Position(int X, int Y, int Z)
{
    public (int X, int Y, int Z) AsTuple() => (X, Y, Z);
    public Position ModifyAxis(BrickDirection direction, int newValue)
    {
        return direction switch
        {
            BrickDirection.XStretch => new Position(newValue, Y, Z),
            BrickDirection.YStretch => new Position(X, newValue, Z),
            BrickDirection.ZStretch => new Position(X, Y, newValue),
            _ => throw new Exception("No 4D available")
        };
    }

    public int GetAxis(BrickDirection direction)
    {
         return direction switch
        {
            BrickDirection.XStretch => X,
            BrickDirection.YStretch => Y,
            BrickDirection.ZStretch => Z,
            _ => throw new Exception("No 4D available")
        };
    }
};

public enum BrickDirection 
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
    public BrickDirection Direction;
    public List<(int x, int y, int z)> Area = new();

    public Brick(string line)
    {
        var parts = line.Split('~');
        StartPosition = ParsePosition(parts[0]);
        EndPosition = ParsePosition(parts[1]);
        Direction = GetDirection();
        CalculateArea();
    }

    // Returns TRUE if this brick supports another brick
    public bool Supports(Brick anotherBrick) => anotherBrick.GetLoweringArea().Intersect(Area).Any(); 
    
    private List<(int,int,int)> GetLoweringArea() => Area.Select(point => (point.x, point.y, point.z -1)).ToList();

    private void CalculateArea()
    {
        if(Direction == BrickDirection.Unitary)
            Area.Add(StartPosition.AsTuple());
        else
        {
            (int min, int max) = (MinAxis(Direction), MaxAxis(Direction));
            Area = Enumerable.Range(min, max - min + 1).Select(i => StartPosition.ModifyAxis(Direction, i).AsTuple()).ToList();
        }
    }

    private BrickDirection GetDirection()
    {
        if(StartPosition.X != EndPosition.X)
            return BrickDirection.XStretch;
        
        if(StartPosition.Y != EndPosition.Y)
            return BrickDirection.YStretch;
        
        if(StartPosition.Z != EndPosition.Z)
            return BrickDirection.ZStretch;
        return BrickDirection.Unitary;
    }

    private int MinAxis(BrickDirection direction) => Math.Min(StartPosition.GetAxis(direction), EndPosition.GetAxis(direction));
    private int MaxAxis(BrickDirection direction) => Math.Max(StartPosition.GetAxis(direction), EndPosition.GetAxis(direction));

    public Position ParsePosition(string pos)
    {
        var ints = pos.Split(',').Select(p => Convert.ToInt32(p)).ToList();
        return new Position(ints[0], ints[1], ints[2]);
    }
}
