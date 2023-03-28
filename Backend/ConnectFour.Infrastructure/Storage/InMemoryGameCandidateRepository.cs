using ConnectFour.AppLogic.Contracts;
using ConnectFour.Common;
using ConnectFour.Domain.GameDomain.Contracts;

namespace ConnectFour.Infrastructure.Storage;

/// <inheritdoc cref="IGameCandidateRepository"/>
internal class InMemoryGameCandidateRepository : IGameCandidateRepository
{
    private readonly ExpiringDictionary<Guid, IGameCandidate> _candidateDictionary;

    public InMemoryGameCandidateRepository()
    {
        _candidateDictionary = new ExpiringDictionary<Guid, IGameCandidate>(TimeSpan.FromMinutes(10));
    }

    public void AddOrReplace(IGameCandidate candidate)
    {
        _candidateDictionary.AddOrReplace(candidate.User.Id, candidate);
    }

    public void RemoveCandidate(Guid userId)
    {
        _candidateDictionary.TryRemove(userId, out IGameCandidate _);
    }

    public IGameCandidate GetCandidate(Guid userId)
    {
        if (_candidateDictionary.TryGetValue(userId, out IGameCandidate candidate))
        {
            return candidate;
        }
        throw new DataNotFoundException();
    }

    public IList<IGameCandidate> FindCandidatesThatCanBeChallengedBy(Guid userId)
    {
        //TODO: retrieve the candidate with userId as key in the _candidateDictionary (use the TryGetValue method)

        //TODO: loop over all candidates (user the Values property of _candidateDictionary)
        //and check if those candidates can be challenged by the candidate you retrieved in the first step (use the CanChallenge method of IGameCandidate).
        //Put the candidates that can be challenged in a list and return that list.

        throw new NotImplementedException();
    }

    public IList<IGameCandidate> FindChallengesFor(Guid challengedUserId)
    {
        return _candidateDictionary.Values.Where(t => t.ProposedOpponentUserId == challengedUserId).ToList();
    }
}