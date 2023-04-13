using AutoMapper;
using ConnectFour.Api.Models;
using ConnectFour.AppLogic.Contracts;
using ConnectFour.Domain;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ApiControllerBase
{
    private readonly IGameService _gameService;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GamesController(IGameService gameService, UserManager<User> userManager, IMapper mapper)
    {
        _gameService = gameService;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets information about a game
    /// </summary>
    /// <param name="id">Id (guid) of the game</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetGame(Guid id)
    {
        IGame game = _gameService.GetById(id);
        if(game.Player1.Id != UserId && game.Player2.Id != UserId)
        {
            return BadRequest(new ErrorModel($"De gebruiker met id '{UserId}' is geen speler van het spel."));
        }
        return Ok(game);
    }

    /// <summary>
    /// Gets the possible moves of the logged in user for a certain game
    /// </summary>
    /// <param name="id">Id (guid) of the game</param>
    [HttpGet("{id}/possible-moves")]
    [ProducesResponseType(typeof(IReadOnlyList<IMove>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPossibleMovesForGame(Guid id)
    {
        IGame game = _gameService.GetById(id);
        if (game.Player1.Id != UserId && game.Player2.Id != UserId)
        {
            return BadRequest(new ErrorModel($"De gebruiker met id '{UserId}' is geen speler van het spel."));
        }
        return Ok(game.GetPossibleMovesFor(UserId));
    }

    /// <summary>
    /// Executes a move on the grid of the game.
    /// </summary>
    /// <param name="id">Id (guid) of the game</param>
    /// <param name="inputModel">Specification of the move (color, move type)</param>
    [HttpPost("{id}/move")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public IActionResult ExecuteMove(Guid id, [FromBody] MoveInputModel inputModel)
    {
        _gameService.ExecuteMove(id, UserId, _mapper.Map<IMove>(inputModel));
        return Ok();
    }

    /// <summary>
    /// Creates a single player game for the logged in user against a computer player (AI).
    /// </summary>
    /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
    [HttpPost("single-player")]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSinglePlayerGame([FromBody] GameSettings gameSettings)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        IGame game = _gameService.CreateSinglePlayerGameForUser(currentUser, gameSettings);
        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
    }
}