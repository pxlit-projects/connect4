using System.Reflection;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;

namespace ConnectFour.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "Connection", @"ConnectFour.Domain\GridDomain\Connection.cs")]
public class ConnectionTests
{
    [MonitoredTest("Should implement IConnection")]
    public void ShouldImplementIConnection()
    {
        var connectionType = typeof(Connection);
        Assert.That(connectionType.IsAssignableTo(typeof(IConnection)), "The interface IConnection is not implemented");
    }

    [MonitoredTest("Should have a private parameterless constructor")]
    public void ShouldHaveAPrivateParameterlessConstructor()
    {
        ShouldImplementIConnection();

        ConstructorInfo? constructor = typeof(Connection).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(c => c.GetParameters().Length == 0);
        Assert.That(constructor, Is.Not.Null, "Cannot find a parameterless constructor");

        Assert.That(constructor!.IsPrivate, Is.True, "The parameterless constructor should be private so it cannot be uses in other classes");

        IConnection? connection = constructor.Invoke(null) as IConnection;
        Assert.That(connection!.Color, Is.EqualTo(DiscColor.Red), "The private constructor should set the color to Red");
        Assert.That(connection.Size, Is.EqualTo(0), "The private constructor should set the size to zero");
        Assert.That(connection.From, Is.EqualTo(GridCoordinate.Empty), "The private constructor should set 'from' to an empty coordinate");
        Assert.That(connection.To, Is.EqualTo(GridCoordinate.Empty), "The private constructor should set 'to' to an empty coordinate");
    }

    [MonitoredTest("Empty - Should return a Connection of size zero")]
    public void Empty_ShouldReturnAConnectionOfSizeZero()
    {
        ShouldImplementIConnection();

        IConnection? emptyConnection = Connection.Empty as IConnection;

        Assert.That(emptyConnection!.Size, Is.EqualTo(0), "The size should be zero");
        Assert.That(emptyConnection.From, Is.EqualTo(GridCoordinate.Empty), "The 'From' should be an empty coordinate");
        Assert.That(emptyConnection.To, Is.EqualTo(GridCoordinate.Empty), "The 'To' should be an empty coordinate");
    }


    [MonitoredTest("Constructor - Should initialize correctly")]
    [TestCase(0, 0, 0, 3, DiscColor.Yellow, 4)]
    [TestCase(1, 1, 1, 1, DiscColor.Red, 1)]
    [TestCase(0, 2, 0, 3, DiscColor.Yellow, 2)]
    [TestCase(1, 2, 4, 5, DiscColor.Red, 4)]
    [TestCase(5, 0, 3, 2, DiscColor.Red, 3)]
    public void Constructor_ShouldInitializeCorrectly(int rowFrom, int columnFrom, int rowTo, int columnTo, DiscColor color, int expectedSize)
    {
        ShouldImplementIConnection();

        IConnection? connection = new Connection(rowFrom, columnFrom, rowTo, columnTo, color) as IConnection;

        string messagePrefix = $"Error creating a '{color}' connection from ({rowFrom},{columnFrom}) to ({rowTo}, {columnTo}).\n";

        Assert.That(connection!.From, Is.EqualTo(new GridCoordinate(rowFrom, columnFrom)), messagePrefix + "'From' is not initialized correctly");
        Assert.That(connection.To, Is.EqualTo(new GridCoordinate(rowTo, columnTo)), messagePrefix + "'To' is not initialized correctly");
        Assert.That(connection.Color, Is.EqualTo(color), messagePrefix + "'Color' is not initialized correctly");
        Assert.That(connection.Size, Is.EqualTo(expectedSize), messagePrefix + "'Size' is not calculated correctly");
    }

    [MonitoredTest("The properties From, To, Size and Color should only have a getter")]
    public void Properties_From_To_Size_Color_ShouldOnlyHaveAGetter()
    {
        ShouldImplementIConnection();
        var connectionType = typeof(Connection);

        AssertIsReadOnlyProperty(connectionType.GetProperty(nameof(IConnection.From))!);
        AssertIsReadOnlyProperty(connectionType.GetProperty(nameof(IConnection.To))!);
        AssertIsReadOnlyProperty(connectionType.GetProperty(nameof(IConnection.Size))!);
        AssertIsReadOnlyProperty(connectionType.GetProperty(nameof(IConnection.Color))!);
    }

    private void AssertIsReadOnlyProperty(PropertyInfo property)
    {
        Assert.That(!property.CanWrite || property.SetMethod is not null && property.SetMethod.IsPrivate, Is.True,
            $"The property '{property.Name}' should not have a (public) setter.");
    }
}