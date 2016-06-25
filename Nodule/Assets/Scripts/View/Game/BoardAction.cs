using System.Linq;
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

            // Check for special rotation case:
            //
            // TODO: 
            var canRotate = !_puzzleState.HasArcAt(nodeView.Position, dir) &&
                            !_puzzleState.HasArcAt(nodeView.Position, dir.Opposite()) &&
                            !(_puzzleState.IsPulled && nodeView.Position.Equals(_puzzleState.PullPosition));

            // Modify the game view accordingly
            if (!movePlayed && canRotate) {
                // Even though no move has been played, if there are no arcs parallel 
                // to the swipe, we can have the nodes/arcs rotate for effect,
                // but not in the case where there is a pulled arc on the node
                _puzzleView.Rotate(nodeView, dir);
            } else if (movePlayed && _puzzleState.IsPulled) {
                // If a pull move has been played, rotate the node
                _puzzleView.Rotate(nodeView, _puzzleState.PulledArcView, dir);
            } else if (movePlayed && !_puzzleState.IsPulled) {
                // If a push move has been played, move the arc to the node, then rotate it
                _puzzleView.MoveArc(nodeView, _puzzleState.PulledArcView);
                _puzzleView.Rotate(nodeView, _puzzleState.PulledArcView, dir);
            }

            // TODO

            // Update node highlighting
            _puzzleView.Highlight(_puzzleState.NonPlayerNodes, false);
            _puzzleView.Highlight(_puzzleState.PlayerNodes, true);

            // Update arc highlighting
            _puzzleView.Highlight(_puzzleState.NonPlayerArcs, false);
            _puzzleView.Highlight(_puzzleState.PlayerArcs, true);

            // Update field highlighting
            _puzzleView.Highlight(_puzzleState.NonPushFields, false);
            _puzzleView.Highlight(_puzzleState.PushFields, true);
        }
    }
}
