using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GridDomain.Contracts;

namespace ConnectFour.Domain.GridDomain;

/// <inheritdoc cref="IGrid"/>
public class Grid
{
    public Grid(GameSettings settings = null)
    {
        settings = settings ?? new GameSettings();
    }

    /// <summary>
    /// Creates a grid that is a copy of an other grid.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public Grid(IGrid otherGrid)
    {
        //TODO: create a cells matrix and copy the values from the other grid
        //TODO: copy other property values
        throw new NotImplementedException();
    }
}