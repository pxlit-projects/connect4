using ConnectFour.Domain.GridDomain.Contracts;

namespace ConnectFour.Domain.GridDomain;

/// <summary>
/// Represents a position in a <see cref="IGrid"/>.
/// </summary>
public struct GridCoordinate
{
    /// <summary>
    /// The index of the row in the grid.
    /// The top row has index 0. The second row from the top has index 1, ...
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// The index of the column in the grid.
    /// The most left column has index 0. The second column from the left has index 1, ...
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Represents a <see cref="GridCoordinate"/> that is not on the grid.
    /// </summary>
    public static GridCoordinate Empty => new GridCoordinate(-1, -1);

    public GridCoordinate(int row, int column)
    {
        Row = row;
        Column = column;
    }


    //Do not change this method
    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (!(obj is GridCoordinate other))
        {
            return false;
        }

        return Row == other.Row && Column == other.Column;
    }

    //Do not change this method
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Row.GetHashCode();
            hash = hash * 23 + Column.GetHashCode();
            return hash;
        }
    }

}