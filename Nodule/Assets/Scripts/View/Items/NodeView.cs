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

        private readonly Queue<Direction> _rotateQueue = new Queue<Direction>();

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

            if (inStartIsland) {
                _colorizer.Highlight(true);
            } else if (!node.Final) {
                _colorizer.Darken(true);
            } else {
                _colorizer.PrimaryColor = new Color(27f / 255f, 113f / 255f, 232f / 255f);
                _colorizer.ColorThis(_colorizer.PrimaryColor, true);
            }
        }

        public void Rotate(Direction dir)
        {
            if (LeanTween.isTweening(_rotor)) {
                // Queue the request, which will get completed after this one is complete
                // TODO: set parent objects
                //_rotateQueue.Enqueue(dir);
                return;
            }

            Rotate90(dir);
        }

        private void Rotate90(Direction dir)
        {
            // Grab the axis of the direction, and rotate it relative to the current rotation.
            // This is accomplished by getting the rotation that undoes the current rotation, 
            // and applying it to the absolute axis to get the relative axis we want
            var rot = Quaternion.Inverse(_rotor.transform.localRotation);
            var axis = rot*dir.Axis();

            // Rotate 90 degrees in the direction specified
            LeanTween.rotateAroundLocal(_rotor, axis, 90f, 0.5f)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(OnRotateComplete);
        }

        private void OnRotateComplete()
        {
            if (_rotateQueue.Count == 0) {
                return;
            }

            var dir = _rotateQueue.Dequeue();
            Rotate90(dir);
        }

        public void Highlight(bool enable)
        {
            if (Node.Final) {
                return;
            }

            if (enable) {
                _colorizer.Highlight();
            } else {
                _colorizer.Darken();
            }
        }
    }
}
