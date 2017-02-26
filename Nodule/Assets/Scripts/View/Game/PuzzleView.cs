using System;
using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    /// <summary>
    /// Handles all basic initialization logic to create and destroy the gameboard
    /// </summary>
    public class PuzzleView : MonoBehaviour
    {
        private PuzzleScale _puzzleScale;
        private PuzzleState _puzzleState;

        public event Action ViewUpdated;

        void Awake()
        {
            _puzzleScale = GetComponent<PuzzleScale>();
            _puzzleState = GetComponent<PuzzleState>();
        }

        public void Init(Point startNode, Point boardSize)
        {
            _puzzleScale.Init(startNode, boardSize);
            OnViewUpdated();
        }

        public void Rotate(NodeView nodeView, ArcView arcView, Direction dir, bool pull, Action onComplete)
        {
            arcView.transform.parent = nodeView.Rotor;
            Rotate(nodeView, dir, pull, onComplete);
        }

        // TODO: make pull cleaner
        public void Rotate(NodeView nodeView, Direction dir, bool pull, Action onComplete)
        {
            // Set all connecting arcs as the parent of this node
            // so that all arcs will rotate accordingly
            var arcViews = _puzzleState.GetArcs(nodeView.Position);

            var stay = pull ? dir : dir.Opposite();
            foreach (var pair in arcViews) {
                pair.Value.transform.parent = pair.Key == stay ?
                    nodeView.transform :
                    nodeView.Rotor;
            }

            // TODO: cool field rotations
            //var fieldViews = _puzzleState.GetFields(nodeView.Position);

            // Finally, rotate the node!
            nodeView.Rotate(dir, () => { OnViewUpdated(); onComplete(); });
        }

        public void MoveRotate(List<NodeView> nodeViews, ArcView arcView, Direction dir, Action onComplete)
        {
            var path = _puzzleState.PushNodePath;

            arcView.MoveTo(nodeViews, () => Rotate(nodeViews[nodeViews.Count-1], arcView, dir, false, onComplete));
        }

        public void Highlight(IEnumerable<NodeView> nodes, bool enable)
        {
            foreach (var nodeView in nodes) {
                nodeView.Highlight(enable);
            }
        }

        public void Highlight(IEnumerable<ArcView> arcs, bool enable)
        {
            foreach (var arcView in arcs)
            {
                arcView.Highlight(enable);
            }
        }

        public void Highlight(IEnumerable<FieldView> fields, bool enable)
        {
            foreach (var fieldView in fields)
            {
                fieldView.Highlight(enable);
            }
        }

        private void OnViewUpdated()
        {
            if (ViewUpdated != null) {
                ViewUpdated();
            }
        }
    }
}
