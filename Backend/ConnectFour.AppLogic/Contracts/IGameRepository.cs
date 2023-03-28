using ConnectFour.Common;
using ConnectFour.Domain.GameDomain.Contracts;

namespace ConnectFour.AppLogic.Contracts;

/// <summary>
/// Stores and retrieves game instances in a storage medium (e.g. in server RAM memory)
/// </summary>
/// <remarks>
/// Implemented by the InMemoryGameRepository class in the ConnectFour.Infrastructure layer
/// </remarks>
public interface IGameRepository
{
    /// <summary>
    /// Adds a game
    /// </summary>
    /// <param name="newGame">The game to be stored</param>
    void Add(IGame newGame);

    /// <summary>
    /// Retrieves a game from storage
    /// </summary>
    /// <param name="id">The unique identifier of the game</param>
    /// <returns>The matching game if it exists</returns>
    /// <exception cref="DataNotFoundException">Thrown when no matching game can be found</exception>
    IGame GetById(Guid id);
}