using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;

namespace ConnectFour.Domain.Tests.Builders;

public class GameCandidateMockBuilder : MockBuilder<IGameCandidate>
{
    public GameCandidateMockBuilder()
    {
        User user = new UserBuilder().Build();
        GameSettings settings = new GameSettingsBuilder().Build();

        Mock.SetupGet(c => c.User).Returns(user);
        Mock.SetupGet(c => c.GameSettings).Returns(settings);
        Mock.SetupProperty(c => c.GameId, Guid.Empty);
        Mock.SetupGet(c => c.ProposedOpponentUserId).Returns(Guid.Empty);
        Mock.Setup(c => c.Challenge(It.IsAny<IGameCandidate>()));
        Mock.Setup(c => c.AcceptChallenge(It.IsAny<IGameCandidate>()));
    }

    public GameCandidateMockBuilder WithUser(User user)
    {
        Mock.SetupGet(c => c.User).Returns(user);
        return this;
    }

    public GameCandidateMockBuilder WithSettings(GameSettings settings)
    {
        Mock.SetupGet(c => c.GameSettings).Returns(settings);
        return this;
    }

    public GameCandidateMockBuilder WithGameId()
    {
        Mock.SetupProperty(c => c.GameId, Guid.NewGuid());
        return this;
    }

    public GameCandidateMockBuilder WithProposedOpponentUserId(Guid userId)
    {
        Mock.SetupGet(c => c.ProposedOpponentUserId).Returns(userId);
        return this;
    }
}