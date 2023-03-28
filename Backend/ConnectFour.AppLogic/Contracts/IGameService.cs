using ConnectFour.Common;
using ConnectFour.Domain;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;

namespace ConnectFour.AppLogic.Contracts;

/// <summary>
/// Service to manipulate all the games in the application
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Creates and stores a new game for 2 players (users)
    /// </summary>
    /// <param name="user1">The user that will be player 1</param>
    /// <param name="user2">The user that will be player 2</param>
    /// <param name="settings">The settings that should be used when creating the game</param>
    IGame CreateGameForUsers(User user1, User user2, GameSettings settings);

    /// <summary>
    /// Retrieves a game from storage
    /// </summary>
    /// <param name="gameId">The unique identifier of the game</param>
    /// <returns>The matching game if it exists</returns>
    /// <exception cref="DataNotFoundException">Thrown when no matching game can be found</exception>
    IGame GetById(Guid gameId);

    /// <summary>
    /// Retrieves a game from storage and executes a move on it
    /// </summary>
    /// <param name="gameId">The unique identifier of the game</param>
    /// <param name="playerId">The unique identifier of the player that makes the move</param>
    /// <param name="move">The move that should be made</param>
    void ExecuteMove(Guid gameId, Guid playerId, IMove move);

    /// <summary>
    /// Creates and stores a new game for 1 player (user) against a computer player (AI)
    /// </summary>
    /// <param name="user">The user that will be player 1</param>
    /// <param name="settings">The settings that should be used when creating the game</param>
    IGame CreateSinglePlayerGameForUser(User user, GameSettings settings);

}