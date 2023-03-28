using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.PlayerDomain.Contracts;

namespace ConnectFour.Domain.GameDomain;

/// <inheritdoc cref="IGameFactory"/>
internal class GameFactory
{
    public GameFactory(IGamePlayStrategy gamePlayStrategy)
    {
    }
}