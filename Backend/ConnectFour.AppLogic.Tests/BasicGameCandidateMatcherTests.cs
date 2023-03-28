using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.Tests.Builders;

namespace ConnectFour.AppLogic.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "GameCandidateMatcher", @"ConnectFour.AppLogic\BasicGameCandidateMatcher.cs")]
public class BasicGameCandidateMatcherTests
{
    private static readonly Random RandomGenerator = new Random();

    private BasicGameCandidateMatcher _matcher;

    [SetUp]
    public void Setup()
    {
        _matcher = new BasicGameCandidateMatcher();
    }

    [MonitoredTest("SelectOpponentToChallenge - One or more candidates - Should return the first")]
    public void SelectOpponentToChallenge_OneOrMoreCandidates_ShouldReturnTheFirst()
    {
        //Arrange
        IList<IGameCandidate> candidates = new List<IGameCandidate>();
        for (int i = 0; i < RandomGenerator.Next(2, 10); i++)
        {
            candidates.Add(new GameCandidateMockBuilder().Mock.Object);
        }
        IGameCandidate first = candidates[0];
            
        //Act
        IGameCandidate selectedCandidate = _matcher.SelectOpponentToChallenge(candidates);

        //Assert
        Assert.That(selectedCandidate, Is.SameAs(first));
    }

    [MonitoredTest("SelectOpponentToChallenge - No candidates - Should return null")]
    public void SelectOpponentToChallenge_NoCandidates_ShouldReturnNull()
    {
        //Arrange
        IList<IGameCandidate> candidates = new List<IGameCandidate>();
            
        //Act
        IGameCandidate selectedCandidate = _matcher.SelectOpponentToChallenge(candidates);

        //Assert
        Assert.That(selectedCandidate, Is.Null);
    }
}