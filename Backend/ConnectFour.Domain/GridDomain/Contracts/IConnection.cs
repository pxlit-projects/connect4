namespace ConnectFour.Domain.GridDomain.Contracts;

/// <summary>
/// A group of aligned (horizontal, vertical or diagonal) discs of the same color.
/// </summary>
public interface IConnection
{
    /// <summary>
    /// Cell coordinate in the grid where the connection starts.
    /// </summary>
    GridCoordinate From { get; }

    /// <summary>
    /// Cell coordinate in the grid where the connection ends.
    /// </summary>
    GridCoordinate To { get; }

    /// <summary>
    /// The size of the connection.
    /// Typically this is 4. 
    /// </summary>
    int Size { get; }

    /// <summary>
    /// The color of the discs in the connection.
    /// </summary>
    DiscColor Color { get; }
}