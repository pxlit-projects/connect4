using ConnectFour.AppLogic.Contracts;
using ConnectFour.Domain;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;

namespace ConnectFour.AppLogic;

/// <inheritdoc cref="IWaitingPool"/>
internal class WaitingPool : IWaitingPool
{
    public WaitingPool(
        IGameCandidateFactory gameCandidateFactory,
        IGameCandidateRepository gameCandidateRepository, 
        IGameCandidateMatcher gameCandidateMatcher, 
        IGameService gameService)
    {
    }

    public void Join(User user, GameSettings gameSettings)
    {
        throw new NotImplementedException();
    }

    public void Leave(Guid userId)
    {
        throw new NotImplementedException();
    }

    public IGameCandidate GetCandidate(Guid userId)
    {
        throw new NotImplementedException();
    }

    public void Challenge(Guid challengerUserId, Guid targetUserId)
    {
        throw new NotImplementedException();
    }

    public IList<IGameCandidate> FindCandidatesThatCanBeChallengedBy(Guid userId)
    {
        throw new NotImplementedException();
    }

    public void WithdrawChallenge(Guid userId)
    {
        throw new NotImplementedException();
    }

    public IList<IGameCandidate> FindChallengesFor(Guid challengedUserId)
    {
        throw new NotImplementedException();
    }
}