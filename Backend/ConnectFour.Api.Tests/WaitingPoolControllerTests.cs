using System.Security.Claims;
using AutoMapper;
using ConnectFour.Api.Models;
using ConnectFour.AppLogic.Contracts;
using ConnectFour.Domain;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.Tests.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConnectFour.Api.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "WaitingPoolCtlr", @"ConnectFour.Api\Controllers\WaitingPoolController.cs")]
public class WaitingPoolControllerTests
{
    private WaitingPoolController _controller;
    private Mock<IWaitingPool> _waitingPoolMock;
    private Mock<UserManager<User>> _userManagerMock;
    private Mock<IMapper> _mapperMock;
    private User _loggedInUser;
    private GameSettings _gameSettings;

    [SetUp]
    public void Setup()
    {
        _waitingPoolMock = new Mock<IWaitingPool>();
        _mapperMock = new Mock<IMapper>();

        var userStoreMock = new Mock<IUserStore<User>>();
        var passwordHasherMock = new Mock<IPasswordHasher<User>>();
        var lookupNormalizerMock = new Mock<ILookupNormalizer>();
        var errorsMock = new Mock<IdentityErrorDescriber>();
        var loggerMock = new Mock<ILogger<UserManager<User>>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object,
            null,
            passwordHasherMock.Object,
            null,
            null,
            lookupNormalizerMock.Object,
            errorsMock.Object,
            null,
            loggerMock.Object);

        _controller = new WaitingPoolController(_waitingPoolMock.Object, _userManagerMock.Object, _mapperMock.Object);

        _loggedInUser = new UserBuilder().Build();
        var userClaimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _loggedInUser.Id.ToString())
            })
        );
        var context = new ControllerContext { HttpContext = new DefaultHttpContext() };
        context.HttpContext.User = userClaimsPrincipal;
        _controller.ControllerContext = context;
        _userManagerMock.Setup(manager => manager.GetUserAsync(userClaimsPrincipal))
            .ReturnsAsync(_loggedInUser);

        _gameSettings = new GameSettingsBuilder().Build();
    }

    [MonitoredTest("Join - Should add the current user to the waiting pool")]
    public void Join_ShouldAddTheCurrentUserToTheWaitingPool()
    {
        //Act
        var result = _controller.Join(_gameSettings).Result as OkResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkResult' should be returned.");

        _userManagerMock.Verify(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once,
            "The 'GetUserAsync' of the UserManager is not called");

        _waitingPoolMock.Verify(pool => pool.Join(_loggedInUser, _gameSettings), Times.Once,
            "The 'Join' method of the waiting pool is not called correctly");
    }

    [MonitoredTest("Leave - Should remove the current user from the waiting pool")]
    public void Leave_ShouldRemoveTheCurrentUserFromTheWaitingPool()
    {
        //Act
        var result = _controller.Leave() as OkResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkResult' should be returned.");
        _waitingPoolMock.Verify(pool => pool.Leave(_loggedInUser.Id), Times.Once,
            "The 'Leave' method of the waiting pool is not called correctly");
    }

    [MonitoredTest("GetOwnCandidate - Should retrieve the candidacy of the current user from the waiting pool")]
    public void GetOwnCandidate_ShouldRetrieveTheCandidacyOfTheCurrentUserFromTheWaitingPool()
    {
        //Arrange
        IGameCandidate candidate = new GameCandidateMockBuilder().Mock.Object;
        _waitingPoolMock.Setup(pool => pool.GetCandidate(It.IsAny<Guid>())).Returns(candidate);

        var candidateModel = new CandidateModel();
        _mapperMock.Setup(mapper => mapper.Map<CandidateModel>(It.IsAny<IGameCandidate>())).Returns(candidateModel);

        //Act
        var result = _controller.GetOwnCandidate() as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned");
        _waitingPoolMock.Verify(pool => pool.GetCandidate(_loggedInUser.Id), Times.Once,
            "The 'GetCandidate' method of the waiting pool is not called correctly");
        _mapperMock.Verify(mapper => mapper.Map<CandidateModel>(candidate), Times.Once, "The candidate returned from the pool is not mapped");
        Assert.That(result.Value, Is.SameAs(candidateModel), "The mapped candidate model is not in the OkObjectResult");
    }
}