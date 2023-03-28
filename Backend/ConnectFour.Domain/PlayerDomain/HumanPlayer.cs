using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.PlayerDomain.Contracts;

namespace ConnectFour.Domain.PlayerDomain;

/// <inheritdoc cref="IPlayer"/>
public class HumanPlayer : PlayerBase
{
    public HumanPlayer(Guid userId, string name, DiscColor color, int numberOfNormalDiscs) : base(userId, name, color, numberOfNormalDiscs) { }
}