using ConnectFour.AppLogic.Contracts;
using ConnectFour.Domain.GameDomain.Contracts;

namespace ConnectFour.AppLogic;

/// <inheritdoc cref="IGameCandidateMatcher"/>
public class BasicGameCandidateMatcher : IGameCandidateMatcher
{
    public IGameCandidate SelectOpponentToChallenge(IList<IGameCandidate> possibleOpponents)
    {
        throw new NotImplementedException();
    }
}