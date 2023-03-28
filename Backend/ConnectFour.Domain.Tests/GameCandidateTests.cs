using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.Tests.Builders;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConnectFour.Domain.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "GameCandidate", @"ConnectFour.Domain\GameDomain\GameCandidate.cs")]
public class GameCandidateTests
{
    private User _user;
    private GameSettings _gameSettings;
    private GameCandidate _candidate;
    private GameSettings _matchingGameSettings;

    [SetUp]
    public void Setup()
    {
        _user = new UserBuilder().Build();
        _gameSettings = new GameSettingsBuilder().WithAutoMatching(true).Build();
        _matchingGameSettings = new GameSettingsBuilder().AsCopyOf(_gameSettings).Build();
        _candidate = new GameCandidate(_user, _gameSettings);
    }

    [MonitoredTest("Constructor - Should initialize properly")]
    public void Constructor_ShouldInitializeProperly()
    {
        //Assert
        Assert.That(_candidate.User, Is.SameAs(_user), "The 'User' property is not set correctly.");
        Assert.That(_candidate.GameSettings, Is.SameAs(_gameSettings), "The 'GameSettings' property is not set correctly.");
        Assert.That(_candidate.GameId, Is.EqualTo(Guid.Empty), "The 'GameId' should be an empty Guid.");
        Assert.That(_candidate.ProposedOpponentUserId, Is.EqualTo(Guid.Empty), "The 'ProposedOpponentUserId' should be an empty Guid.");
    }

    [MonitoredTest("CanChallenge - Challenger already in game - Should return false")]
    public void CanChallenge_ChallengerAlreadyInGame_ShouldReturnFalse()
    {
        //Arrange
        IGameCandidate target = new GameCandidateMockBuilder().WithSettings(_matchingGameSettings).Object;
        _candidate.GameId = Guid.NewGuid();

        //Act
        bool result = _candidate.CanChallenge(target);

        //Assert
        Assert.That(result, Is.False, "A candidate cannot challenge when its 'GameId' has a non-empty value.");
    }

    [MonitoredTest("CanChallenge - Challenged candidate already in game - Should return false")]
    public void CanChallenge_ChallengedCandidateAlreadyInGame_ShouldReturnFalse()
    {
        //Arrange
        IGameCandidate target = new GameCandidateMockBuilder().WithSettings(_matchingGameSettings).WithGameId().Object;

        //Act
        bool result = _candidate.CanChallenge(target);

        //Assert
        Assert.That(result, Is.False, "A candidate cannot be challenged when its 'GameId' has a non-empty value.");
    }

    [MonitoredTest("CanChallenge - Challenge yourself - Should return false")]
    public void CanChallenge_ChallengeYourself_ShouldReturnFalse()
    {
        //Act
        bool result = _candidate.CanChallenge(_candidate);

        //Assert
        Assert.That(result, Is.False, "A candidate cannot challenge himself.");
    }

    [MonitoredTest("CanChallenge - Different grid dimensions - Should return false")]
    public void CanChallenge_DifferentGridDimensions_ShouldReturnFalse()
    {
        //Arrange
        GameSettings nonMatchingSettings =
            new GameSettingsBuilder().WithGridDimensions(_gameSettings.GridColumns + 1, _gameSettings.GridColumns + 1).Build();
        IGameCandidate target = new GameCandidateMockBuilder().WithSettings(nonMatchingSettings).Object;

        //Act
        bool result = _candidate.CanChallenge(target);

        //Assert
        Assert.That(result, Is.False,
            "A candidate cannot be challenged when the dimensions (rows, columns) of the settings do not match.");
    }

    [MonitoredTest("CanChallenge - Different connection size - Should return false")]
    public void CanChallenge_DifferentConnectionSize_ShouldReturnFalse()
    {
        //Arrange
        GameSettings nonMatchingSettings =
            new GameSettingsBuilder().WithConnectionSize(_gameSettings.ConnectionSize - 1).Build();
        IGameCandidate target = new GameCandidateMockBuilder().WithSettings(nonMatchingSettings).Object;

        //Act
        bool result = _candidate.CanChallenge(target);

        //Assert
        Assert.That(result, Is.False,
            "A candidate cannot be challenged when connection size of the settings does not match.");
    }

    [MonitoredTest("CanChallenge - Different pop out setting - Should return false")]
    public void CanChallenge_DifferentPopOutSetting_ShouldReturnFalse()
    {
        //Arrange
        GameSettings nonMatchingSettings =
            new GameSettingsBuilder().WithPopOut(!_gameSettings.EnablePopOut).Build();
        IGameCandidate target = new GameCandidateMockBuilder().WithSettings(nonMatchingSettings).Object;

        //Act
        bool result = _candidate.CanChallenge(target);

        //Assert
        Assert.That(result, Is.False,
            "A candidate cannot be challenged when EnablePopOut of the settings does not match.");
    }

