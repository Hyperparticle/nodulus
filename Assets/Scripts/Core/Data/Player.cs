using Core.Game;
using Core.Items;
using Core.Moves;

namespace Core.Data
{
    /// <summary>
    /// A player contains state specifying score and win information, along with functions to update the player's
    /// position in the game board.
    /// </summary>
    public class Player
    {
        public PlayerState PlayerState { get; }

        public long NumMoves { get; private set; }
        public long MovesBestScore { get; private set; }
        public bool Win => PlayerState.IsFinal;

        public Player(GameBoard gameBoard)
        {
            PlayerState = new PlayerState(gameBoard);
            
            MoveTo(gameBoard.StartNode);

            NumMoves += gameBoard.Metadata.Moves;
            MovesBestScore = gameBoard.Metadata.MovesBestScore;
        }

        /// <summary>
        /// Move the player to the given node. Note: external validation is necessary to enforce game rules.
        /// </summary>
        /// <param name="node"></param>
        public void MoveTo(Node node)
        {
            PlayerState.MoveTo(node);

            if (Win && (NumMoves < MovesBestScore || MovesBestScore == 0)) {
                MovesBestScore = NumMoves + 1;
            }
        }

        /// <summary>
        /// Whether the player can access the field (a node in the player island touches the field)
        /// </summary>
        public bool IsProximal(Field field)
        {
            return PlayerState.Contains(field.ParentNode) || PlayerState.Contains(field.ConnectedNode);
        }

        /// <summary>
        /// Attempts to play the given move. Returns true if the player move is valid and therefore was played.
        /// </summary>
        public bool PlayMove(IMove move)
        {
            var movePlayed = move.Play();
            
            if (movePlayed && move is PushMove) {
                NumMoves++;
            }

            return movePlayed;
        }
    }
}
