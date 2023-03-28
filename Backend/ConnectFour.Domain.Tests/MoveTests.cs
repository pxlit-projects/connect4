using System.Reflection;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain;

namespace ConnectFour.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "Move", @"ConnectFour.Domain\GameDomain\Move.cs")]
public class MoveTests
{
    [MonitoredTest("Should implement IMove")]
    public void ShouldImplementIMove()
    {
        var moveType = typeof(Move);
        Assert.That(moveType.IsAssignableTo(typeof(IMove)), "The interface IMove is not implemented");
    }


    [MonitoredTest("Constructor - Should initialize correctly")]
    [TestCase(0, MoveType.SlideIn, DiscType.Normal)]
    [TestCase(7, MoveType.PopOut, DiscType.Normal)]
    public void Constructor_ShouldInitializeCorrectly(int column, MoveType type, DiscType discType)
    {
        ShouldImplementIMove();

        IMove move = new Move(column, type, discType) as IMove;
        Assert.That(move!.Column, Is.EqualTo(column), "'Column' is not initialized correctly");
        Assert.That(move.Type, Is.EqualTo(type), "'Type' is not initialized correctly");
        Assert.That(move.DiscType, Is.EqualTo(discType), "'DiscType' is not initialized correctly");
    }

    [MonitoredTest("Constructor - Should have default values for Type and DiscType")]
    public void Constructor_ShouldHaveDefaultValuesForTypeAndDiscType()
    {
        var moveType = typeof(Move);

        ConstructorInfo? constructor = moveType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(c => c.GetParameters().Length == 3);
        Assert.That(constructor, Is.Not.Null, "Cannot find a public constructor that has 3 parameters");

        ParameterInfo? moveTypeParameter = constructor!.GetParameters().FirstOrDefault(p => p.ParameterType == typeof(MoveType));
        Assert.That(moveTypeParameter, Is.Not.Null, "Cannot find a parameter of type 'MoveType'");
        Assert.That(moveTypeParameter!.IsOptional && moveTypeParameter.HasDefaultValue, Is.True,
            "The 'MoveType' parameter should be optional and have a default value");
        Assert.That(moveTypeParameter.DefaultValue, Is.EqualTo(MoveType.SlideIn),
            "The default value of the 'MoveType' parameter should be 'SlideIn'");

        ParameterInfo? discTypeParameter = constructor.GetParameters().FirstOrDefault(p => p.ParameterType == typeof(DiscType));
        Assert.That(discTypeParameter, Is.Not.Null, "Cannot find a parameter of type 'DiscType'");
        Assert.That(discTypeParameter!.IsOptional && discTypeParameter.HasDefaultValue, Is.True,
            "The 'DiscType' parameter should be optional and have a default value");
        Assert.That(discTypeParameter.DefaultValue, Is.EqualTo(DiscType.Normal),
            "The default value of the 'DiscType' parameter should be 'Normal'");
    }

    [MonitoredTest("The properties Type, DiscType and Column should only have a getter")]
    public void Properties_Type_DiscType_Column_ShouldOnlyHaveAGetter()
    {
        ShouldImplementIMove();
        var moveType = typeof(Move);
        AssertIsReadOnlyProperty(moveType.GetProperty(nameof(IMove.Type))!);
        AssertIsReadOnlyProperty(moveType.GetProperty(nameof(IMove.DiscType))!);
        AssertIsReadOnlyProperty(moveType.GetProperty(nameof(IMove.Column))!);
    }

    private void AssertIsReadOnlyProperty(PropertyInfo property)
    {
        Assert.That(!property.CanWrite || property.SetMethod is not null && property.SetMethod.IsPrivate, Is.True,
            $"The property '{property.Name}' should not have a (public) setter.");
    }
}