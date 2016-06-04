using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Game;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Control
{
    public class BoardInput : MonoBehaviour
    {
        public float MinSwipeDistanceCm = 3f;

        private PuzzleView _puzzleView;
        private PuzzleScale _puzzleScale;

        private IDictionary<Point, NodeView> _nodeMap;

        void Start()
        {
            // Add an event handler for swiping the screen
            var swipeRecognizer = new TKSwipeRecognizer(MinSwipeDistanceCm);
            swipeRecognizer.gestureRecognizedEvent += OnSwipe;
            TouchKit.addGestureRecognizer(swipeRecognizer);

            // Add an event handler for tapping the screen
            var tapRecognizer = new TKTapRecognizer();
            tapRecognizer.gestureRecognizedEvent += OnTap;
            TouchKit.addGestureRecognizer(tapRecognizer);

            _puzzleScale = GetComponent<PuzzleScale>();
            _puzzleView = GetComponent<PuzzleView>();
        }

        public void Init(IDictionary<Point, NodeView> nodeMap)
        {
            _nodeMap = nodeMap;
        }

        /// <summary>
        /// Called every time the screen is tapped
        /// </summary>
        private void OnTap(TKTapRecognizer recognizer)
        {
            //Debug.Log(recognizer);

            // Find the nearest node to the tap (within 1 grid unit)
            var node = GetNearest(recognizer);

            // TODO
        }

        /// <summary>
        /// Called every time the screen is swiped
        /// </summary>
        private void OnSwipe(TKSwipeRecognizer recognizer)
        {
            //Debug.Log(recognizer);

            // Find the nearest node to the swipe (within 1 grid unit)
            var node = GetNearest(recognizer);

            // Notify the puzzle of the swipe
            var swipe = recognizer.completedSwipeDirection.ToDirection();
            _puzzleView.Swipe(node, swipe);
        }

        private NodeView GetNearest(TKAbstractGestureRecognizer recognizer)
        {
            // Obtain the gesture position
            var touch = recognizer.touchLocation();
            var scaledPos = (Vector2) Camera.main.ScreenToWorldPoint(touch);

            // Remove any scaling, and round the position to the nearest integer
            var pos = (scaledPos + _puzzleScale.Dimensions / 2f) / _puzzleScale.Scaling;
            var point = Point.Round(pos);

            // Retrieve the node, if it exists
            NodeView node;
            _nodeMap.TryGetValue(point, out node);
            return node;
        }
    }
}
