using AutoMapper;
using ConnectFour.Api.Models;
using ConnectFour.AppLogic.Contracts;
using ConnectFour.Domain;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Api.Controllers;

/// <summary>
/// Provides functionality for users that want to find an opponent to play against.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class WaitingPoolController : ApiControllerBase
{
    private readonly IWaitingPool _waitingPool;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public WaitingPoolController(IWaitingPool waitingPool, UserManager<User> userManager, IMapper mapper)
    {
        _waitingPool = waitingPool;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Join the waiting pool to find an opponent.
    /// if the AutoMatchCandidates of the <paramref name="gameSettings"/> is true,
    /// you will be automatically matched with an opponent (as soon as one is available in the waiting pool)
    /// </summary>
    /// <param name="gameSettings">
    /// Contains info about the type of game you want to play. You can only match with others that join with the same settings.
    /// </param>
    /// <remarks>Candidates are automatically removed from the waiting pool after 10 minutes.</remarks>
    [HttpPost("join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Join([FromBody] GameSettings gameSettings)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        _waitingPool.Join(currentUser, gameSettings);
        return Ok();
    }

    /// <summary>
    /// Leave the waiting pool of game candidates.
    /// </summary>
    [HttpPost("leave")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public IActionResult Leave()
    {
        _waitingPool.Leave(UserId);

        return Ok();
    }

    /// <summary>
    /// Gets information about your candidacy in the waiting pool.
    /// Call this endpoint periodically (e.g. by using polling) to check if a game has been created for you (by inspecting the GameId of the returned info).
    /// </summary>
    [HttpGet("candidates/me")]
    [ProducesResponseType(typeof(CandidateModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOwnCandidate()
    {
        IGameCandidate candidate = _waitingPool.GetCandidate(UserId);
        CandidateModel model = _mapper.Map<CandidateModel>(candidate);
        return Ok(model);
    }

    /// <summary>
    /// Get a list of candidates in the waiting pool that have challenged you.
    /// </summary>
    /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
    [HttpGet("candidates/challenging-me")]
    [ProducesResponseType(typeof(IList<CandidateModel>), StatusCodes.Status200OK)]
    public IActionResult GetCandidatesChallengingMe()
    {
        IList<IGameCandidate> challengingCandidates = _waitingPool.FindChallengesFor(UserId);

        var models = challengingCandidates.Select(t => _mapper.Map<CandidateModel>(t)).ToList();
        return Ok(models);
    }

    /// <summary>
    /// Challenge another candidate in the waiting pool.
    /// </summary>
    /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
    [HttpPost("challenge")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public IActionResult Challenge([FromBody] ChallengeModel model)
    {
        _waitingPool.Challenge(UserId, model.TargetUserId);
        return Ok();
    }

    /// <summary>
    /// Withdraw your challenge so that you can challenge another candidate.
    /// </summary>
    /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
    [HttpPost("withdraw-challenge")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult WithDrawChallenge()
    {
        _waitingPool.WithdrawChallenge(UserId);
        return Ok();
    }

    /// <summary>
    /// Get a list of candidates in the waiting pool that can be challenged.
    /// Only candidates that with matching game settings and that are not in a game, can be challenged.
    /// </summary>
    /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
    [HttpGet("candidates/possible-targets")]
    [ProducesResponseType(typeof(IList<CandidateModel>), StatusCodes.Status200OK)]
    public IActionResult GetCandidatesThatCanBeChallenged()
    {
        IList<IGameCandidate> targetCandidates = _waitingPool.FindCandidatesThatCanBeChallengedBy(UserId);
        var models = targetCandidates.Select(t => _mapper.Map<CandidateModel>(t)).ToList();
        return Ok(models);
    }
}