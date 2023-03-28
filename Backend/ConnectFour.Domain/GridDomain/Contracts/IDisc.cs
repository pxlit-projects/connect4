namespace ConnectFour.Domain.GridDomain.Contracts;

/// <summary>
/// Disc that can be slide in to an <see cref="IGrid"/>.
/// </summary>
public interface IDisc
{
    /// <summary>
    /// Type of the disc.
    /// Typically 'Normal' (1).
    /// Can also have another value when EXTRA'S are implemented.
    /// </summary>
    public DiscType Type { get; }

    /// <summary>
    /// Color of the disc.
    /// Red (1) or Yellow (2)
    /// </summary>
    public DiscColor Color { get; }
}