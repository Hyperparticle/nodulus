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

        public void Rotate(NodeView nodeView, ArcView arcView, Direction dir, bool pull)
        {
            arcView.transform.parent = nodeView.Rotor;
            Rotate(nodeView, dir, pull);
        }

        // TODO: make pull cleaner
        public void Rotate(NodeView nodeView, Direction dir, bool pull)
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
            nodeView.Rotate(dir, () => OnViewUpdated());
        }

        public void Shake(NodeView nodeView, Direction dir)
        {
            // Set all connecting arcs as the parent of this node
            // so that all arcs will rotate accordingly
            var arcViews = _puzzleState.GetArcs(nodeView.Position);
            
            var stay = dir;
            foreach (var pair in arcViews)
            {
                pair.Value.transform.parent = nodeView.Rotor;
            }

            // TODO: cool field rotations
            //var fieldViews = _puzzleState.GetFields(nodeView.Position);

            // Finally, rotate the node!
            nodeView.Shake(dir, () => OnViewUpdated());
        }

        public void MoveRotate(List<NodeView> nodeViews, ArcView arcView, Direction dir)
        {
            var path = _puzzleState.PushNodePath;

            arcView.MoveTo(nodeViews, () => Rotate(nodeViews[nodeViews.Count-1], arcView, dir, false));
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

        public void Shake(Direction dir)
        {
            LeanTween.cancel(gameObject);
            transform.position = _puzzleScale.Offset;

            var dirVector = dir.Vector();

            var shakeAmt = 0.05f;
            var initPos = transform.localPosition;
            var shakePeriodTime = 0.1f;

            LeanTween.moveLocal(gameObject, initPos + dirVector * shakeAmt, shakePeriodTime)
                .setEase(LeanTweenType.easeInSine)
                .setOnComplete(() => {
                    LeanTween.moveLocal(gameObject, initPos, shakePeriodTime)
                        .setEase(LeanTweenType.easeOutSine);
                });
            //    .setLoopClamp()
            //    .setRepeat(-1);

            //// Slow the shake down to zero
            //LeanTween.value(gameObject, shakeAmt, 0f, dropOffTime)
            //    .setEase(LeanTweenType.easeOutQuad)
            //    .setOnUpdate(val => shakeTween.setTo(initPos + dirVector * shakeAmt * val));
        }

        private void OnViewUpdated()
        {
            if (ViewUpdated != null) {
                ViewUpdated();
            }
        }
    }
}
