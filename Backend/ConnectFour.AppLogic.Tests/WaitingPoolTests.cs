using ConnectFour.AppLogic.Contracts;
using ConnectFour.Domain;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.Tests.Builders;

namespace ConnectFour.AppLogic.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "WaitingPool", @"ConnectFour.AppLogic\WaitingPool.cs")]
public class WaitingPoolTests
{
    private Mock<IGameCandidateFactory> _gameCandidateFactoryMock = null!;
    private Mock<IGameService> _gameServiceMock = null!;
    private Mock<IGameCandidateRepository> _gameCandidateRepositoryMock = null!;
    private Mock<IGameCandidateMatcher> _gameCandidateMatcherMock = null!;
    private WaitingPool _waitingPool = null!;
    private GameSettings _gameSettings = null!;
    private User _user = null!;
    private IGameCandidate _candidate = null!;
    private GameCandidateMockBuilder _candidateMockBuilder = null!;

    [SetUp]
    public void Setup()
    {
        _gameCandidateFactoryMock = new Mock<IGameCandidateFactory>();
        _gameCandidateRepositoryMock = new Mock<IGameCandidateRepository>();
        _gameCandidateMatcherMock = new Mock<IGameCandidateMatcher>();
        _gameServiceMock = new Mock<IGameService>();
        _waitingPool = new WaitingPool(
            _gameCandidateFactoryMock.Object, 
            _gameCandidateRepositoryMock.Object, 
            _gameCandidateMatcherMock.Object, 
            _gameServiceMock.Object);

        _gameSettings = new GameSettingsBuilder().Build();
        _user = new UserBuilder().Build();
        _candidateMockBuilder = new GameCandidateMockBuilder().WithUser(_user).WithSettings(_gameSettings);
        _candidate = _candidateMockBuilder.Mock.Object;
        _gameCandidateFactoryMock
            .Setup(factory => factory.CreateNewForUser(It.IsAny<User>(), It.IsAny<GameSettings>()))
            .Returns(_candidate);
    }

    [MonitoredTest("Join - Should create a candidate and add it using the game candidate repository")]
    public void Join_ShouldCreateACandidateAndAddItUsingTheGameCandidateRepository()
    {
        //Act
        _waitingPool.Join(_user, _gameSettings);

        //Assert
        _gameCandidateFactoryMock.Verify(factory => factory.CreateNewForUser(_user, _gameSettings), Times.Once,
            "The 'CreateNewForUser' method of the 'IGameCandidateFactory' is not called correctly.");

        _gameCandidateRepositoryMock.Verify(repo => repo.AddOrReplace(_candidate), Times.Once,
            "The 'AddOrReplace' method of the IGameCandidateRepository is not called correctly.");
    }

    [MonitoredTest("Join - With auto matching and other candidate waiting - Should challenge the other candidate and create a new game")]
    public void Join_WithAutoMatchingAndOtherCandidateWaiting_ShouldChallengeTheOtherCandidateAndCreateANewGame()
    {
        //Arrange
        _gameSettings = new GameSettingsBuilder().WithAutoMatching(true).Build();
        _candidateMockBuilder.WithSettings(_gameSettings);

        GameCandidateMockBuilder waitingCandidateMockBuilder = new GameCandidateMockBuilder().WithSettings(_gameSettings);
        IGameCandidate waitingCandidate = waitingCandidateMockBuilder.Mock.Object;

        IList<IGameCandidate> waitingCandidates = new List<IGameCandidate>
        {
            waitingCandidate
        };

        _gameCandidateRepositoryMock.Setup(repo => repo.FindCandidatesThatCanBeChallengedBy(It.IsAny<Guid>()))
            .Returns(waitingCandidates);

        _gameCandidateMatcherMock
            .Setup(matcher => matcher.SelectOpponentToChallenge(It.IsAny<IList<IGameCandidate>>()))
            .Returns(waitingCandidate);

        IGame game = new GameMockBuilder().Object;
        _gameServiceMock.Setup(service =>
                service.CreateGameForUsers(It.IsAny<User>(), It.IsAny<User>(), It.IsAny<GameSettings>()))
            .Returns(game);

        //Act
        _waitingPool.Join(_user, _gameSettings);

        //Assert
        _gameCandidateRepositoryMock.Verify(repo => repo.FindCandidatesThatCanBeChallengedBy(_user.Id), Times.Once,
            "The 'FindCandidatesThatCanBeChallengedBy' method of the 'IGameCandidateRepository' is not called correctly.");

        _gameCandidateMatcherMock.Verify(matcher => matcher.SelectOpponentToChallenge(waitingCandidates),
            Times.Once,
            "The 'SelectOpponentToChallenge' method of the 'IGameCandidateMatcher' is not called with the candidates that can be challenged " +
            "(returned by the 'IGameCandidateRepository').");

        _candidateMockBuilder.Mock.Verify(c => c.Challenge(waitingCandidate), Times.Once,
            "The other candidate is not challenged properly.");
        waitingCandidateMockBuilder.Mock.Verify(other => other.AcceptChallenge(_candidate), 
            "The other candidate did not accept the challenge properly.");
        _gameServiceMock.Verify(
            service => service.CreateGameForUsers(
                It.Is<User>(u => u.Id == _candidate.User.Id || u.Id == waitingCandidate.User.Id),
                It.Is<User>(u => u.Id == _candidate.User.Id || u.Id == waitingCandidate.User.Id), 
                _gameSettings),
            Times.Once, "The 'CreateGameForUsers' method of the 'IGameService' is not called correctly.");

        Assert.That(_candidate.GameId, Is.EqualTo(game.Id), "The game id is not set correctly for the joining candidate.");
        Assert.That(waitingCandidate.GameId, Is.EqualTo(game.Id), "The game id is not set correctly for the candidate that was already waiting.");
    }

