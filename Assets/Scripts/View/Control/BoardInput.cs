using System.Collections.Generic;
using System.Linq;
using Core.Data;
using UnityEngine;
using View.Game;
using View.Items;

namespace View.Control
{
    /// <summary>
    /// Handles all initial inputs to the game board (i.e., screen swipes and taps)
    /// and converts them to an action.
    /// </summary>
    public class BoardInput : MonoBehaviour
    {
        private BoardAction _boardAction;
        private PuzzleScale _puzzleScale;

        private IDictionary<Point, NodeView> _nodeMap;

        private TKSwipeRecognizer _swipeRecognizer;
        private TKTapRecognizer _tapRecognizer;
        
        public float MinSwipeDistanceCm => GameDef.Get.MinSwipeDistanceCm;

        private void Awake()
        {
            _puzzleScale = GetComponent<PuzzleScale>();
            _boardAction = GetComponent<BoardAction>();
        }

        private void Start()
        {
            // Add event handlers for swiping the screen
            _swipeRecognizer = new TKSwipeRecognizer(MinSwipeDistanceCm) {
                minimumNumberOfTouches = 1,
                maximumNumberOfTouches = 10,
                timeToSwipe = 1f
            };

            _swipeRecognizer.gestureRecognizedEvent += OnSwipe;
            TouchKit.addGestureRecognizer(_swipeRecognizer);

            // Add an event handler for tapping the screen
            _tapRecognizer = new TKTapRecognizer();
            _tapRecognizer.gestureRecognizedEvent += OnTap;
            TouchKit.addGestureRecognizer(_tapRecognizer);
        }

        private void OnDestroy()
        {
            if (_swipeRecognizer == null || _tapRecognizer == null) {
                return;
            }
            
            TouchKit.removeGestureRecognizer(_swipeRecognizer);
            TouchKit.removeGestureRecognizer(_tapRecognizer);
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
            if (!enabled) {
                return;
            }
            
//            // Find the nearest node to the tap
//            var field = GetNearestField(recognizer);
//
//            if (field == null) {
//                return;
//            }

//            _boardAction.Play(field);
        }

        /// <summary>
        /// Called every time the screen is swiped
        /// </summary>
        private void OnSwipe(TKSwipeRecognizer recognizer)
        {
            if (!enabled) {
                return;
            }
            
            // Find the nearest node to the swipe (within 1 grid unit), and the swipe direction
            var swipeDirection = recognizer.completedSwipeDirection.ToDirection();
            var node = GetNearestNode(recognizer.startPoint) ?? GetNearestNode(recognizer.endPoint);

            // If the swipe is invalid, don't do anything
            if (node == null || swipeDirection == Direction.None) {
                return;
            }

            // Otherwise, play the move
            _boardAction.Play(node, swipeDirection);
        }

        /// <summary>
        /// Finds the nearest node to the gesture
        /// </summary>
        private NodeView GetNearestNode(Vector2 screenPoint)
        {
            // TODO: make this more robust

            // Obtain the gesture positions
            var startTouch = Camera.main.ScreenToWorldPoint(screenPoint);
            var scaledPos = (Vector2) (startTouch - transform.position);

            // Remove any scaling, and round the position to the nearest integer
            var pos = scaledPos/_puzzleScale.Scaling;
            var point = Point.Round(pos);

            // Retrieve the node, if it exists
            NodeView node;
            _nodeMap.TryGetValue(point, out node);
            return node;
        }

//        private FieldView GetNearestField(TKTapRecognizer recognizer)
//        {
//            var pos = Camera.main.ScreenToWorldPoint(recognizer.startTouchLocation()) / _puzzleScale.Scaling;
//            return null;
//        }
    }
}
