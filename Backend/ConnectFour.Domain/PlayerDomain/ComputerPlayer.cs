using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.PlayerDomain.Contracts;

namespace ConnectFour.Domain.PlayerDomain;

/// <inheritdoc cref="IPlayer"/>
public class ComputerPlayer : PlayerBase
{
    public ComputerPlayer(DiscColor color, int numberOfNormalDiscs, IGamePlayStrategy strategy) : base(Guid.NewGuid(), "Computer", color, numberOfNormalDiscs)
    {
    }

    /// <summary>
    /// Uses gameplay strategy to determine the best move to execute.
    /// </summary>
    /// <param name="game">The game (in its current state)</param>
    public IMove DetermineBestMove(IGame game)
    {
        throw new NotImplementedException();
    }
}