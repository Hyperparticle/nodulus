using System;
using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.View.Items
{
    /// <summary>
    /// A NodeView represents the view for an node in the gameboard. It is responsible
    /// for visualizing nodes that connect and rotate arcs.
    /// </summary>
    public class NodeView : MonoBehaviour
    {
        public Transform Rotor;
        public Color NodeColor;
        public Color NodeFinalColor;

        private ScaleScript _nodeScale;
        private Colorizer _colorizer;
        private Transit _transit;

        private readonly Queue<Direction> _rotateQueue = new Queue<Direction>();

        public Node Node { get; private set; }

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
            _transit = GetComponent<Transit>();
        }

        public void Init(Node node, bool inStartIsland, int delay)
        {
            Node = node;

            // TODO: replace nodescale
            _nodeScale.SetNode(node);

            _colorizer.PrimaryColor = node.Final ? NodeFinalColor : NodeColor;

            if (!inStartIsland && !node.Final) {
                _colorizer.Darken(0f);
            }
        }

        public void WaveIn(int delay)
        {
            _transit.WaveIn(delay);
        }

        public void WaveOut(int delay)
        {
            _transit.WaveOut(delay);
        }


        public void Rotate(Direction dir, Action onComplete)
        {
            //if (LeanTween.isTweening(_rotor)) {
            //    // Queue the request, which will get completed after this one is complete
            //    // TODO: set parent objects
            //    //_rotateQueue.Enqueue(dir);
            //    return;
            //}

            _transit.Rotate90(dir, onComplete);
        }

        //private void OnRotateComplete()
        //{
        //    if (_rotateQueue.Count == 0) {
        //        return;
        //    }

        //    var dir = _rotateQueue.Dequeue();
        //    Rotate90(dir);
        //}

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
