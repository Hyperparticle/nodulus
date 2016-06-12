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

        private BoardActions _boardActions;
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
            _boardActions = GetComponent<BoardActions>();
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
            var field = GetNearestField(recognizer);

            _boardActions.Tap(field);
        }

        /// <summary>
        /// Called every time the screen is swiped
        /// </summary>
        private void OnSwipe(TKSwipeRecognizer recognizer)
        {
            //Debug.Log(recognizer);

            // Find the nearest node to the swipe (within 1 grid unit)
            var node = GetNearestNode(recognizer);

            // Notify the puzzle of the swipe
            var swipeDirection = recognizer.completedSwipeDirection.ToDirection();
            _boardActions.Swipe(node, swipeDirection);
        }

        private NodeView GetNearestNode(TKAbstractGestureRecognizer recognizer)
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

        private FieldView GetNearestField(TKAbstractGestureRecognizer recognizer)
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
            return null;
            // TODO
        }
    }
}
