using ConnectFour.AppLogic.Contracts;
using ConnectFour.Domain;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.Tests.Builders;

namespace ConnectFour.AppLogic.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "GameService", @"ConnectFour.AppLogic\GameService.cs")]
public class GameServiceTests
{
    private Mock<IGameFactory> _gameFactoryMock;
    private Mock<IGameRepository> _gameRepositoryMock;

    private GameService _service;
    private GameSettings _gameSettings;
    private IGame _game;
    private Mock<IGame> _gameMock;

    [SetUp]
    public void Setup()
    {
        _gameMock = new GameMockBuilder().Mock;
        _game = _gameMock.Object;

        _gameFactoryMock = new Mock<IGameFactory>();
        _gameFactoryMock.Setup(factory =>
            factory.CreateNewTwoPlayerGame(It.IsAny<GameSettings>(), It.IsAny<User>(), It.IsAny<User>())).Returns(_game);
        _gameFactoryMock.Setup(factory =>
            factory.CreateNewSinglePlayerGame(It.IsAny<GameSettings>(), It.IsAny<User>())).Returns(_game);

        _gameRepositoryMock = new Mock<IGameRepository>();
        _gameRepositoryMock.Setup(repo => repo.GetById(It.IsAny<Guid>())).Returns(_game);


        _service = new GameService(
            _gameFactoryMock.Object,
            _gameRepositoryMock.Object
        );

        _gameSettings = new GameSettings();
    }

    [MonitoredTest("CreateGameForUsers - Should create a 2 player game and add it to the repository")]
    public void _01_CreateGameForUsers_ShouldCreateA2PlayerGameAndAddItToTheRepository()
    {
        //Arrange
        User user1 = new UserBuilder().Build();
        User user2 = new UserBuilder().Build();

        //Act
        _service.CreateGameForUsers(user1, user2, _gameSettings);

        //Assert
        _gameFactoryMock.Verify(factory => factory.CreateNewTwoPlayerGame(_gameSettings, user1, user2), Times.Once,
            "The 'CreateNewTwoPlayerGame' method of the 'IGameFactory' is not called with the correct users or settings.");

        _gameRepositoryMock.Verify(repo => repo.Add(_game), Times.Once,
            "The 'Add' method of the 'IGameRepository' is not called correctly.");
    }

    [MonitoredTest("GetById - Should use the repository")]
    public void _02_GetById_ShouldUseTheRepository()
    {
        //Act
        IGame retrievedGame = _service.GetById(_game.Id);

        //Assert
        _gameRepositoryMock.Verify(repo => repo.GetById(_game.Id), Times.Once,
            "The 'GetById' method of the 'IGameRepository' is not called correctly.");

        Assert.That(retrievedGame, Is.SameAs(_game),
            "The game returned should be the exact same object that is returned by the repository.");
    }

    [MonitoredTest("ExecuteMove - Should retrieve the game and execute the move on it")]
    public void _03_ExecuteMove_ShouldRetrieveTheGameAndExecuteTheMoveOnIt()
    {
        //Arrange
        IMove? move = new MoveBuilder().Build() as IMove;
        Assert.That(move, Is.Not.Null, "Make sure the tests on the Move class are green first");

        //Act
        _service.ExecuteMove(_game.Id, _game.PlayerToPlayId, move);

        //Assert
        _gameRepositoryMock.Verify(repo => repo.GetById(_game.Id), Times.Once,
            "The 'GetById' method of the 'IGameRepository' is not called correctly.");

        _gameMock.Verify(game => game.ExecuteMove(_game.PlayerToPlayId, move), Times.Once,
            $"The '{nameof(IGame.ExecuteMove)}' method of the retrieved game is not called correctly.");
    }

    [MonitoredTest("EXTRA - CreateSinglePlayerGameForUser - Should create a single player game and add it to the repository")]
    public void _EXTRA_CreateSinglePlayerGameForUser_ShouldCreateASinglePlayerGameAndAddItToTheRepository()
    {
        //Arrange
        User user = new UserBuilder().Build();

        //Act
        _service.CreateSinglePlayerGameForUser(user, _gameSettings);

        //Assert
        _gameFactoryMock.Verify(factory => factory.CreateNewSinglePlayerGame(_gameSettings, user), Times.Once,
            "The 'CreateNewSinglePlayerGame' method of the 'IGameFactory' is not called with the correct users or settings.");

        _gameRepositoryMock.Verify(repo => repo.Add(_game), Times.Once,
            "The 'Add' method of the 'IGameRepository' is not called correctly.");
    }
}