    [MonitoredTest("Join - With auto matching and no other matching candidates waiting - Should not challenge another candidate")]
    public void Join_WithAutoMatchingAndNoOtherMatchingCandidatesWaiting_ShouldNotChallengeAnotherCandidate()
    {
        //Arrange
        _gameSettings = new GameSettingsBuilder().WithAutoMatching(true).Build();
        _candidateMockBuilder.WithSettings(_gameSettings);

        IList<IGameCandidate> waitingCandidates = new List<IGameCandidate>();

        _gameCandidateRepositoryMock.Setup(repo => repo.FindCandidatesThatCanBeChallengedBy(It.IsAny<Guid>()))
            .Returns(waitingCandidates);

        _gameCandidateMatcherMock
            .Setup(matcher => matcher.SelectOpponentToChallenge(It.IsAny<IList<IGameCandidate>>()))
            .Returns(() => null);

        //Act
        _waitingPool.Join(_user, _gameSettings);

        //Assert
        _gameCandidateRepositoryMock.Verify(repo => repo.FindCandidatesThatCanBeChallengedBy(_user.Id), Times.Once,
            "The 'FindCandidatesThatCanBeChallengedBy' method of the 'IGameCandidateRepository' is not called correctly.");

        _gameCandidateMatcherMock.Verify(matcher => matcher.SelectOpponentToChallenge(waitingCandidates),
            Times.Once,
            "The 'SelectOpponentToChallenge' method of the 'IGameCandidateMatcher' is not called with the candidates that can be challenged " +
            "(returned by the 'IGameCandidateRepository').");

        _candidateMockBuilder.Mock.Verify(c => c.Challenge(It.IsAny<IGameCandidate>()), Times.Never,
            "No candidate should have been challenged.");
           
        _gameServiceMock.Verify(service => service.CreateGameForUsers(It.IsAny<User>(), It.IsAny<User>(), It.IsAny<GameSettings>()),
            Times.Never, "The 'CreateGameForUsers' method of the 'IGameService' should not have been called.");
    }

    [MonitoredTest("Join - NoAutoMatching - Should not challenge another candidate")]
    public void Join_NoAutoMatching_ShouldNotChallengeAnotherCandidate()
    {
        //Arrange
        _gameSettings = new GameSettingsBuilder().WithAutoMatching(false).Build();
        _candidateMockBuilder.WithSettings(_gameSettings);

        //Act
        _waitingPool.Join(_user, _gameSettings);

        //Assert
        _gameCandidateRepositoryMock.Verify(repo => repo.FindCandidatesThatCanBeChallengedBy(It.IsAny<Guid>()), Times.Never,
            "The 'FindCandidatesThatCanBeChallengedBy' method of the 'IGameCandidateRepository' should not be called.");
    }

    [MonitoredTest("Leave - Should use the game candidate repository")]
    public void Leave_ShouldUseTheGameCandidateRepository()
    {
        //Act
        _waitingPool.Leave(_user.Id);

        //Assert
        _gameCandidateRepositoryMock.Verify(repo => repo.RemoveCandidate(_user.Id), Times.Once,
            "The 'RemoveCandidate' method of the IGameCandidateRepository is not called correctly.");
    }

    [MonitoredTest("GetCandidate - Should use the game candidate repository")]
    public void GetCandidate_ShouldUseTheGameCandidateRepository()
    {
        //Arrange
        _gameCandidateRepositoryMock.Setup(repo => repo.GetCandidate(It.IsAny<Guid>())).Returns(_candidate);

        //Act
        IGameCandidate returnedCandidate = _waitingPool.GetCandidate(_user.Id);

        //Assert
        _gameCandidateRepositoryMock.Verify(repo => repo.GetCandidate(_user.Id), Times.Once,
            "The 'GetCandidate' method of the IGameCandidateRepository is not called correctly.");
        Assert.That(_candidate, Is.SameAs(returnedCandidate),
            "The candidate returned should be the same candidate returned by the repository");
    }
}