namespace ConnectFour.Domain.GridDomain.Contracts;

/// <summary>
/// Can give a score on a grid. The score indicates who is winning.
/// </summary>
/// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
public interface IGridEvaluator
{
    /// <summary>
    /// Calculates a score for a <see cref="IGrid"/>.
    /// The score indicates how likely is is that the <paramref name="maximizingColor"/> is going to win.
    /// If the <paramref name="maximizingColor"/> is winning the score will be positive.
    /// If the <paramref name="maximizingColor"/> is losing the score will be negative.
    /// </summary>
    /// <param name="grid">The grid (possibly) containing discs.</param>
    /// <param name="maximizingColor">
    /// The color (red or yellow) of the player that tries to get a score as high as possible.
    /// The minimizing player (the other player) is winning when the score is negative.
    /// </param>
    /// <returns>
    /// 0 if nobody has the upper hand.
    /// A positive score if the maximizing player has the upper hand. The higher the score, the more likely the maximizing player is going to win.
    /// A negative score if the minimizing player has the upper hand. The lower the score, the more likely the minimizing player is going to win.
    /// int.MinValue when the minimizing player has a connect4 (and the maximizing player does not).
    /// int.MaxValue when the maximizing player has a connect4 (and the minimizing player does not).
    /// </returns>
    int CalculateScore(IGrid grid, DiscColor maximizingColor);
}