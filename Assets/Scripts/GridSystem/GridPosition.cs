using System;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int z;

    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public override string ToString()
    {
        return "x: " + x + "; z: " + z;
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        bool status = false;
        if (a.x == b.x && a.z == b.z)
        {
            status = true;
        }
        return status;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.z + b.z);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.z - b.z);
    }

    public override bool Equals(object o)
    {
        if (o == null || !(o is GridPosition))
        {
            return false;
        }

        if (this.x == ((GridPosition)o).x && this.z == ((GridPosition)o).z)
        {
            return true;
        }

        return false;

        //return o is GridPosition position && x == position.x && z == position.z;
    }

    public override int GetHashCode()
    {
        return x * z;
        //return HashCode.Combine(x, z);
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }
}
