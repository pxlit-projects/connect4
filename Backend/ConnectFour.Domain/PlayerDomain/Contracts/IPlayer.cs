using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;

namespace ConnectFour.Domain.PlayerDomain.Contracts;

/// <summary>
/// A player in a <see cref="IGame"/>
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// Unique identifier of the player
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// (Display) name of the player
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Color of the player.
    /// Red (1) or Yellow (2)
    /// </summary>
    DiscColor Color { get; }

    /// <summary>
    /// The number of normal discs the player holds currently.
    /// If the player does not have any discs left, it cannot execute a move anymore.
    /// </summary>
    int NumberOfNormalDiscs { get; }

    /// <summary>
    /// The special discs the player holds currently.
    /// If the player does not have any discs left (normal or special), it cannot execute a move anymore.
    /// </summary>
    /// <remarks>Only needed when implementing EXTRA'S</remarks>
    IReadOnlyList<IDisc> SpecialDiscs { get; }

    /// <summary>
    /// Checks if the player has a disk of a certain type.
    /// If the player does not have a disk of a type, it cannot execute moves anymore with discs of that type.
    /// </summary>
    /// <param name="discType">The type of the disk.</param>
    bool HasDisk(DiscType discType);

    /// <summary>
    /// Returns a previously played disc to the player.
    /// This can happen when the Pop-Out extra is implemented, and discs can be taken back from the grid.
    /// </summary>
    /// <param name="discType">The type of the disk</param>
    /// <remarks>Only needed when implementing EXTRA'S</remarks>
    void AddDisc(DiscType discType);

    /// <summary>
    /// Removes a disk from the player.
    /// When a disc is slide in the grid, it should be removed from the discs the player holds.
    /// </summary>
    /// <param name="discType">The type of the disk</param>
    void RemoveDisc(DiscType discType);
}