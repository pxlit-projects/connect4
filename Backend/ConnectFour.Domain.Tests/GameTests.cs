using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;
using ConnectFour.Domain.PlayerDomain.Contracts;
using ConnectFour.Domain.Tests.Builders;

namespace ConnectFour.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "Game", @"ConnectFour.Domain\GameDomain\Game.cs")]
public class GameTests
{
    private static readonly Random RandomGenerator = new Random();

    private PlayerMockBuilder _player1MockBuilder;
    private PlayerMockBuilder _player2MockBuilder;
    private GridMockBuilder _gridMockBuilder;
    private IGame _game;
    private string _iGameHash;
    private IGrid _grid;

    [OneTimeSetUp]
    public void BeforeAllTests()
    {
        _iGameHash = Solution.Current.GetFileHash(@"ConnectFour.Domain\GameDomain\Contracts\IGame.cs");
    }

    [SetUp]
    public void Setup()
    {
        _player1MockBuilder = new PlayerMockBuilder();
        _player2MockBuilder = new PlayerMockBuilder();
        _gridMockBuilder = new GridMockBuilder();
        _grid = _gridMockBuilder.Object;

        _game = new Game(_player1MockBuilder.Object, _player2MockBuilder.Object, _grid) as IGame;
    }

    [MonitoredTest("Should implement IGame")]
    public void ShouldImplementIGame()
    {
        //Assert
        AssertThatInterfaceHasNotChanged();
        Assert.That(_game, Is.Not.Null, "IGame is not implemented.");
    }

    [MonitoredTest("Constructor - Should initialize properly")]
    public void Constructor_ShouldInitializeProperly()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        //Assert
        IPlayer player1 = _player1MockBuilder.Object;
        IPlayer player2 = _player2MockBuilder.Object;

        Assert.That(_game.Player1, Is.SameAs(player1), "The 'Player1' property is not set correctly.");
        Assert.That(_game.Player2, Is.SameAs(player2), "The 'Player2' property is not set correctly.");
        Assert.That(_game.PlayerToPlayId, Is.EqualTo(player1.Id), "The 'PlayerToPlayId' should be the id of player1 after construction.");
        Assert.That(_game.Grid, Is.SameAs(_grid),
            "The 'Grid' property should return the same grid instance that was given to the constructor.");

