using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;
using ConnectFour.Domain.Tests.Builders;
using System.Reflection;

namespace ConnectFour.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "Grid", @"ConnectFour.Domain\GridDomain\Grid.cs")]
public class GridTests
{
    [MonitoredTest("Should implement IGrid")]
    public void ShouldImplementIGrid()
    {
        var gridType = typeof(Grid);
        Assert.That(gridType.IsAssignableTo(typeof(IGrid)), "The interface IGrid is not implemented");
    }

    [MonitoredTest("The properties NumberOfRows, NumberOfColumns, WinningConnectSize, Cells and WinningConnections should only have a getter")]
    public void Properties_NumberOfRows_NumberOfColumns_WinningConnectSize_Cells_WinningConnections_ShouldOnlyHaveAGetter()
    {
        ShouldImplementIGrid();
        var gridType = typeof(Grid);

        AssertIsReadOnlyProperty(gridType.GetProperty(nameof(IGrid.NumberOfRows))!);
        AssertIsReadOnlyProperty(gridType.GetProperty(nameof(IGrid.NumberOfColumns))!);
        AssertIsReadOnlyProperty(gridType.GetProperty(nameof(IGrid.WinningConnectSize))!);
        AssertIsReadOnlyProperty(gridType.GetProperty(nameof(IGrid.Cells))!);
        AssertIsReadOnlyProperty(gridType.GetProperty(nameof(IGrid.WinningConnections))!);
    }

