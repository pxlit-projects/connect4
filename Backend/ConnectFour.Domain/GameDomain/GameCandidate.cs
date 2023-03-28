using ConnectFour.Domain.GameDomain.Contracts;

namespace ConnectFour.Domain.GameDomain;

/// <inheritdoc cref="IGameCandidate"/>
internal class GameCandidate : IGameCandidate
{
    public User User { get; }
    public GameSettings GameSettings { get; }
    public Guid GameId { get; set; }
    public Guid ProposedOpponentUserId { get; }

    internal GameCandidate(User user, GameSettings gameSettings)
    {

    }

    public bool CanChallenge(IGameCandidate targetCandidate)
    {
        throw new NotImplementedException();
    }

    public void Challenge(IGameCandidate targetCandidate)
    {
        throw new NotImplementedException();
    }

    public void AcceptChallenge(IGameCandidate challenger)
    {
        throw new NotImplementedException();
    }

    public void WithdrawChallenge()
    {
        throw new NotImplementedException();
    }
}