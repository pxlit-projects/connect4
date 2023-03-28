using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;
using ConnectFour.Domain.Tests.Extensions;

namespace ConnectFour.Domain.Tests.Builders;

public class ConnectionMockBuilder : MockBuilder<IConnection>
{
    public ConnectionMockBuilder()
    {
        Mock.SetupGet(c => c.Size).Returns(4);
        Mock.SetupGet(c => c.Color).Returns(Enum.GetValues<DiscColor>().NextRandomElement());
        Mock.SetupGet(c => c.From).Returns(new GridCoordinate(0, 0));
        Mock.SetupGet(c => c.To).Returns(new GridCoordinate(3, 3));
    }
}