namespace ConnectFour.Domain.GameDomain.Contracts;

/// <summary>
/// Can create a <see cref="IGameCandidate"/>
/// </summary>
public interface IGameCandidateFactory
{
    /// <summary>
    /// Creates a new <see cref="IGameCandidate"/> from a <paramref name="user"/> and <paramref name="settings"/>.
    /// </summary>
    IGameCandidate CreateNewForUser(User user, GameSettings settings);
}