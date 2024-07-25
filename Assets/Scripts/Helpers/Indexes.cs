[System.Serializable]
public struct Indexes
{
    public int Row, Column;

    public Indexes(int row, int column)
    {
        this.Row = row;
        this.Column = column;
    }

    public override bool Equals(System.Object obj)
    {
        return obj is Indexes && this == (Indexes)obj;
    }

    public override int GetHashCode()
    {
        return System.Tuple.Create(Row, Column).GetHashCode();
    }

    public static bool operator ==(Indexes x, Indexes y)
    {
        return x.Row == y.Row && x.Column == y.Column;
    }

    public static bool operator !=(Indexes x, Indexes y)
    {
        return !(x == y);
    }
}