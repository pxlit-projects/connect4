using ConnectFour.AppLogic.Contracts;
using ConnectFour.Domain;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;

namespace ConnectFour.AppLogic;

/// <inheritdoc cref="IGameService"/>
internal class GameService : IGameService
{
    public GameService(
        IGameFactory gameFactory,
        IGameRepository gameRepository)
    {
    }

    public IGame CreateGameForUsers(User user1, User user2, GameSettings settings)
    {
        throw new NotImplementedException();
    }

    public IGame GetById(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public void ExecuteMove(Guid gameId, Guid playerId, IMove move)
    {
        throw new NotImplementedException();
    }

    public IGame CreateSinglePlayerGameForUser(User user, GameSettings settings)
    {
        throw new NotImplementedException();
    }
}