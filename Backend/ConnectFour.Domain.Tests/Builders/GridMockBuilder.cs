using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;

namespace ConnectFour.Domain.Tests.Builders;

public class GridMockBuilder : MockBuilder<IGrid>
{
    private static readonly Random RandomGenerator = new Random();
    private readonly IDisc?[,] _cells;
    public const int NumberOfRows = 6;
    public const int NumberOfColumns = 7;
    public const int WinningConnectSize = 4;

    public GridMockBuilder()
    {
        Mock.SetupGet(g => g.WinningConnections).Returns(new List<IConnection>());
        Mock.SetupGet(g => g.WinningConnectSize).Returns(WinningConnectSize);
        Mock.SetupGet(g => g.NumberOfRows).Returns(NumberOfRows);
        Mock.SetupGet(g => g.NumberOfColumns).Returns(NumberOfColumns);

        _cells = new IDisc?[NumberOfRows, NumberOfColumns];
        Mock.SetupGet(g => g.Cells).Returns(_cells);
    }

    public GridMockBuilder WithWinningConnection()
    {
        Mock.SetupGet(g => g.WinningConnections).Returns(new List<IConnection>
        {
            new ConnectionMockBuilder().Object
        });
        return this;
    }

    public void WithAllColumnsFilledExcept(int numberOfColumnsWithEmptySpace)
    {
        //Fill all rows except the top one
        DiscColor color = DiscColor.Red;
        for (int i = 1; i < NumberOfRows; i++)
        {
            for (int j = 0; j < NumberOfColumns; j++)
            {
                _cells[i, j] = new Disc(DiscType.Normal, color) as IDisc;
                color = color == DiscColor.Red ? DiscColor.Yellow : DiscColor.Red;
            }
        }

        int numberOfFilledColumns = 0;
        while (numberOfFilledColumns < NumberOfColumns - numberOfColumnsWithEmptySpace)
        {
            int column = RandomGenerator.Next(0, NumberOfColumns);
            if (_cells[0, column] is null)
            {
                _cells[0, column] = new Disc(DiscType.Normal, color) as IDisc;
                color = color == DiscColor.Red ? DiscColor.Yellow : DiscColor.Red;
                numberOfFilledColumns++;
            }
        }
    }
}