    [MonitoredTest("Constructor - GameSettings passed in - Should initialize correctly")]
    public void Constructor_GameSettingsPassedIn_ShouldInitializeCorrectly()
    {
        ShouldImplementIGrid();

        GameSettings settings = new GameSettingsBuilder().Build();

        IGrid grid = (new Grid(settings) as IGrid)!;

        Assert.That(grid.NumberOfRows, Is.EqualTo(settings.GridRows),
            "Grid should have the same number of rows as provided in the settings parameter.");
        Assert.That(grid.NumberOfColumns, Is.EqualTo(settings.GridColumns),
            "Grid should have the same number of columns as provided in the settings parameter.");
        Assert.That(grid.WinningConnectSize, Is.EqualTo(settings.ConnectionSize),
            "The WinningConnectSize of the grid should be the ConnectionSize in the settings parameter.");
        Assert.That(grid.WinningConnections is not null && grid.WinningConnections.Count == 0, Is.True,
            "The WinningConnections of the grid should be an empty list after initialization");

        int rowCount = grid.Cells.GetLength(0);
        int columnCount = grid.Cells.GetLength(1);
        Assert.That(rowCount == settings.GridRows && columnCount == settings.GridColumns, Is.True,
            $"The Cells of the grid should be a {settings.GridRows}x{settings.GridColumns} matrix");

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                Assert.That(grid.Cells[i, j], Is.Null, "All cells should be null");
            }
        }
    }

    [MonitoredTest("SlideInDisc - Insert a disc in each column - Each disc should slide down as far as possible")]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 0 2\n" +
              "0 0 0 1 2\n" +
              "0 0 2 2 1\n" +
              "0 1 2 1 1", "(4,0) (3,1) (2,2) (1,3) (0,4)")]
    public void SlideInDisc_InsertDiscInEachColumn_EachDiscShouldSlideDownAsFarAsPossible(string cellConfiguration, string targetCoordinates)
    {
        ShouldImplementIGrid();

        GameSettings settings = new GameSettingsBuilder().WithGridDimensions(5, 5).Build();
        IGrid grid = new GridBuilder(settings).WithCellConfiguration(cellConfiguration).Build();

        IList<GridCoordinate> expectedCoordinates = ParseCoordinateText(targetCoordinates);

        for (int column = 0; column < grid.NumberOfColumns; column++)
        {
            IDisc? disc = new Disc(DiscType.Normal, DiscColor.Red) as IDisc;
            Assert.That(disc, Is.Not.Null, "Make sure the tests on the Disc class are green first.");

            grid.SlideInDisc(disc, column);

            GridCoordinate expectedCoordinate = expectedCoordinates[column];
            IDisc discOnExpectedCoordinate = grid.Cells[expectedCoordinate.Row, expectedCoordinate.Column];

            Assert.That(discOnExpectedCoordinate, Is.SameAs(disc), () =>
            {
                string message = $"For the following start situation (0=empty,1=red,2=yellow):\n{cellConfiguration}\n " +
                                 $"When a disc is inserted in column with index {column} \n" +
                                 $"then the cell at position ({expectedCoordinate.Row},{expectedCoordinate.Column}) should contain the inserted disc instance.";
                return message;
            });
        }

    }


    [MonitoredTest("SlideInDisc - Column is full - Should throw InvalidOperationException")]
    [TestCase("2 1 2\n" +
              "1 2 1\n" +
              "2 1 2")]
    public void SlideInDisc_ColumnIsFull_ShouldThrowInvalidOperationException(string cellConfiguration)
    {
        ShouldImplementIGrid();

        GameSettings settings = new GameSettingsBuilder().WithGridDimensions(3, 3).Build();
        IGrid grid = new GridBuilder(settings).WithCellConfiguration(cellConfiguration).Build();


        for (int column = 0; column < grid.NumberOfColumns; column++)
        {
            IDisc? disc = new Disc(DiscType.Normal, DiscColor.Red) as IDisc;
            Assert.That(disc, Is.Not.Null, "Make sure the tests on the Disc class are green first.");

            Assert.That(() => grid.SlideInDisc(disc, column), Throws.InvalidOperationException, () =>
            {
                string message = $"For the following start situation (0=empty,1=red,2=yellow):\n{cellConfiguration}\n " +
                                 $"When a disc is inserted in column with index {column} \n" +
                                 "an InvalidOperationException should be thrown.";
                return message;
            });


        }

    }

    [MonitoredTest("SlideInDisc - Creates a connection - Should detect the winning connection")]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "0 1 0 1 0\n" +
              "1 2 2 2 0\n" +
              "2 1 2 1 1", 4, DiscColor.Yellow, "(3,1) (3,4)")]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "2 2 0 0 2\n" +
              "1 1 0 1 2", 2, DiscColor.Red, "(4,0) (4,3)")]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "0 1 0 0 0\n" +
              "0 1 0 0 0\n" +
              "0 1 2 2 2", 1, DiscColor.Red, "(1,1) (4,1)")]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 2 0\n" +
              "0 0 0 2 0\n" +
              "0 0 0 2 1\n" +
              "0 0 0 1 1", 3, DiscColor.Yellow, "(0,3) (3,3)")]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 0 1\n" +
              "0 0 0 1 2\n" +
              "0 0 0 2 2\n" +
              "0 1 2 2 1", 2, DiscColor.Red, "(4,1) (1,4)")]
    [TestCase("0 0 0 2 0\n" +
              "0 0 2 1 0\n" +
              "0 2 2 1 0\n" +
              "0 1 1 2 0\n" +
              "1 1 2 2 0", 0, DiscColor.Yellow, "(3,0) (0,3)")]
    [TestCase("1 0 0 0 0\n" +
              "2 0 0 0 0\n" +
              "2 1 1 2 0\n" +
              "1 2 2 1 0\n" +
              "1 1 2 2 2", 1, DiscColor.Red, "(0,0) (3,3)")]
    [TestCase("0 0 0 0 0\n" +
              "2 1 0 0 0\n" +
              "2 1 1 2 0\n" +
              "1 2 2 1 0\n" +
              "1 1 2 2 0", 4, DiscColor.Red, "(1,1) (4,4)")]
    public void SlideInDisc_CreatesAConnection_ShouldDetectTheWinningConnection(string cellConfiguration, int targetColumn, DiscColor color, string connectionCoordinates)
    {
        ShouldImplementIGrid();
        GameSettings settings = new GameSettingsBuilder().WithGridDimensions(5, 5).Build();
        IGrid grid = new GridBuilder(settings).WithCellConfiguration(cellConfiguration).Build();

        Assert.That(grid.WinningConnections, Has.Count.Zero,
            "The grid should not have any winning connection before the winning disc is inserted");

        IList<GridCoordinate> expectedConnectionCoordinates = ParseCoordinateText(connectionCoordinates);

        IDisc? disc = new Disc(DiscType.Normal, color) as IDisc;
        Assert.That(disc, Is.Not.Null, "Make sure the tests on the Disc class are green first.");

        grid.SlideInDisc(disc, targetColumn);

        string CreateErrorMessage()
        {
            string message = $"For the following start situation (0=empty,1=red,2=yellow):\n{cellConfiguration}\n " +
                             $"When a {color} disc is inserted in column with index {targetColumn} \n" +
                             $"then a winning connection of size 4 from ({expectedConnectionCoordinates[0].Row},{expectedConnectionCoordinates[0].Column}) " +
                             $"to ({expectedConnectionCoordinates[1].Row},{expectedConnectionCoordinates[1].Column}) should be detected.";
            return message;
        }

        Assert.That(grid.WinningConnections, Has.Count.EqualTo(1), CreateErrorMessage);
        var winningConnection = grid.WinningConnections.First();

        Assert.That(winningConnection.Size, Is.EqualTo(4), CreateErrorMessage);
        Assert.That(winningConnection.From, Is.EqualTo(expectedConnectionCoordinates.First()), CreateErrorMessage);
        Assert.That(winningConnection.To, Is.EqualTo(expectedConnectionCoordinates.ElementAt(1)), CreateErrorMessage);
        Assert.That(winningConnection.Color, Is.EqualTo(color), CreateErrorMessage);
    }

    [MonitoredTest("SlideInDisc - Does not create a connection - Should not detect a winning connection")]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "2 1 1 0 2", 3, DiscColor.Red)]
    [TestCase("0 0 0 0 0\n" +
              "0 1 0 0 0\n" +
              "0 2 0 0 0\n" +
              "0 1 0 0 0\n" +
              "2 1 0 0 0", 1, DiscColor.Red)]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "0 0 2 1 0\n" +
              "0 2 1 2 0\n" +
              "1 1 2 2 0", 3, DiscColor.Yellow)]
    [TestCase("1 0 0 0 0\n" +
              "2 0 0 0 0\n" +
              "1 1 2 0 0\n" +
              "1 2 1 2 0\n" +
              "1 1 2 2 1", 1, DiscColor.Yellow)]
    [TestCase("0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "0 0 0 0 0\n" +
              "0 0 2 0 0\n" +
              "0 1 2 1 0", 2, DiscColor.Yellow)]
    public void SlideInDisc_DoesNotCreateAConnection_ShouldNotDetectAWinningConnection(string cellConfiguration, int targetColumn, DiscColor color)
    {
        ShouldImplementIGrid();
        GameSettings settings = new GameSettingsBuilder().WithGridDimensions(5, 5).Build();
        IGrid grid = new GridBuilder(settings).WithCellConfiguration(cellConfiguration).Build();

        Assert.That(grid.WinningConnections, Has.Count.Zero,
            "The grid should not have any winning connection before the the disc is inserted");


        IDisc? disc = new Disc(DiscType.Normal, color) as IDisc;
        Assert.That(disc, Is.Not.Null, "Make sure the tests on the Disc class are green first.");

        grid.SlideInDisc(disc, targetColumn);

        string CreateErrorMessage()
        {
            string message = $"For the following start situation (0=empty,1=red,2=yellow):\n{cellConfiguration}\n " +
                             $"When a {color} disc is inserted in column with index {targetColumn} \n" +
                             "then no winning connection should be detected.";
            return message;
        }

        Assert.That(grid.WinningConnections, Has.Count.Zero, CreateErrorMessage);
    }

    private IList<GridCoordinate> ParseCoordinateText(string coordinateText)
    {
        IList<GridCoordinate> coordinates = new List<GridCoordinate>();
        string[] coordinatePairs = coordinateText.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        foreach (string coordinatePair in coordinatePairs)
        {
            coordinates.Add(new GridCoordinate(int.Parse(coordinatePair.Substring(1, 1)),
                int.Parse(coordinatePair.Substring(3, 1))));
        }
        return coordinates;
    }

    private void AssertIsReadOnlyProperty(PropertyInfo property)
    {
        Assert.That(!property.CanWrite || property.SetMethod is not null && property.SetMethod.IsPrivate, Is.True,
            $"The property '{property.Name}' should not have a (public) setter.");
    }
}