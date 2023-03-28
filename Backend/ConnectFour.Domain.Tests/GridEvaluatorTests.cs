using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;
using ConnectFour.Domain.Tests.Builders;
using ConnectFour.Domain.Tests.Extensions;

namespace ConnectFour.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "GridEvaluator", @"ConnectFour.Domain\GridDomain\GridEvaluator.cs")]
public class GridEvaluatorTests
{
    private IGridEvaluator _evaluator = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _evaluator = (new GridEvaluator() as IGridEvaluator)!;
    }

    [MonitoredTest("EXTRA - Should implement IGridEvaluator")]
    public void _EXTRA_ShouldImplementIGridEvaluator()
    {
        var gridEvaluatorType = typeof(GridEvaluator);
        Assert.That(gridEvaluatorType.IsAssignableTo(typeof(IGridEvaluator)), "The interface IGridEvaluator is not implemented");
    }

    [MonitoredTest("EXTRA - CalculateScore - Grid contains a winning connection - Should return int.MaxValue or int.MinValue")]
    [TestCase("0 0 0 0 0\n" +
              "0 0 2 0 0\n" +
              "0 0 2 0 0\n" +
              "0 0 2 0 0\n" +
              "1 1 2 1 1", DiscColor.Red, int.MinValue)]
    [TestCase("0 0 0 0 1\n" +
              "0 0 2 1 1\n" +
              "0 0 1 2 2\n" +
              "0 1 2 1 2\n" +
              "2 1 2 1 1", DiscColor.Red, int.MaxValue)]
    [TestCase("0 2 0 0 0\n" +
              "0 2 0 0 0\n" +
              "0 2 1 0 0\n" +
              "0 2 1 0 0\n" +
              "1 1 1 0 0", DiscColor.Yellow, int.MaxValue)]
    public void _EXTRA_CalculateScore_GridContainsWinningConnection_ShouldReturnMaximumOrMinimumScore(string cellConfiguration, DiscColor maximizingColor, int expectedScore)
    {
        _EXTRA_ShouldImplementIGridEvaluator();

        GameSettings settings = new GameSettingsBuilder().WithGridDimensions(5, 5).Build();
        IGrid grid = new GridBuilder(settings).WithCellConfiguration(cellConfiguration, useSlideInMethod: true).Build();

        int score = _evaluator.CalculateScore(grid, maximizingColor);

        Assert.That(score, Is.EqualTo(expectedScore),
            () => expectedScore < 0
                ? $"When the opponent of the maximizing color ({maximizingColor}) has a winning connection, then int.MinValue should be returned."
                : $"When the maximizing color ({maximizingColor}) has a winning connection, then int.MaxValue should be returned.");
    }

    [MonitoredTest("EXTRA - CalculateScore - Should reward 'near connections'")]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "0 0 0 2 0\n" +
              "0 2 0 2 0\n" +
              "1 1 0 1 0", DiscColor.Red, DiscColor.Red)]
    [TestCase("0 0 0 0 0\n" +
              "0 2 0 0 0\n" +
              "0 1 2 0 0\n" +
              "0 1 1 2 0\n" +
              "0 1 2 1 0", DiscColor.Red, DiscColor.Yellow)]
    [TestCase("0 0 0 0 0\n" +
              "0 1 1 1 0\n" +
              "0 2 2 2 0\n" +
              "0 1 2 1 1\n" +
              "0 2 1 1 2", DiscColor.Yellow, DiscColor.Yellow)]
    public void _EXTRA_CalculateScore_ShouldRewardNearConnections(string cellConfiguration, DiscColor maximizingColor, DiscColor winningColor)
    {
        _EXTRA_ShouldImplementIGridEvaluator();

        GameSettings settings = new GameSettingsBuilder().WithGridDimensions(5, 5).Build();
        IGrid grid = new GridBuilder(settings).WithCellConfiguration(cellConfiguration, useSlideInMethod: true).Build();
        bool expectPositiveScore = maximizingColor == winningColor;

        int score = _evaluator.CalculateScore(grid, maximizingColor);

        if (expectPositiveScore)
        {
            Assert.That(score, Is.GreaterThan(0), $"When the maximizing color ({maximizingColor}) has more 'near connections', the score should be positive.");
        }
        else
        {
            Assert.That(score, Is.LessThan(0), $"When the maximizing color ({maximizingColor}) has less 'near connections', the score should be negative.");
        }
    }

    [MonitoredTest("EXTRA - CalculateScore - No player is winning - Should return a score near zero")]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 0 1\n" +
              "0 0 0 0 2\n" +
              "0 0 0 0 2\n" +
              "2 1 1 1 2")]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "2 0 0 0 1\n" +
              "2 2 0 1 1")]
    [TestCase("2 1 1 1 2\n" +
              "2 1 2 1 2\n" +
              "1 2 1 2 1\n" +
              "1 2 1 2 1\n" +
              "1 2 1 2 1")]
    public void _EXTRA_CalculateScore_NoPlayerIsWinning_ShouldReturnScoreNearZero(string cellConfiguration)
    {
        _EXTRA_ShouldImplementIGridEvaluator();

        GameSettings settings = new GameSettingsBuilder().WithGridDimensions(5, 5).Build();
        IGrid grid = new GridBuilder(settings).WithCellConfiguration(cellConfiguration, useSlideInMethod: true).Build();

        DiscColor color = Enum.GetValues<DiscColor>().NextRandomElement();
        int score = _evaluator.CalculateScore(grid, color);

        Assert.That(score, Is.EqualTo(0).Within(10), $"The following grid\n{cellConfiguration}\n should have a score near zero (maximizing color = {color}).");
    }
}