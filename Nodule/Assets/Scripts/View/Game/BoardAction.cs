using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Utility;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    /// <summary>
    /// Given an action, this component modifies the puzzle model and view accordingly
    /// </summary>
    public class BoardAction : MonoBehaviour
    {
        private PuzzleView _puzzleView;
        private PuzzleState _puzzleState;

        void Start()
        {
            _puzzleView = GetComponent<PuzzleView>();
            _puzzleState = GetComponent<PuzzleState>();
        }

        /// <summary>
        /// Handle a move action on a node in the given direction
        /// </summary>
        public void Play(NodeView nodeView, Direction direction)
        {
            var pos = nodeView.Position;
            var movePlayed = _puzzleState.Play(nodeView, direction);


        }

        /// <summary>
        /// Initiate a pull action
        /// </summary>
        private void PullNode(NodeView nodeView, Direction direction, bool movePlayed)
        {
            // Try to obtain an arc corresponding to the node's position and the swipe's (opposite) direction.
            var pos = nodeView.Position;
            var arcDir = direction.Opposite();

            // If there are no arcs parallel to the swipe, we can have the nodes/arcs
            // rotate for effect (even though no move was played). Exit early if that is not true.
            if (_arcMap.ContainsKeys(pos, direction) || _arcMap.ContainsKeys(pos, direction.Opposite())) {
                return;
            }

            // Play the node rotate animation
            PullRotate(nodeView, direction);

            // If the move was played, update the arc map
            if (!movePlayed) {
                return;
            }

            _pulledArcView = _arcMap[pos, arcDir];

            _arcMap.Remove(_pulledArcView.Arc.Position);
            _arcMap.Remove(_pulledArcView.Arc.ConnectedPosition);
        }

        /// <summary>
        /// Initiate a push action
        /// </summary>
        private void PushNode(NodeView nodeView, Direction direction, bool movePlayed)
        {
            // Try to obtain a field corresponding to the node's position and the swipe's direction.
            var pos = nodeView.Position;
            var arcDir = direction;

            if (movePlayed) {
                
            }

            var field = nodeView.GetField(direction);

            var result = _puzzleView.Puzzle.PushArc(field);
            if (!result) {
                Debug.Log("Failed push");
                return;
            }


            // Update the arc map
            var arc = arcView.Arc;
            _arcMap.Add(arc.Position, arc.Direction, arcView);
            _arcMap.Add(arc.ConnectedPosition, arc.Direction.Opposite(), arcView);

            var fieldView = _fieldMap[nodeView.Position, direction];

            // Rotate the node
            PushRotate(nodeView, direction);

            _pulledArcView = null;
        }

        private void PullRotate(NodeView nodeView, Direction direction)
        {
            // Set all connecting arcs as the parent of this node
            // so that all arcs will rotate accordingly
            var arcs = _arcMap.GetValues(nodeView.Position);
            var arcViews = arcs as IList<ArcView> ?? arcs.ToList();
            foreach (var arc in arcViews) {
                arc.transform.parent = nodeView.Rotor;
            }

            // Finally, rotate the node!
            nodeView.Rotate(direction, () => {
                foreach (var arcView in arcViews) {
                    arcView.ResetParent();
                }
            });
        }

        private void PushRotate(NodeView nodeView, Direction direction)
        {
            //var arcs = _puzzleSpawner.GetArcs(nodeView.Position);

            //nodeView.Rotate(direction, () =>
            //{
            //    foreach (var arcView in arcViews)
            //    {
            //        arcView.ResetParent();
            //    }
            //});
        }
    }
}
