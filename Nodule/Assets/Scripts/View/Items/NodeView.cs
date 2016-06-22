using System;
using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Data;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    /// <summary>
    /// A NodeView represents the view for an node in the gameboard. It is responsible
    /// for visualizing nodes that connect and rotate arcs.
    /// </summary>
    public class NodeView : MonoBehaviour
    {
        public Transform Rotor;

        private ScaleScript _nodeScale;
        private Colorizer _colorizer;
        private GameObject _rotor;

        private readonly Queue<RotateRequest> _rotateQueue = new Queue<RotateRequest>();

        private Node Node { get; set; }

        public Point Position
        {
            get { return Node.Position; }
        }

        public Field GetField(Direction dir)
        {
            return Node.Fields[dir];
        }

        void Awake()
        {
            _nodeScale = GetComponent<ScaleScript>();
            _colorizer = GetComponentInChildren<Colorizer>();
        }

        public void Init(Node node, bool inStartIsland)
        {
            _rotor = _colorizer.gameObject;

            Node = node;

            _nodeScale.SetNode(node);

            if (!inStartIsland) {
                _colorizer.Darken();
            }
            if (node.Final) {
                _colorizer.SetSecondary();
            }
        }

        public void Rotate(Direction dir, Action onComplete)
        {
            if (LeanTween.isTweening(_colorizer.gameObject)) {
                // Queue the request, which will get completed after this one is complete
                // TODO: set parent objects
                _rotateQueue.Enqueue(new RotateRequest(dir, onComplete));
                return;
            }

            Rotate90(dir, onComplete);
        }

        private void Rotate90(Direction dir, Action onComplete)
        {
            // Rotate 90 degrees in the direction specified
            LeanTween.rotateAroundLocal(_rotor, dir.Axis(), 90f, 0.5f)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(() => {
                    onComplete();
                    _rotor.transform.localRotation = Quaternion.identity;
                    OnRotateComplete();
                });
        }

        private void OnRotateComplete()
        {
            if (_rotateQueue.Count == 0) {
                return;
            }

            var request = _rotateQueue.Dequeue();
            Rotate90(request.Direction, request.OnComplete);
        }
    }
}
