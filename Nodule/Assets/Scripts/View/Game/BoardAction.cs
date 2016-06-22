using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    /// <summary>
    /// Given an action, this component modifies the puzzle model and view accordingly.
    /// </summary>
    public class BoardAction : MonoBehaviour
    {
        private PuzzleView _puzzleView;
        private PuzzleState _puzzleState;

        void Awake()
        {
            _puzzleView = GetComponent<PuzzleView>();
            _puzzleState = GetComponent<PuzzleState>();
        }

        /// <summary>
        /// Handle a move action on a node in the given direction
        /// </summary>
        public void Play(NodeView nodeView, Direction dir)
        {
            // Try to play the move
            var movePlayed = _puzzleState.Play(nodeView, dir);

            // Check for special rotation case
            var canRotate = !_puzzleState.HasArcAt(nodeView.Position, dir) &&
                            !_puzzleState.HasArcAt(nodeView.Position, dir.Opposite());// && 
                            //!(_puzzleState.IsPulled && _puzzleState.PulledArcView.Arc.Position.Equals(nodeView.Position));

            // Modify the game view accordingly
            if (!movePlayed && canRotate) {
                // Even though no move has been played, if there are no arcs parallel 
                // to the swipe, we can have the nodes/arcs rotate for effect 
                _puzzleView.Rotate(nodeView, dir);
            } else if (movePlayed && _puzzleState.IsPulled) {
                // If a pull move has been played, rotate the node
                _puzzleView.Rotate(nodeView, _puzzleState.PulledArcView, dir);
            } else if (movePlayed && !_puzzleState.IsPulled) {
                // If a push move has been played, move the arc to the node, then rotate it
                _puzzleView.MoveArc(nodeView, _puzzleState.PulledArcView);
                _puzzleView.Rotate(nodeView, _puzzleState.PulledArcView, dir);
            }
        }
    }
}