        Assert.That(_game.Id, Is.Not.EqualTo(Guid.Empty), "The 'Id' property is not set correctly. It should be a non-empty Guid.");
        Assert.That(_game.Id, Is.EqualTo(_game.Id), "When the 'Id' property is read twice it returns a different Guid the second time. " +
                                                    "The same Guid should always be returned.");
    }

    [MonitoredTest("Finished - Grid contains winning connection - Should return true")]
    public void Finished_GridContainsWinningConnection_ShouldReturnTrue()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        _gridMockBuilder.WithWinningConnection();

        Assert.That(_game.Finished, Is.True);
    }

    [MonitoredTest("Finished - Player to play has no discs left - Should return true")]
    public void Finished_PlayerToPlayHasNoDiscsLeft_ShouldReturnTrue()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        if (_game.PlayerToPlayId == _player1MockBuilder.Object.Id)
        {
            _player1MockBuilder.WithoutDiscs();
        }
        else
        {
            _player2MockBuilder.WithoutDiscs();
        }

        Assert.That(_game.Finished, Is.True);
    }

    [MonitoredTest("Finished - Players have discs and grid does not contain a winning connection - Should return false")]
    public void Finished_PlayerHaveDiscsAndGridDoesNotContainWinningConnection_ShouldReturnFalse()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        IMove? move = new Move(0, MoveType.SlideIn, DiscType.Normal) as IMove;
        Assert.That(move, Is.Not.Null, "Make sure the tests on the Move class are green first.");
        _game.ExecuteMove(_player1MockBuilder.Object.Id, move);

        Assert.That(_game.Finished, Is.False);
    }

    [MonitoredTest("GetPlayerById - Existing id - Should return the player")]
    public void GetPlayerById_ExistingId_ShouldReturnThePlayer()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        IPlayer player1 = _game.GetPlayerById(_player1MockBuilder.Object.Id);
        IPlayer player2 = _game.GetPlayerById(_player2MockBuilder.Object.Id);

        Assert.That(player1, Is.SameAs(_game.Player1));
        Assert.That(player2, Is.SameAs(_game.Player2));
    }

    [MonitoredTest("GetPlayerById - Non existing id - Should throw InvalidOperationException")]
    public void GetPlayerById_NonExistingId_ShouldThrowInvalidOperationException()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        Assert.That(() => _game.GetPlayerById(Guid.NewGuid()), Throws.InvalidOperationException);
    }

    [MonitoredTest("GetOpponent - Existing id - Should return the other player")]
    public void GetOpponent_ExistingId_ShouldReturnTheOtherPlayer()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        IPlayer player1Opponent = _game.GetOpponent(_game.Player1.Id);
        IPlayer player2Opponent = _game.GetOpponent(_game.Player2.Id);

        Assert.That(player1Opponent, Is.SameAs(_game.Player2), "The opponent of player 1 should be player 2.");
        Assert.That(player2Opponent, Is.SameAs(_game.Player1), "The opponent of player 2 should be player 1.");
    }

    [MonitoredTest("GetOpponent - Non existing id - Should throw InvalidOperationException")]
    public void GetOpponent_NonExistingId_ShouldThrowInvalidOperationException()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        Assert.That(() => _game.GetOpponent(Guid.NewGuid()), Throws.InvalidOperationException);
    }

    [MonitoredTest("GetPossibleMovesFor - Player 1 to play - Has a normal disk - Should return moves for each grid column with empty space")]
    public void GetPossibleMovesFor_Player1ToPlay_HasNormalDisk_ShouldReturnMovesForEachGridColumnWithEmptySpace()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        //Arrange
        int numberOfColumnsWithEmptySpace = RandomGenerator.Next(2, _gridMockBuilder.Object.NumberOfColumns);
        _gridMockBuilder.WithAllColumnsFilledExcept(numberOfColumnsWithEmptySpace);

        //Act
        IReadOnlyList<IMove> moves = _game.GetPossibleMovesFor(_game.Player1.Id);

        //Assert
        Assert.That(moves, Has.Count.EqualTo(numberOfColumnsWithEmptySpace),
            $"When the grid as {numberOfColumnsWithEmptySpace} columns with empty space, {numberOfColumnsWithEmptySpace} moves should be returned.");
        Assert.That(moves, Has.All.Matches((IMove move) => move.Type == MoveType.SlideIn), "All moves should be slide-in moves.");
        Assert.That(moves, Has.All.Matches((IMove move) => move.DiscType == DiscType.Normal), "All moves should be with normal discs.");
        Assert.That(moves.Select(move => move.Column), Is.Unique, "All moves should be for different columns.");
    }

    [MonitoredTest("GetPossibleMovesFor - Not player's turn - Should return empty list")]
    public void GetPossibleMovesFor_NotPlayersTurn_ShouldReturnEmptyList()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        //Arrange
        Guid playerNotToPlayId = _game.Player2.Id;

        //Act
        IReadOnlyList<IMove> moves = _game.GetPossibleMovesFor(playerNotToPlayId);

        //Assert
        Assert.That(moves, Has.Count.Zero);
    }

    [MonitoredTest("GetPossibleMovesFor - Player has no discs left - Should return empty list")]
    public void GetPossibleMovesFor_PlayerHasNoDiscsLeft_ShouldReturnEmptyList()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        //Arrange
        _player1MockBuilder.WithoutDiscs();

        //Act
        IReadOnlyList<IMove> moves = _game.GetPossibleMovesFor(_game.Player1.Id);

        //Assert
        Assert.That(moves, Has.Count.Zero);
    }

    [MonitoredTest("ExecuteMove - Valid move - Should slide in the disc, give turn to opponent and take the disc away from the player")]
    public void ExecuteMove_ValidMove_ShouldSlideInTheDisc_GiveTurnToOpponent_AndTakeTheDiscAwayFromThePlayer()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        TestValidMove(_game.Player1.Id);
    }

    [MonitoredTest("ExecuteMove - Not player's turn - Should throw InvalidOperationException")]
    public void ExecuteMove_NotPlayersTurn_ShouldThrowInvalidOperationException()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        //Arrange
        Guid playerNotToPlayId = _game.Player2.Id;
        IMove? move = new MoveBuilder().Build() as IMove;
        Assert.That(move, Is.Not.Null, "Make sure the tests on the Move class are green first.");

        //Act + Assert
        Assert.That(() => _game.ExecuteMove(playerNotToPlayId, move), Throws.InvalidOperationException);
        Assert.That(() => _game.ExecuteMove(playerNotToPlayId, move),
            Throws.InvalidOperationException.With.Message.Length.GreaterThan(0),
            "Make sure the exception contains a meaningful message.");
    }

    [MonitoredTest("ExecuteMove - Game is finished - Should throw InvalidOperationException")]
    public void ExecuteMove_GameIsFinished_ShouldThrowInvalidOperationException()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        //Arrange
        _gridMockBuilder.WithWinningConnection();
        IMove? move = new MoveBuilder().Build() as IMove;
        Assert.That(move, Is.Not.Null, "Make sure the tests on the Move class are green first.");

        //Act + Assert
        Assert.That(() => _game.ExecuteMove(_game.Player1.Id, move), Throws.InvalidOperationException);
        Assert.That(() => _game.ExecuteMove(_game.Player1.Id, move),
            Throws.InvalidOperationException.With.Message.Length.GreaterThan(0),
            "Make sure the exception contains a meaningful message.");
    }

    [MonitoredTest("ExecuteMove - Player does not have disc - Should throw InvalidOperationException")]
    public void ExecuteMove_PlayerDoesNotHaveDisc_ShouldThrowInvalidOperationException()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        //Arrange
        IMove? move = new MoveBuilder().Build() as IMove;
        Assert.That(move, Is.Not.Null, "Make sure the tests on the Move class are green first.");

        _player1MockBuilder.WithoutDiscs();

        //Act + Assert
        Assert.That(() => _game.ExecuteMove(_game.Player1.Id, move), Throws.InvalidOperationException);
        Assert.That(() => _game.ExecuteMove(_game.Player1.Id, move),
            Throws.InvalidOperationException.With.Message.Length.GreaterThan(0),
            "Make sure the exception contains a meaningful message.");
    }

    [MonitoredTest("ExecuteMove - Multiple valid moves - Should slide in the discs and switch turns")]
    public void ExecuteMove_MultipleValidMoves_ShouldSlideInTheDiscsAndSwitchTurns()
    {
        AssertThatInterfaceHasNotChanged();
        ShouldImplementIGame();

        TestValidMove(_game.Player1.Id);
        TestValidMove(_game.Player2.Id);
        TestValidMove(_game.Player1.Id);
        TestValidMove(_game.Player2.Id);
    }

    private void AssertThatInterfaceHasNotChanged()
    {
        Assert.That(_iGameHash, Is.EqualTo("0E-AD-C4-0D-F1-FA-8C-4F-59-1E-58-F5-32-1C-D0-26"),
            "The code of the IGame interface has changed. This is not allowed. Undo your changes in 'IGame.cs'");
    }

    private void TestValidMove(Guid playerId)
    {
        //Arrange
        _player1MockBuilder.Mock.Invocations.Clear();
        _player2MockBuilder.Mock.Invocations.Clear();
        _gridMockBuilder.Mock.Invocations.Clear();

        IMove? move = new MoveBuilder().Build() as IMove;
        Assert.That(move, Is.Not.Null, "Make sure the tests on the Move class are green first.");

        PlayerMockBuilder playerMockBuilder = playerId == _player1MockBuilder.Object.Id ? _player1MockBuilder : _player2MockBuilder;
        IPlayer player = playerMockBuilder.Object;
        Guid opponentId = player.Id == _player1MockBuilder.Object.Id
            ? _player2MockBuilder.Object.Id
            : _player1MockBuilder.Object.Id;

        //Act
        _game.ExecuteMove(playerId, move);

        //Assert
        playerMockBuilder.Mock.Verify(p => p.HasDisk(move.DiscType), Times.AtLeastOnce,
            "The move should only be executed when the player has a disc");
        _gridMockBuilder.Mock.Verify(
            g => g.SlideInDisc(It.Is<IDisc>(disc => disc.Color == player.Color && disc.Type == move.DiscType),
                move.Column), Times.Once, "The 'SlideInDisc' method of the grid is not called correctly");

        Assert.That(_game.PlayerToPlayId, Is.EqualTo(opponentId), "The turn should go to the opponent");

        playerMockBuilder.Mock.Verify(p => p.RemoveDisc(move.DiscType), Times.Once,
            "The disc should be removed from the player");
    }
}