    [MonitoredTest("CanChallenge - All validations pass - Should return true")]
    public void CanChallenge_AllValidationsPass_ShouldReturnTrue()
    {
        //Arrange
        IGameCandidate target = new GameCandidateMockBuilder().WithSettings(_matchingGameSettings).Object;

        //Act
        bool result = _candidate.CanChallenge(target);

        //Assert
        Assert.That(result, Is.True);
    }

    [MonitoredTest("Challenge - Should use CanChallenge method")]
    public void Challenge_ShouldUseCanChallengeMethod()
    {
        var code = Solution.Current.GetFileContent(@"ConnectFour.Domain\GameDomain\GameCandidate.cs");
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot();
        var method = root
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(md => md.Identifier.ValueText == "Challenge");

        Assert.That(method, Is.Not.Null, "Could not find the 'Challenge' method.");

        var body = CodeCleaner.StripComments(method.Body.ToString());

        Assert.That(body, Contains.Substring("CanChallenge("),
            "You must use the 'CanChallenge' method to check if the challenge can be made.");
    }

    [MonitoredTest("Challenge - Should set the proposed opponent")]
    public void Challenge_ShouldSetTheProposedOpponent()
    {
        //Arrange
        IGameCandidate target = new GameCandidateMockBuilder().WithSettings(_matchingGameSettings).Object;

        //Act
        _candidate.Challenge(target);

        //Assert
        Assert.That(_candidate.ProposedOpponentUserId, Is.EqualTo(target.User.Id),
            "The 'ProposedOpponentUserId' of the challenger should the user id of the challenged candidate.");
    }

    [MonitoredTest("Challenge - Invalid challenge - Should throw InvalidOperationException")]
    public void Challenge_InvalidChallenge_ShouldThrowInvalidOperationException()
    {
        //Arrange
        GameSettings nonMatchingSettings =
            new GameSettingsBuilder().WithGridDimensions(_gameSettings.GridRows - 1, _gameSettings.GridColumns - 1).Build();
        IGameCandidate target = new GameCandidateMockBuilder().WithSettings(nonMatchingSettings).WithGameId().Object;
        _candidate.GameId = Guid.NewGuid();

        //Act
        Assert.That(() => _candidate.Challenge(target), Throws.InvalidOperationException);
    }

    [MonitoredTest("AcceptChallenge - Already in game - Should throw InvalidOperationException")]
    public void AcceptChallenge_AlreadyInGame_ShouldThrowInvalidOperationException()
    {
        //Arrange
        IGameCandidate challenger = new GameCandidateMockBuilder()
            .WithSettings(_matchingGameSettings)
            .WithProposedOpponentUserId(_user.Id)
            .Object;
        _candidate.GameId = Guid.NewGuid();

        //Act + Assert
        Assert.That(() => _candidate.AcceptChallenge(challenger), Throws.InvalidOperationException);
        Assert.That(() => _candidate.AcceptChallenge(challenger),
            Throws.InvalidOperationException.With.Message.Contains("game").IgnoreCase,
            "The message of the exception should contain the word 'game'.");

        Assert.That(_candidate.ProposedOpponentUserId, Is.EqualTo(Guid.Empty),
            "The 'ProposedOpponentUserId' of the candidate should remain empty.");
    }

    [MonitoredTest("AcceptChallenge - Challenge was for other candidate - Should throw InvalidOperationException")]
    public void AcceptChallenge_ChallengeWasForOtherCandidate_ShouldThrowInvalidOperationException()
    {
        //Arrange
        Guid otherUserId = Guid.NewGuid();
        IGameCandidate challenger = new GameCandidateMockBuilder()
            .WithSettings(_matchingGameSettings)
            .WithProposedOpponentUserId(otherUserId)
            .Object;

        //Act + Assert
        Assert.That(() => _candidate.AcceptChallenge(challenger), Throws.InvalidOperationException);
        Assert.That(() => _candidate.AcceptChallenge(challenger),
            Throws.InvalidOperationException.With.Message.Contains("other candidate").IgnoreCase,
            "The message of the exception should contain the words 'other candidate'.");

        Assert.That(_candidate.ProposedOpponentUserId, Is.EqualTo(Guid.Empty),
            "The 'ProposedOpponentUserId' of the candidate should remain empty.");
    }

    [MonitoredTest("AcceptChallenge - Valid challenge - Should set proposed opponent")]
    public void AcceptChallenge_ValidChallenge_ShouldSetProposedOpponent()
    {
        //Arrange
        IGameCandidate challenger = new GameCandidateMockBuilder()
            .WithSettings(_matchingGameSettings)
            .WithProposedOpponentUserId(_user.Id)
            .Object;

        //Act
        _candidate.AcceptChallenge(challenger);

        //Assert
        Assert.That(_candidate.ProposedOpponentUserId, Is.EqualTo(challenger.User.Id),
            "The 'ProposedOpponentUserId' of the candidate should be the user id of the challenger.");
    }
}