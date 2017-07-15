using System;
using System.Collections.Generic;
using Core.Items;
using UnityEngine;
using View.Control;

namespace View.Items
{
    /// <summary>
    /// An ArcView represents the view for an arc in the gameboard. It is responsible
    /// for visualizing arcs rotating and sliding across the gameboard.
    /// </summary>
    public class ArcView : MonoBehaviour
    {
        public Color ArcColor { get { return GameDef.Get.ArcColor; } }
        public float ArcMoveTime { get { return GameDef.Get.ArcMoveTime; } }
        public LeanTweenType ArcMoveEase { get { return GameDef.Get.ArcMoveEase; } }

        private ScaleScript _arcScale;
        private Colorizer _colorizer;

        public Transform Parent { private get; set; }

        public Arc Arc { get; private set; }

        private void Awake()
        {
            _arcScale = GetComponent<ScaleScript>();
            _colorizer = GetComponent<Colorizer>();
        }

        public void Init(Arc arc, Transform parent, bool inStartIsland)
        {
            Arc = arc;
            Parent = parent;

            _arcScale.SetArc(arc);

            _colorizer.PrimaryColor = ArcColor;

            if (!inStartIsland) {
                _colorizer.Darken(0f);
            }
        }

        public void ResetParent()
        {
            transform.parent = Parent;
        }

        public void Highlight(bool enable)
        {
            if (enable) {
                _colorizer.Highlight();
            } else {
                _colorizer.Darken();
            }
        }

        public void MoveTo(NodeView nodeView, Action onComplete)
        {
            // Move to the same node
            if (nodeView.transform.Equals(transform.parent.parent)) {
                onComplete();
                return;
            }

            transform.parent = nodeView.transform;

            LeanTween.move(gameObject, nodeView.transform, ArcMoveTime)
                .setEase(ArcMoveEase)
                .setOnComplete(onComplete);
            LeanTween.moveLocalZ(gameObject, -_arcScale.Length, ArcMoveTime)
                .setEase(ArcMoveEase);
        }

        /// <summary>
        /// Moves this arc along a path of nodes specified in the given node list
        /// </summary>
        public void MoveTo(List<NodeView> nodeViews, Action onComplete)
        {
            var list = new LinkedList<NodeView>(nodeViews);
            MoveNext(list.First, onComplete);
        }

        /// <summary>
        /// Recursively applies arc movement to a node based on a list of nodes
        /// </summary>
        private void MoveNext(LinkedListNode<NodeView> nodeViews, Action onComplete)
        {
            if (nodeViews == null) {
                return;
            }

            var head = nodeViews.Value;
            var tail = nodeViews.Next;

            if (tail == null) {
                MoveTo(head, onComplete);
                return;
            }

            MoveTo(head, () => MoveNext(tail, onComplete));
        }
    }
}
