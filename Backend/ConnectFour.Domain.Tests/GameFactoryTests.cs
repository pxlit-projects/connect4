using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.PlayerDomain;
using ConnectFour.Domain.PlayerDomain.Contracts;
using ConnectFour.Domain.Tests.Builders;

namespace ConnectFour.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "GameFactory", @"ConnectFour.Domain\GameDomain\GameFactory.cs")]
public class GameFactoryTests
{
    private IGameFactory _factory;
    private Mock<IGamePlayStrategy> _gamePlayStrategyMock;

    [SetUp]
    public void BeforeEachTest()
    {
        _gamePlayStrategyMock = new Mock<IGamePlayStrategy>();
        _factory = new GameFactory(_gamePlayStrategyMock.Object) as IGameFactory;
    }

    [MonitoredTest("Should implement IGameFactory")]
    public void _01_ShouldImplementIGameFactory()
    {
        //Assert
        Assert.That(_factory, Is.Not.Null, "IGameFactory is not implemented");
    }

    [MonitoredTest("CreateNewTwoPlayerGame - Should create game with 2 human players")]
    public void _02_CreateNewTwoPlayerGame_ShouldCreateGameWith2HumanPlayers()
    {
        _01_ShouldImplementIGameFactory();

        //Arrange
        GameSettings settings = new GameSettingsBuilder().Build();
        int expectedNumberOfNormalDiscs = (settings.GridRows * settings.GridColumns) / 2;
        User user1 = new UserBuilder().Build();
        User user2 = new UserBuilder().Build();

        //Act
        IGame game = _factory.CreateNewTwoPlayerGame(settings, user1, user2);

        //Assert
        Assert.That(game.Player1, Is.InstanceOf<HumanPlayer>(), "Player 1 should be an instance of 'HumanPlayer'.");
        Assert.That(game.Player1.Id, Is.EqualTo(user1.Id), "The 'Id' of player 1 should be the id of user 1.");
        Assert.That(game.Player1.Name, Is.EqualTo(user1.NickName), "The 'Name' of player 1 should be the nickname of user 1.");
        Assert.That(game.Player1.Color, Is.EqualTo(DiscColor.Red), "The 'Color' of player 1 should be red.");
        Assert.That(game.Player1.NumberOfNormalDiscs, Is.EqualTo(expectedNumberOfNormalDiscs),
            $"The 'NumberOfNormalDiscs' of player 1 should be {expectedNumberOfNormalDiscs}.");

        Assert.That(game.Player2, Is.InstanceOf<HumanPlayer>(), "Player 2 should be an instance of 'HumanPlayer'.");
        Assert.That(game.Player2.Id, Is.EqualTo(user2.Id), "The 'Id' of player 2 should be the id of user 2.");
        Assert.That(game.Player2.Name, Is.EqualTo(user2.NickName), "The 'Name' of player 2 should be the nickname of user 2.");
        Assert.That(game.Player2.Color, Is.EqualTo(DiscColor.Yellow), "The 'Color' of player 2 should be yellow.");
        Assert.That(game.Player2.NumberOfNormalDiscs, Is.EqualTo(expectedNumberOfNormalDiscs),
            $"The 'NumberOfNormalDiscs' of player 2 should be {expectedNumberOfNormalDiscs}.");

        Assert.That(game.Grid, Is.Not.Null, "The created game should have a Grid");
        Assert.That(game.Grid.NumberOfColumns, Is.EqualTo(settings.GridColumns),
            "The number of columns of the game Grid should be the amount provided by the GameSettings");
        Assert.That(game.Grid.NumberOfRows, Is.EqualTo(settings.GridRows),
            "The number of rows of the game Grid should be the amount provided by the GameSettings");
    }

    [MonitoredTest("CreateNewTwoPlayerGame - Player 1 and 2 are the same - Should throw InvalidOperationException")]
    public void _03_CreateNewTwoPlayerGame_Player1And2AreTheSame_ShouldThrowInvalidOperationException()
    {
        _01_ShouldImplementIGameFactory();

        //Arrange
        GameSettings settings = new GameSettingsBuilder().Build();
        User user = new UserBuilder().Build();
        User sameUser = new UserBuilder().AsCloneOf(user).Build();

        //Act
        Assert.That(() => _factory.CreateNewTwoPlayerGame(settings, user, sameUser),
            Throws.TypeOf<InvalidOperationException>());
    }
}