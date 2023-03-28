using System.Reflection;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;

namespace ConnectFour.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "Disc", @"ConnectFour.Domain\GridDomain\Disc.cs")]
public class DiscTests
{
    [MonitoredTest("Should implement IDisc")]
    public void ShouldImplementIDisc()
    {
        var discType = typeof(Disc);
        Assert.That(discType.IsAssignableTo(typeof(IDisc)), "The interface IDisk is not implemented");
    }

    [MonitoredTest("Constructor - Should initialize correctly")]
    [TestCase(DiscType.Normal, DiscColor.Yellow)]
    [TestCase(DiscType.Normal, DiscColor.Red)]
    public void Constructor_ShouldInitializeCorrectly(DiscType type, DiscColor color)
    {
        ShouldImplementIDisc();

        IDisc? disc = new Disc(type, color) as IDisc;
        Assert.That(disc!.Type, Is.EqualTo(type), "'Type' is not initialized correctly");
        Assert.That(disc.Color, Is.EqualTo(color), "'Color' is not initialized correctly");
    }

    [MonitoredTest("The properties Type and Color should only have a getter")]
    public void Properties_Type_Color_ShouldOnlyHaveAGetter()
    {
        ShouldImplementIDisc();
        var discType = typeof(Disc);
        AssertIsReadOnlyProperty(discType.GetProperty(nameof(IDisc.Type))!);
        AssertIsReadOnlyProperty(discType.GetProperty(nameof(IDisc.Color))!);
    }

    private void AssertIsReadOnlyProperty(PropertyInfo property)
    {
        Assert.That(!property.CanWrite || property.SetMethod is not null && property.SetMethod.IsPrivate, Is.True,
            $"The property '{property.Name}' should not have a (public) setter.");
    }
}