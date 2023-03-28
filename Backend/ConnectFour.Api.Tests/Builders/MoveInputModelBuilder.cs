using ConnectFour.Api.Models;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.Tests.Extensions;

namespace ConnectFour.Api.Tests.Builders;

public class MoveInputModelBuilder
{
    private static readonly Random RandomGenerator = new Random();

    private readonly MoveInputModel _model;

    public MoveInputModelBuilder()
    {
        _model = new MoveInputModel
        {
            Column = RandomGenerator.Next(0,7),
            Type = Enum.GetValues<MoveType>().NextRandomElement(),
            DiscType = Enum.GetValues<DiscType>().NextRandomElement()
        };
    }

    public MoveInputModel Build()
    {
        return _model;
    }
}