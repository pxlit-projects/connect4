using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.PlayerDomain.Contracts;

namespace ConnectFour.Domain.Tests.Builders;

public class GameMockBuilder : MockBuilder<IGame>
{
    public PlayerMockBuilder RedPlayerMockBuilder { get; }
    public PlayerMockBuilder YellowPlayerMockBuilder { get; }

    public GameMockBuilder()
    {
        Mock.SetupGet(g => g.Id).Returns(Guid.NewGuid());

        YellowPlayerMockBuilder = new PlayerMockBuilder().WithColor(DiscColor.Yellow);
        RedPlayerMockBuilder = new PlayerMockBuilder().WithColor(DiscColor.Red);

        IPlayer yellowPlayer = YellowPlayerMockBuilder.Mock.Object;
        IPlayer redPlayer = RedPlayerMockBuilder.Mock.Object;
           
        WithPlayers(yellowPlayer, redPlayer);

        Mock.Setup(g => g.GetPossibleMovesFor(It.IsAny<Guid>())).Returns(new List<IMove>());
    }

    public GameMockBuilder WithPlayers(IPlayer player1, IPlayer player2)
    {
        Mock.SetupGet(g => g.Player1).Returns(player1);
        Mock.SetupGet(g => g.Player2).Returns(player2);
        Mock.Setup(g => g.GetPlayerById(player1.Id)).Returns(player1);
        Mock.Setup(g => g.GetPlayerById(player2.Id)).Returns(player2);
        Mock.Setup(g => g.GetOpponent(player1.Id)).Returns(player2);
        Mock.Setup(g => g.GetOpponent(player2.Id)).Returns(player1);
        return this;
    }

    public GameMockBuilder WithPossibleMoves(IReadOnlyList<IMove> moves)
    {
        Mock.Setup(g => g.GetPossibleMovesFor(It.IsAny<Guid>())).Returns(moves);
        return this;
    }
}