namespace ConnectFour.Domain.GridDomain.Contracts;

/// <summary>
/// Grid that holds the player discs during a game.
/// </summary>
public interface IGrid
{
    /// <summary>
    /// Number of rows in the grid.
    /// Typically 6.
    /// </summary>
    int NumberOfRows { get; }

    /// <summary>
    /// Number of columns in the grid.
    /// Typically 7.
    /// </summary>
    int NumberOfColumns { get; }

    /// <summary>
    /// Number of discs of the same color that need to be aligned to form a winning connection.
    /// </summary>
    int WinningConnectSize { get; }

    /// <summary>
    /// The cells of the grid. This is a <see cref="NumberOfRows"/> x <see cref="NumberOfColumns"/> matrix.
    /// If null, then the cell is empty.
    /// Otherwise the cell is occupied with a <see cref="IDisc"/>.
    /// </summary>
    IDisc[,] Cells { get; }

    /// <summary>
    /// Holds the winning connections in the grid.
    /// In a normal game this list holds zero or one winning connection.
    /// But in a special games (e.g. when pop-out is enabled) it is possible that there are multiple winning connections.
    /// This list may change each time <see cref="SlideInDisc"/> or <see cref="PopOutDisc"/> is called.
    /// </summary>
    IReadOnlyList<IConnection> WinningConnections { get; }

    /// <summary>
    /// Slides a disc in one of the columns of the grid (from the top).
    /// The disc 'falls down' until it is in a cell above a cell occupied with a disc or until it is at the bottom cell.
    /// </summary>
    /// <param name="disc">The disc being slide in.</param>
    /// <param name="column">The index of column of the grid</param>
    void SlideInDisc(IDisc disc, int column);

    /// <summary>
    /// Removes a disc from a column of the grid (the one at the bottom row). All discs above it 'fall' one cell down.
    /// </summary>
    /// <param name="disc">The disc being popped out.</param>
    /// <param name="column">The index of column of the grid.</param>
    /// <remarks>This is an EXTRA</remarks>
    void PopOutDisc(IDisc disc, int column);
}