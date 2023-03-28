using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.Tests.Builders;
using ConnectFour.Infrastructure.Storage;
using Guts.Client.Core;
using Moq;

namespace ConnectFour.Infrastructure.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "GameCandidateRepo", @"ConnectFour.Infrastructure\Storage\InMemoryGameCandidateRepository.cs")]
public class InMemoryGameCandidateRepositoryTests
{
    private static readonly Random RandomGenerator = new Random();

    private InMemoryGameCandidateRepository _repository = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new InMemoryGameCandidateRepository();
    }

    [MonitoredTest("FindCandidatesThatCanBeChallengedBy - Repo contains candidates that can be challenged - Should return them")]
    public void FindCandidatesThatCanBeChallengedBy_RepoContainsCandidatesThatCanBeChallenged_ShouldReturnThem()
    {
        //Arrange
        var challengerBuilder = new GameCandidateMockBuilder();
        Mock<IGameCandidate> challengerMock = challengerBuilder.Mock;
        challengerMock.Setup(c => c.CanChallenge(It.IsAny<IGameCandidate>())).Returns(false);
        IGameCandidate challenger = challengerMock.Object;
        _repository.AddOrReplace(challenger);

        var unChallengeables = new List<IGameCandidate>();
        for (int i = 0; i < RandomGenerator.Next(1, 11); i++)
        {
            var candidate = new GameCandidateMockBuilder().Mock.Object;
            unChallengeables.Add(candidate);
            _repository.AddOrReplace(candidate);
        }
        var challengeables = new List<IGameCandidate>();
        for (int i = 0; i < RandomGenerator.Next(1, 11); i++)
        {
            var candidate = new GameCandidateMockBuilder().Mock.Object;
            challengerMock.Setup(c => c.CanChallenge(candidate)).Returns(true);
            challengeables.Add(candidate);
            _repository.AddOrReplace(candidate);
        }

        //Act
        IList<IGameCandidate> result = _repository.FindCandidatesThatCanBeChallengedBy(challenger.User.Id);

        //Assert
        Assert.That(result, Is.Not.Null, "No list was returned.");
        challengerMock.Verify(c => c.CanChallenge(It.IsAny<IGameCandidate>()),
            Times.AtLeast(challengeables.Count + unChallengeables.Count),
            "The 'CanChallenge' method of the challenger should have been called multiple times.");

        Assert.That(result.Count, Is.EqualTo(challengeables.Count),
            "The amount of returned candidates is not correct " +
            $"({challengeables.Count} out of {challengeables.Count + unChallengeables.Count + 1} could be challenged in this test run).");

        foreach (IGameCandidate candidate in result)
        {
            Assert.That(challengeables.Contains(candidate), "At least one candidate that could be challenged is not returned.");
        }
    }

    [MonitoredTest("FindCandidatesThatCanBeChallengedBy - Repo contains no candidates that can be challenged - Should return empty list")]
    public void FindCandidatesThatCanBeChallengedBy_RepoContainsNoCandidatesThatCanBeChallenged_ShouldReturnEmptyList()
    {
        //Arrange
        var challengerBuilder = new GameCandidateMockBuilder();
        Mock<IGameCandidate> challengerMock = challengerBuilder.Mock;
        challengerMock.Setup(c => c.CanChallenge(It.IsAny<IGameCandidate>())).Returns(false);
        IGameCandidate challenger = challengerMock.Object;

        _repository.AddOrReplace(challenger);

        var unChallengeables = new List<IGameCandidate>();
        for (int i = 0; i < RandomGenerator.Next(1, 11); i++)
        {
            IGameCandidate candidate = new GameCandidateMockBuilder().Mock.Object;
            unChallengeables.Add(candidate);
            _repository.AddOrReplace(candidate);
        }
            
        //Act
        IList<IGameCandidate> result = _repository.FindCandidatesThatCanBeChallengedBy(challenger.User.Id);

        //Assert
        Assert.That(result, Is.Not.Null, "No list was returned.");
        Assert.That(result, Is.Empty, "The list returned is not empty.");
    }
}