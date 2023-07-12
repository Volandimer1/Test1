[System.Serializable]
public struct indexes
{
    public int i, j;

    public indexes(int i, int j)
    {
        this.i = i;
        this.j = j;
    }

    public override bool Equals(System.Object obj)
    {
        return obj is indexes && this == (indexes)obj;
    }

    public override int GetHashCode()
    {
        return System.Tuple.Create(i, j).GetHashCode();
    }

    public static bool operator ==(indexes x, indexes y)
    {
        return x.i == y.i && x.j == y.j;
    }

    public static bool operator !=(indexes x, indexes y)
    {
        return !(x == y);
    }
}