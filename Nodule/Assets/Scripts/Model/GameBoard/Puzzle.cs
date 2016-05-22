using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Items;
using Assets.Scripts.Model.Moves;

namespace Assets.Scripts.Model.GameBoard
{
    /// <summary>
    /// A Puzzle specifies the rules for interacting 
    /// with a <seealso cref="GameBoard"/>.
    /// </summary>
    public class Puzzle
    {
        private readonly GameBoard _gameBoard;

        public Player Player { get; private set; }
        public Node StartNode { get { return _gameBoard.StartNode; } }

        public Arc Inversion { get; private set; }
        
        public int NumMoves { get; private set; }
        public bool Win { get; private set; }
        
        private readonly HashSet<PullMove> _inversions = new HashSet<PullMove>();
        private readonly HashSet<PushMove> _reversions = new HashSet<PushMove>();
        
        public IEnumerable<PullMove> Inversions { get { return _inversions; } }
        public IEnumerable<PushMove> Reversions { get { return _reversions; } }

        public Point BoardSize { get { return _gameBoard.Size; } }

        public Puzzle(GameBoard gameBoard)
        {
            _gameBoard = gameBoard;
            Player = new Player(_gameBoard.StartNode);

            UpdateMoves();
        }

        public bool Invert(Arc arc, Direction direction)
        {
            var move = _inversions.FirstOrDefault(takeMove => takeMove.Equals(arc));
            if (move == null || !move.Play(direction)) return false;

            Inversion = arc;
            NumMoves++;
            UpdateMoves();

            return true;
        }

        public bool Revert(Field field)
        {
            var move = _reversions.FirstOrDefault(placeMove => placeMove.Equals(Inversion, field));
            if (move == null || !move.Play()) return false;

            Inversion = null;
            NumMoves++;
            UpdateMoves();
            Win = Player.Win;

            return true;
        }

        private void UpdateMoves()
        {
            _inversions.Clear();
            _reversions.Clear();

            _inversions.UnionWith(FindTakeMoves());
            _reversions.UnionWith(FindPlaceMoves());
        }

        private IEnumerable<Inversion> FindTakeMoves()
        {
            return Player.Fields
                .Where(field => field.HasArc)
                .Select(connection => new Inversion(Player, connection.Arc, Inversion != null))
                .Where(takeMove => takeMove.IsValid);
        }

        private IEnumerable<Reversion> FindPlaceMoves()
        {
            return Player.Fields
                .Select(field => new Reversion(Player, Inversion, field))
                .Where(placeMoves => placeMoves.IsValid);
        }
    }
}
