namespace ConnectFour.Domain.GameDomain.Contracts;

/// <summary>
/// Can create an <see cref="IGame"/>.
/// </summary>
public interface IGameFactory
{
    /// <summary>
    /// Creates a 2 player game. Human against human.
    /// </summary>
    /// <param name="settings">The settings for the game.</param>
    /// <param name="user1">The user that will be player 1 (the red player).</param>
    /// <param name="user2">The user that will be player 2 (the yellow player).</param>
    IGame CreateNewTwoPlayerGame(GameSettings settings, User user1, User user2);

    /// <summary>
    /// Creates a single player game against a computer player (AI).
    /// </summary>
    /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
    IGame CreateNewSinglePlayerGame(GameSettings settings, User user);
}