using System.Security.Claims;
using AutoMapper;
using ConnectFour.Api.Models;
using ConnectFour.Api.Tests.Builders;
using ConnectFour.AppLogic.Contracts;
using ConnectFour.Domain;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.Tests.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConnectFour.Api.Tests;

[ProjectComponentTestFixture("1TINProject", "Connect4", "GamesCtlr", @"ConnectFour.Api\Controllers\GamesController.cs")]
public class GamesControllerTests
{
    private GamesController _controller = null!;
    private Mock<IGameService> _gameServiceMock = null!;
    private User _loggedInUser = null!;
    private Mock<IMapper> _mapperMock = null!;
    private Mock<UserManager<User>> _userManagerMock = null!;

    [SetUp]
    public void Setup()
    {
        _gameServiceMock = new Mock<IGameService>();
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

        _controller = new GamesController(_gameServiceMock.Object, _userManagerMock.Object, _mapperMock.Object);

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
    }

    [MonitoredTest("GetGame - Logged in user is one of the players of the game - Should return OK")]
    public void GetGame_UserIsOneOfThePlayersOfTheGame_ShouldReturnOk()
    {
        //Arrange
        var gameMockBuilder = new GameMockBuilder();
        gameMockBuilder.YellowPlayerMockBuilder.Mock.SetupGet(p => p.Id).Returns(_loggedInUser.Id);
        IGame existingGame = gameMockBuilder.Object;

        _gameServiceMock.Setup(service => service.GetById(It.IsAny<Guid>()))
            .Returns(existingGame);

        //Act
        var result = _controller.GetGame(existingGame.Id) as OkObjectResult;

        //Assert
        _gameServiceMock.Verify(service => service.GetById(existingGame.Id), Times.Once,
            "The 'GetById' method of the game service is not called correctly");
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
        Assert.That(result.Value, Is.SameAs(existingGame), "The result should contain the game returned by the game service");
    }

    [MonitoredTest("GetGame - Logged in user is not one of the players of the game - Should return bad request")]
    public void GetGame_UserIsNotOneOfThePlayersOfTheGame_ShouldReturnBadRequest()
    {
        //Arrange
        var gameMockBuilder = new GameMockBuilder();
        IGame existingGame = gameMockBuilder.Object;

        _gameServiceMock.Setup(service => service.GetById(It.IsAny<Guid>()))
            .Returns(existingGame);

        //Act
        var result = _controller.GetGame(existingGame.Id) as BadRequestObjectResult;

        //Assert
        _gameServiceMock.Verify(service => service.GetById(existingGame.Id), Times.Once,
            "The 'GetById' method of the game service is not called correctly");
        Assert.That(result, Is.Not.Null, "An instance of 'BadRequestObjectResult' should be returned.");
        ErrorModel? error = result.Value as ErrorModel;
        Assert.That(error, Is.Not.Null, "An ErrorModel object should be the value of the bad request");
    }

    [MonitoredTest("GetPossibleMovesForGame - Should use the game service and return Ok")]
    public void GetPossibleMovesForGame_ShouldUseTheGameServiceAndReturnOk()
    {
        //Arrange
        var moves = new List<IMove>();
        var gameMockBuilder = new GameMockBuilder().WithPossibleMoves(moves);
        gameMockBuilder.RedPlayerMockBuilder.WithId(_loggedInUser.Id);
        Mock<IGame> gameMock = gameMockBuilder.Mock;
        IGame game = gameMockBuilder.Object;
        _gameServiceMock.Setup(service => service.GetById(It.IsAny<Guid>()))
            .Returns(game);

        //Act
        var result = _controller.GetPossibleMovesForGame(game.Id) as OkObjectResult;

        //Assert
        _gameServiceMock.Verify(service => service.GetById(game.Id), Times.Once,
            "The 'GetById' method of the game service is not called correctly");
        gameMock.Verify(game => game.GetPossibleMovesFor(_loggedInUser.Id),
            "The 'GetPossibleMovesFor' method of the game is not called correctly");
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
        Assert.That(result.Value, Is.SameAs(moves), "The exact same list of moves returned by the game should be in the result.");
    }

    [MonitoredTest("GetPossibleMovesForGame - User is no player of the game - Should return bad request")]
    public void GetPossibleMovesForGame_UserIsNoPlayerOfTheGame_ShouldReturnBadRequest()
    {
        //Arrange
        var gameMockBuilder = new GameMockBuilder();
        IGame game = gameMockBuilder.Object;
        _gameServiceMock.Setup(service => service.GetById(It.IsAny<Guid>()))
            .Returns(game);

        //Act
        var result = _controller.GetPossibleMovesForGame(game.Id) as BadRequestObjectResult;

        //Assert
        _gameServiceMock.Verify(service => service.GetById(game.Id), Times.Once,
            "The 'GetById' method of the game service is not called correctly");

        Assert.That(result, Is.Not.Null, "An instance of 'BadRequestObjectResult' should be returned.");
        ErrorModel? error = result.Value as ErrorModel;
        Assert.That(error, Is.Not.Null, "An ErrorModel object should be the value of the bad request");
    }

    [MonitoredTest("ExecuteMove - Should use the game service and return Ok")]
    public void ExecuteMove_ShouldUseTheGameServiceAndReturnOk()
    {
        //Arrange
        MoveInputModel inputModel = new MoveInputModelBuilder().Build();
        Guid gameId = Guid.NewGuid();

        IMove? mappedMove = new Move(inputModel.Column, MoveType.SlideIn, DiscType.Normal) as IMove;
        Assert.That(mappedMove, Is.Not.Null, "Make sure the tests on the Move class are green first.");
        _mapperMock.Setup(mapper => mapper.Map<IMove>(inputModel)).Returns(mappedMove);

        //Act
        var result = _controller.ExecuteMove(gameId, inputModel) as OkResult;

        //Assert
        _gameServiceMock.Verify(service => service.ExecuteMove(gameId, _loggedInUser.Id, mappedMove), Times.Once,
            "The 'ExecuteMove' method of the game service is not called correctly");
        Assert.That(result, Is.Not.Null, "An instance of 'OkResult' should be returned.");
    }
}