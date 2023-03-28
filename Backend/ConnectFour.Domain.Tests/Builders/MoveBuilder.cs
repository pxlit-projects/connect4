using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GridDomain;

namespace ConnectFour.Domain.Tests.Builders;

public class MoveBuilder
{
    private static readonly Random RandomGenerator = new Random();

    private readonly Move _move;

    public MoveBuilder()
    {
        int column = RandomGenerator.Next(0, 7);
        MoveType type = MoveType.SlideIn;
        DiscType discType = DiscType.Normal;
        _move = new Move(column,type, discType);
    }

    public Move Build()
    {
        return _move;
    }
}