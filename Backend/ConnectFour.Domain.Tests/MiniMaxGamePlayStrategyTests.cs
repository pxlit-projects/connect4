using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;
using ConnectFour.Domain.PlayerDomain;

namespace ConnectFour.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "MiniMaxStrategy", @"ConnectFour.Domain\PlayerDomain\MiniMaxGamePlayStrategy.cs")]
public class MiniMaxGamePlayStrategyTests
{
    [MonitoredTest("EXTRA - GetBestMoveFor - Game is set up for red to win - Should pick the column that ensures a win for red")]
    public void _EXTRA_GetBestMoveFor_GameIsSetUpForRedToWin_ShouldPickTheColumnThatEnsuresAWinForRed()
    {
        GameSettings settings = new GameSettings();

        IGridEvaluator? gridEvaluator = new GridEvaluator() as IGridEvaluator;
        Assert.That(gridEvaluator, Is.Not.Null, "Make sure the tests on the GridEvaluator class are green first.");

        var miniMaxStrategy = new MiniMaxGamePlayStrategy(gridEvaluator, 4);

        var user = new User{ Id = Guid.NewGuid(), NickName = "TestUser"};
        IGameFactory? gameFactory = new GameFactory(miniMaxStrategy) as IGameFactory;
        Assert.That(gameFactory, Is.Not.Null, "Make sure the tests on the GameFactory class are green first.");
        IGame game = gameFactory!.CreateNewSinglePlayerGame(settings, user);

        game.ExecuteMove(game.Player1.Id, new Move(3, MoveType.SlideIn, DiscType.Normal) as IMove); //Red middle
        game.ExecuteMove(game.Player2.Id, new Move(6, MoveType.SlideIn, DiscType.Normal) as IMove); //Yellow outer right
        game.ExecuteMove(game.Player1.Id, new Move(2, MoveType.SlideIn, DiscType.Normal) as IMove); //Red middle-left
        game.ExecuteMove(game.Player2.Id, new Move(5, MoveType.SlideIn, DiscType.Normal) as IMove); //Yellow right

        IMove bestMove = miniMaxStrategy.GetBestMoveFor(game.Player1.Id, game);

        Assert.That(bestMove.Column, Is.EqualTo(1), "Player 1 (red) has 2 discs in column 2 and 3. " +
                                                    "Column 1 and 4 are empty. " +
                                                    "Sliding a disc in column 1 makes victory for red certain. " +
                                                    "The strategy should pick column 1 as the best move.");
    }
}