namespace ConnectFour.Domain.Tests.Extensions;

public static class RandomExtensions
{
    public static bool NextBool(this Random random)
    {
        return random.Next(0, 2) == 1;
    }
}