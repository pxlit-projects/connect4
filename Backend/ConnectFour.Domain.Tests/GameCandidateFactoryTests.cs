using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.Tests.Builders;

namespace ConnectFour.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "GameCandidateFactory", @"ConnectFour.Domain\GameDomain\GameCandidateFactory.cs")]
public class GameCandidateFactoryTests
{
    private GameCandidateFactory _factory;

    [SetUp]
    public void Setup()
    {
        _factory = new GameCandidateFactory();
    }

    [MonitoredTest("CreateNewForUser - Should create candidate for user")]
    public void CreateNewForUser_ShouldCreateCandidateForUser()
    {
        //Arrange
        User user = new UserBuilder().Build();
        GameSettings settings = new GameSettingsBuilder().Build();

        //Act
        IGameCandidate candidate = _factory.CreateNewForUser(user, settings);

        //Assert
        Assert.That(candidate.User, Is.SameAs(user), "The 'User' property is not set correctly.");
        Assert.That(candidate.GameSettings, Is.SameAs(settings), "The 'GameSettings' property is not set correctly.");
    }
}