namespace ConnectFour.Domain.GameDomain.Contracts;

/// <summary>
/// User that wants to create or join a game against another (human) user.
/// </summary>
public interface IGameCandidate
{
    /// <summary>
    /// The user
    /// </summary>
    User User { get; }

    /// <summary>
    /// The game settings the <see cref="User"/> desires.
    /// </summary>
    GameSettings GameSettings { get; }

    /// <summary>
    /// The game that was found / created for the <see cref="User"/> and the <see cref="ProposedOpponentUserId">opponent</see>.
    /// When no game is found or created yet, an empty Guid is returned.
    /// </summary>
    Guid GameId { get; set; }

    /// <summary>
    /// UserId of the opponent that has been challenged by the <see cref="User"/>.
    /// If nobody is challenged (yet), an empty Guid is returned.
    /// </summary>
    Guid ProposedOpponentUserId { get; }

    /// <summary>
    /// Indicates if another candidate can be challenged.
    /// The <see cref="GameSettings"/> must match and a <see cref="GameId">game</see> is not found yet.
    /// </summary>
    bool CanChallenge(IGameCandidate targetCandidate);

    /// <summary>
    /// Actually challenge another candidate.
    /// This will set the <see cref="ProposedOpponentUserId"/>.
    /// </summary>
    void Challenge(IGameCandidate targetCandidate);

    /// <summary>
    /// Accept a challenge from another candidate.
    /// This will set the <see cref="ProposedOpponentUserId"/>.
    /// </summary>
    void AcceptChallenge(IGameCandidate challenger);

    /// <summary>
    /// Clears the <see cref="ProposedOpponentUserId"/>.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// </remarks>
    void WithdrawChallenge();
}