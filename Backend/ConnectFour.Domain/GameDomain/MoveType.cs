namespace ConnectFour.Domain.GameDomain;

/// <summary>
/// Type of moves that can be made in a connect4 game.
/// </summary>
public enum MoveType
{
    /// <summary>
    /// Slide a disc in the top of a grid column.
    /// </summary>
    SlideIn = 1,

    /// <summary>
    /// Pop a disc, at the bottom of a grid column, out of the grid.
    /// This will cause the discs above to drop one row lower.
    /// </summary>
    PopOut = 2
}