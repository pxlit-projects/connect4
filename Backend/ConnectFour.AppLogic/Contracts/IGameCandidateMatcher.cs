using ConnectFour.Domain.GameDomain.Contracts;

namespace ConnectFour.AppLogic.Contracts;

/// <summary>
/// Matches <see cref="IGameCandidate">game candidates</see> with each other.
/// </summary>
public interface IGameCandidateMatcher
{
    /// <summary>
    /// Selects an opponent to challenge from a <paramref name="possibleOpponents">list of candidates</paramref>
    /// </summary>
    IGameCandidate SelectOpponentToChallenge(IList<IGameCandidate> possibleOpponents);
}