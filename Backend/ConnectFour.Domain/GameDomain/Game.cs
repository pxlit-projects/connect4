using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain.Contracts;
using ConnectFour.Domain.PlayerDomain.Contracts;

namespace ConnectFour.Domain.GameDomain;

/// <inheritdoc cref="IGame"/>
internal class Game
{
    public Game(IPlayer player1, IPlayer player2, IGrid grid, bool popOutAllowed = false)
    {

    }

    /// <summary>
    /// Creates a game that is a copy of an other game.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public Game(IGame otherGame)
    {
        //TODO: make a copy of the players
        //TODO: make a copy of the grid
        //TODO: initialize the properties with the copies
        throw new NotImplementedException();
    }
}