using ConnectFour.Domain.GridDomain.Contracts;

namespace ConnectFour.Domain.GridDomain;

/// <inheritdoc cref="IConnection"/>
public class Connection
{
    public static Connection Empty
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public Connection(int rowFrom, int columnFrom, int rowTo, int columnTo, DiscColor color)
    {

    }
}