using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;
using ConnectFour.Domain.PlayerDomain.Contracts;

namespace ConnectFour.Domain.PlayerDomain;

/// <summary>
/// <see cref="IGamePlayStrategy"/> that uses the mini-max algorithm (https://en.wikipedia.org/wiki/Minimax).
/// </summary>
/// <remarks>
/// This is an EXTRA. Not needed to implement the minimal requirements.
/// Also when implementing the (AI) extra, it is not needed to alter code in this class.
/// It should work as is.
/// </remarks>
public class MiniMaxGamePlayStrategy : IGamePlayStrategy
{
    private Guid _maximizingPlayerId;
    private Guid _minimizingPlayerId;
    private DiscColor _maximizingColor;

    private int _maximumDepth;
    private readonly IGridEvaluator _gridEvaluator;
    
    public MiniMaxGamePlayStrategy(IGridEvaluator gridEvaluator, int maximumDepth)
    {
        _maximumDepth = maximumDepth;
        _gridEvaluator = gridEvaluator;
    }

    public IMove GetBestMoveFor(Guid playerId, IGame game)
    {
        var scoreDictionary = new Dictionary<IMove, int>();

        _maximizingPlayerId = playerId;
        _maximizingColor = game.GetPlayerById(playerId).Color;
        _minimizingPlayerId = game.GetOpponent(_maximizingPlayerId).Id;

        //Get all possible moves
        IReadOnlyList<IMove> possibleMoves = game.GetPossibleMovesFor(playerId);

        //Link the lowest possible initial score to each move
        foreach (IMove move in possibleMoves)
        {
            scoreDictionary.Add(move, int.MinValue);
        }

        //Apply the mini-max algorithm for each possible move
        foreach (IMove move in possibleMoves)
        {
            IGame newGame = new Game(game) as IGame;
            newGame.ExecuteMove(playerId, move);
            scoreDictionary[move] = MiniMax(newGame, _minimizingPlayerId, _maximumDepth, int.MinValue, int.MaxValue);
        }

        //Get the moves with the highest score (multiple moves may have the same score)
        int bestScore = scoreDictionary.Max(kv => kv.Value);
        IMove[] bestMoves = scoreDictionary.Where(kv => kv.Value == bestScore).Select(kv => kv.Key).ToArray();

        if (bestMoves.Length == 0) return null;

        //Choose a move from the best moves. Favor the columns closest to the middle
        IMove chosenMove = bestMoves.MinBy(move => Math.Abs(move.Column - (game.Grid.NumberOfColumns - 1) / 2));
        return chosenMove;
    }

    private int MiniMax(IGame game, Guid playerId, int depth, int alpha, int beta)
    {
        if (depth == 0 || game.Grid.WinningConnections.Count > 0)
        {
            //Reached a leaf node in the recursion or the game has a winning connection -> do not recurse further. Return the score of the grid.
            return _gridEvaluator.CalculateScore(game.Grid, _maximizingColor);
        }

        //Get the moves the player can execute
        IReadOnlyList<IMove> possibleMoves = game.GetPossibleMovesFor(playerId);
        if (possibleMoves.Count == 0)
        {
            return 0; //no winning connections in grid and no possible moves -> draw
        }

        bool isMaximizingPlayer = playerId == _maximizingPlayerId;
        Guid opponentId = isMaximizingPlayer ? _minimizingPlayerId : _maximizingPlayerId;
        int bestScore = isMaximizingPlayer ? int.MinValue : int.MaxValue;
        int moveCount = 0;
        
        while (moveCount < possibleMoves.Count && alpha < beta)  //alpha < beta = Alpha-Beta pruning -> https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning
        {
            //create a copy of the game and apply a move on it
            IMove move = possibleMoves[moveCount];
            IGame newGame = new Game(game) as IGame;
            newGame.ExecuteMove(playerId, move);
            moveCount++;

            //Recurse (go one level deeper) - Apply MiniMax from opponent perspective
            int score = MiniMax(newGame, opponentId, depth - 1, alpha, beta);

            //Remember the best score (positive for maximizing player, negative for minimizing player)
            if (isMaximizingPlayer)
            {
                if (score > bestScore)
                {
                    bestScore = score;
                    alpha = Math.Max(alpha, bestScore);
                }
            }
            else
            {
                if (score < bestScore)
                {
                    bestScore = score;
                    beta = Math.Min(beta, bestScore);
                }
            }
        }

        return bestScore;
    }
}