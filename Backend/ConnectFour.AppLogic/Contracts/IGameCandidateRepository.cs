using ConnectFour.Common;
using ConnectFour.Domain.GameDomain.Contracts;

namespace ConnectFour.AppLogic.Contracts;

/// <summary>
/// Stores and retrieves game candidate instances in a storage medium (e.g. in server RAM memory)
/// </summary>
public interface IGameCandidateRepository
{
    /// <summary>
    /// Adds a candidate to storage.
    /// If the candidate already exists in storage (same UserId), it is replaced in storage.
    /// </summary>
    void AddOrReplace(IGameCandidate candidate);

    /// <summary>
    /// Removes a candidate from storage.
    /// </summary>
    /// <param name="userId">The user identifier of the candidate.</param>
    void RemoveCandidate(Guid userId);

    /// <summary>
    /// Retrieve a candidate from storage.
    /// </summary>
    /// <param name="userId">The user identifier of the candidate</param>
    /// <exception cref="DataNotFoundException">Thrown if the candidate does not exist in storage.</exception>
    IGameCandidate GetCandidate(Guid userId);

    /// <summary>
    /// Finds all the other candidates in storage that can be challenged by a candidate.
    /// </summary>
    /// <param name="userId">The user identifier of the candidate that wants to find others to challenge.</param>
    IList<IGameCandidate> FindCandidatesThatCanBeChallengedBy(Guid userId);

    /// <summary>
    /// Finds the candidates that have challenged a user.
    /// </summary>
    /// <param name="challengedUserId">The identifier of the user that has possibly received challenges.</param>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// </remarks>
    IList<IGameCandidate> FindChallengesFor(Guid challengedUserId);
}