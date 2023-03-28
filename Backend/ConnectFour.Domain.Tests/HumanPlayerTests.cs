using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.PlayerDomain;
using ConnectFour.Domain.PlayerDomain.Contracts;
using ConnectFour.Domain.Tests.Extensions;

namespace ConnectFour.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "HumanPlayer", @"ConnectFour.Domain\PlayerDomain\HumanPlayer.cs;ConnectFour.Domain\PlayerDomain\PlayerBase.cs")]
public class HumanPlayerTests
{
    private static readonly Random RandomGenerator = new Random();
    private IPlayer _player;
    private DiscColor _color;
    private int _numberOfNormalDiscs;
    private string _userName;
    private Guid _userId;


    [SetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _userName = Guid.NewGuid().ToString();
        _color = Enum.GetValues<DiscColor>().NextRandomElement();
        _numberOfNormalDiscs = RandomGenerator.Next(10, 22);
        _player = new HumanPlayer(_userId, _userName, _color, _numberOfNormalDiscs) as IPlayer;
    }

    [MonitoredTest("Should implement IPlayer")]
    public void _01_ShouldImplementIPlayer()
    {
        //Assert
        Assert.That(_player, Is.Not.Null, "HumanPlayer should implement IPlayer");
    }

    [MonitoredTest("Constructor - Should initialize properly")]
    public void _02_Constructor_ShouldInitializeProperly()
    {
        _01_ShouldImplementIPlayer();

        Assert.That(_player.Id, Is.EqualTo(_userId), "The 'Id' property is not set correctly.");
        Assert.That(_player.Name, Is.EqualTo(_userName), "The 'Name' property is not set correctly.");
        Assert.That(_player.Color, Is.EqualTo(_color), "The 'Color' property is not set correctly.");
        Assert.That(_player.NumberOfNormalDiscs, Is.EqualTo(_numberOfNormalDiscs), "The 'NumberOfNormalDiscs' property is not initialized correctly.");
    }

    [MonitoredTest("HasDisk - Normal disk - PlayerHasNormalDisks - Should return true")]
    public void _03_HasDisk_NormalDisk_PlayerHasNormalDisks_ShouldReturnTrue()
    {
        _01_ShouldImplementIPlayer();
        Assert.That(_player.HasDisk(DiscType.Normal), Is.True);
    }

    [MonitoredTest("HasDisk - Normal disk - PlayerHasNoNormalDisks - Should return false")]
    public void _04_HasDisk_NormalDisk_PlayerHasNoNormalDisks_ShouldReturnFalse()
    {
        _01_ShouldImplementIPlayer();
        _player = new HumanPlayer(_userId, _userName, _color, 0) as IPlayer;
        Assert.That(_player.HasDisk(DiscType.Normal), Is.False);
    }

    [MonitoredTest("RemoveDisk - Normal disk - Should decrement number of normal discs")]
    public void _05_RemoveDisk_NormalDisk_ShouldDecrementNumberOfNormalDiscs()
    {
        _01_ShouldImplementIPlayer();

        _player.RemoveDisc(DiscType.Normal);

        Assert.That(_player.NumberOfNormalDiscs, Is.EqualTo(_numberOfNormalDiscs - 1));
    }

    [MonitoredTest("RemoveDisk - Normal disk - PlayerHasNoNormalDiscs - Should throw InvalidOperationException")]
    public void _06_RemoveDisk_NormalDisk_PlayerHasNoNormalDiscs_ShouldThrowInvalidOperationException()
    {
        _01_ShouldImplementIPlayer();

        _player = new HumanPlayer(_userId, _userName, _color, 0) as IPlayer;

        Assert.That(() => _player.RemoveDisc(DiscType.Normal), Throws.InvalidOperationException);
    }
}