using System.ComponentModel;

namespace ConnectFour.Domain.GameDomain;

public class GameSettings
{
    /// <summary>
    /// If true, you will be automatically matched with another candidate in the waiting pool.
    /// </summary>
    [DefaultValue(true)]
    public bool AutoMatchCandidates { get; set; }

    /// <summary>
    /// If true, it is also possible to pop a disc out of the bottom of the grid (EXTRA).
    /// The default value is false.
    /// </summary>
    [DefaultValue(false)]
    public bool EnablePopOut { get; set; }


    /// <summary>
    /// Number of discs that need to be connected to win.
    /// The default value is four.
    /// </summary>
    [DefaultValue(4)]
    public int ConnectionSize { get; set; }

    /// <summary>
    /// Number of rows in the grid.
    /// The default value is 6.
    /// </summary>
    [DefaultValue(6)]
    public int GridRows { get; set; }

    /// <summary>
    /// Number of columns in the grid.
    /// The default value is 7.
    /// </summary>
    [DefaultValue(7)]
    public int GridColumns { get; set; }

    public GameSettings()
    {
        AutoMatchCandidates = true;
        EnablePopOut = false;
        ConnectionSize = 4;
        GridRows = 6;
        GridColumns = 7;
    }
}