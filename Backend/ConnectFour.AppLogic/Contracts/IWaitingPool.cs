using ConnectFour.Common;
using ConnectFour.Domain;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;

namespace ConnectFour.AppLogic.Contracts;

/// <summary>
/// Manage the game candidates that want to play a game.
/// </summary>
public interface IWaitingPool
{
    /// <summary>
    /// Adds a user as a game candidate to the pool.
    /// </summary>
    /// <param name="user">The user that wants to play a game.</param>
    /// <param name="gameSettings">The game settings that the user desires.</param>
    void Join(User user, GameSettings gameSettings);

    /// <summary>
    /// Removes tha candidacy of a user from the pool. 
    /// </summary>
    /// <param name="userId">The identifier of the user to remove.</param>
    void Leave(Guid userId);

    /// <summary>
    /// Retrieves the candidacy of a user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <exception cref="DataNotFoundException">Thrown when the user is not in the pool.</exception>
    IGameCandidate GetCandidate(Guid userId);

    /// <summary>
    /// Challenges a user to a game.
    /// </summary>
    /// <param name="challengerUserId">The identifier of the user that challenges.</param>
    /// <param name="targetUserId">The identifier of the user that is being challenged.</param>
    void Challenge(Guid challengerUserId, Guid targetUserId);

    /// <summary>
    /// Searches for other candidates in the pool that have requested the same game settings and thus are candidates that can be challenged.
    /// </summary>
    /// <param name="userId">The identifier of the user that wants to find candidates to challenge.</param>
    IList<IGameCandidate> FindCandidatesThatCanBeChallengedBy(Guid userId);

    /// <summary>
    /// Undo a <see cref="Challenge"/>.
    /// </summary>
    /// <param name="userId">The identifier of the user that made a challenge.</param>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// </remarks>
    void WithdrawChallenge(Guid userId);

    /// <summary>
    /// Searches for candidates that have challenged a <paramref name="challengedUserId">user</paramref>.
    /// </summary>
    /// <param name="challengedUserId">The identifier of the user that possibly has been challenged by one or more candidates.</param>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// </remarks>
    IList<IGameCandidate> FindChallengesFor(Guid challengedUserId);
}