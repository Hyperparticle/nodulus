using System;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Control;
using UnityEngine;

namespace Assets.Scripts.View.Items
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

        void Awake()
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

            // TODO: move arc gracefully
            LeanTween.move(gameObject, nodeView.transform, ArcMoveTime)
                .setEase(ArcMoveEase)
                .setOnComplete(onComplete);
            LeanTween.moveLocalZ(gameObject, -_arcScale.Length, ArcMoveTime)
                .setEase(ArcMoveEase);
        }
    }
}
