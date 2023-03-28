using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;
using ConnectFour.Domain.PlayerDomain.Contracts;

namespace ConnectFour.Domain.PlayerDomain;

/// <inheritdoc cref="IPlayer"/>
public class PlayerBase
{
    public PlayerBase(Guid userId, string name, DiscColor color, int numberOfNormalDiscs)
    {
        
    }

    /// <summary>
    /// Creates a player that is a copy of an other player.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public PlayerBase(IPlayer otherPlayer)
    {
        //TODO: copy properties of other player
        //TODO: copy the list of special discs of the other player
        throw new NotImplementedException();
    }
}