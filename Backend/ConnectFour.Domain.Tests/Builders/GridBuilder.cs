using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;

namespace ConnectFour.Domain.Tests.Builders;

public class GridBuilder
{
    private readonly IGrid _grid;

    public GridBuilder(GameSettings settings)
    {
        _grid = new Grid(settings) as IGrid;
        Assert.That(_grid, Is.Not.Null, "Make sure the tests on the Grid class are green first.");
    }

    public GridBuilder WithCellConfiguration(string cellConfiguration, bool useSlideInMethod = false)
    {
        string[] rows = cellConfiguration.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        for (int i = rows.Length - 1; i >= 0; i--)
        {
            string[] values = rows[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            for (int j = 0; j < values.Length; j++)
            {
                IDisc? disc;
                switch (values[j])
                {
                    case "1":
                        disc = new Disc(DiscType.Normal, DiscColor.Red) as IDisc;
                        break;
                    case "2":
                        disc = new Disc(DiscType.Normal, DiscColor.Yellow) as IDisc;
                        break;
                    default:
                        disc = null;
                        break;
                }

                if (useSlideInMethod && disc is not null)
                {
                    _grid.SlideInDisc(disc, j);
                }
                else
                {
                    _grid.Cells[i, j] = disc;
                }
            }
        }

        return this;
    }

    public IGrid Build()
    {
        return _grid;
    }
}