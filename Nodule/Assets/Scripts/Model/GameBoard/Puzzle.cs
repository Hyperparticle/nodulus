using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Items;

namespace Assets.Scripts.Model.GameBoard
{
    public class Puzzle
    {
        private readonly GameBoard _gameBoard;

        public Player Player { get; private set; }
        public Node StartNode { get { return _gameBoard.StartNode; } }

        public Arc Inversion { get; private set; }
        
        public int NumMoves { get; private set; }
        public bool Win { get; private set; }
        
        private readonly HashSet<Inversion> _inversions = new HashSet<Inversion>();
        private readonly HashSet<Reversion> _reversions = new HashSet<Reversion>();
        
        public IEnumerable<Inversion> Inversions { get { return _inversions; } }
        public IEnumerable<Reversion> Reversions { get { return _reversions; } }

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
