using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;
using ConnectFour.Domain.PlayerDomain.Contracts;
using ConnectFour.Domain.Tests.Extensions;

namespace ConnectFour.Domain.Tests.Builders;

public class PlayerMockBuilder : MockBuilder<IPlayer>
{
    private static readonly Random RandomGenerator = new Random();

    public PlayerMockBuilder()
    {
        Mock.SetupGet(p => p.Id).Returns(Guid.NewGuid());
        Mock.SetupGet(p => p.Color).Returns(Enum.GetValues<DiscColor>().NextRandomElement());
        Mock.SetupGet(p => p.NumberOfNormalDiscs).Returns(21);
        Mock.SetupGet(p => p.SpecialDiscs).Returns(new List<IDisc>());
        Mock.SetupGet(p => p.Name).Returns(Guid.NewGuid().ToString());
        Mock.Setup(p => p.HasDisk(It.IsAny<DiscType>())).Returns(true);
    }

    public PlayerMockBuilder WithId(Guid id)
    {
        Mock.SetupGet(p => p.Id).Returns(id);
        return this;
    }

    public PlayerMockBuilder WithColor(DiscColor color)
    {
        Mock.SetupGet(p => p.Color).Returns(color);
        return this;
    }

    public PlayerMockBuilder WithoutDiscs()
    {
        Mock.Setup(p => p.HasDisk(It.IsAny<DiscType>())).Returns(false);
        return this;
    }
}