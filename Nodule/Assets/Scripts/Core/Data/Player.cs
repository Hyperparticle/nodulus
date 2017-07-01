using System.Collections.Generic;
using Assets.Scripts.Core.Game;
using Assets.Scripts.Core.Items;
using Assets.Scripts.Core.Moves;

namespace Assets.Scripts.Core.Data
{
    public class Player
    {
        private readonly PlayerState _playerState;
        public PlayerState PlayerState { get { return _playerState; } }

        public int NumMoves { get; private set; }
        public bool Win { get { return _playerState.IsFinal; } }
        
        public Player(GameBoard gameBoard)
        {
            _playerState = new PlayerState(gameBoard);
            MoveTo(gameBoard.StartNode);
        }

        public void MoveTo(Node node)
        {
            _playerState.MoveTo(node);
        }

        public bool IsProximal(Field field)
        {
            return _playerState.Contains(field.ParentNode) || _playerState.Contains(field.ConnectedNode);
        }

        public bool PlayMove(IMove move)
        {
            var movePlayed = move.Play();
            
            if (movePlayed && move is PushMove) {
                var pushMove = move as PushMove;
                NumMoves++;
            }

            return movePlayed;
        }
    }
}
