using ConnectFour.Domain.GridDomain;

namespace ConnectFour.Domain.GameDomain.Contracts;

/// <summary>
/// Represents a move that can be made in a game.
/// </summary>
public interface IMove
{
    /// <summary>
    /// The type of the move.
    /// </summary>
    /// <remarks>
    /// For the minimal requirements the type will always be 'SlideIn'.
    /// </remarks>
    MoveType Type { get; }

    /// <summary>
    /// The type of disc that is used.
    /// </summary>
    /// <remarks>
    /// For the minimal requirements the disc type will always be 'Normal'.
    /// </remarks>
    DiscType DiscType { get; }

    /// <summary>
    /// Index of the column the disc is inserted in.
    /// The most left column has index 0.
    /// </summary>
    int Column { get; }
}