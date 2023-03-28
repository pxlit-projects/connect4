using ConnectFour.Domain.GridDomain.Contracts;
using ConnectFour.Domain.PlayerDomain.Contracts;

namespace ConnectFour.Domain.GameDomain.Contracts;

/// <summary>
/// A game between 2 players.
/// </summary>
public interface IGame
{
    /// <summary>
    /// Unique identifier of the game.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Player 1, the red player.
    /// </summary>
    IPlayer Player1 { get; }

    /// <summary>
    /// Player 2, the yellow player.
    /// </summary>
    IPlayer Player2 { get; }

    /// <summary>
    /// The unique identifier of the player who's turn it is
    /// </summary>
    Guid PlayerToPlayId { get;}

    /// <summary>
    /// The grid that holds the played discs.
    /// </summary>
    IGrid Grid { get; }

    /// <summary>
    /// True if the <see cref="Grid"/> contains a winning connection or if the player to play cannot execute any move.
    /// False otherwise.
    /// </summary>
    bool Finished { get; }

    /// <summary>
    /// Indicates if it is allowed to pop-out discs from the bottom of the <see cref="Grid"/>.
    /// </summary>
    bool PopOutAllowed { get; }

    /// <summary>
    /// Returns all the moves a player can execute.
    /// If it's not the player's turn, no moves will be returned.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player that wants to know its possible moves.</param>
    IReadOnlyList<IMove> GetPossibleMovesFor(Guid playerId);

    /// <summary>
    /// Executes a move on the grid, gives the turn to the opponent and removes the played disc from the disc collection of the player.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player.</param>
    /// <param name="move">The move that needs to be executed.</param>
    void ExecuteMove(Guid playerId, IMove move);

    /// <summary>
    /// Returns <see cref="Player1"/> or <see cref="Player2"/> depending on the provided <paramref name="playerId"/>.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player.</param>
    IPlayer GetPlayerById(Guid playerId);

    /// <summary>
    /// Returns <see cref="Player1"/> or <see cref="Player2"/>, the player that is not identified with <paramref name="playerId"/>.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player.</param>
    IPlayer GetOpponent(Guid playerId);

}