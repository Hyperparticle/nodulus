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
            // TODO: make configurable
            const float maxDistance = 100f;
            RaycastHit hit;
            
            // Start a raycast from the screen to the board
            var ray = Camera.main.ScreenPointToRay(screenPoint);
            if (!Physics.Raycast(ray, out hit, maxDistance)) {
                return null;
            }
            
            // Convert hit position to relative coordinates
            var relativePos = Quaternion.Inverse(transform.rotation) * (hit.point - transform.position);

            // Remove any scaling
            var scaledPos = relativePos / _puzzleScale.Scaling;
            
            // Offset the position by the rotation of the board that has slightly shifted the cube centroids
            var rot = transform.rotation.eulerAngles;
            var rotOffset = new Vector3(Mathf.Sin(Mathf.Deg2Rad * rot.y), -Mathf.Sin(Mathf.Deg2Rad * rot.x)) / 2f;
            
            var pos = scaledPos - rotOffset;
            
            // Round the position to the nearest integer
            var point = Point.Round(pos);
            
            // Retrieve the node, if it exists
            NodeView node;
            _nodeMap.TryGetValue(point, out node);
            return node;
        }
        
//        private void OnDrawGizmosSelected()
//        {
//            Gizmos.color = new Color(1, 0, 0, 0.5F);
//            Gizmos.DrawWireCube(transform.position, Vector3.one);
//        }

//        private FieldView GetNearestField(TKTapRecognizer recognizer)
//        {
//            var pos = Camera.main.ScreenToWorldPoint(recognizer.startTouchLocation()) / _puzzleScale.Scaling;
//            return null;
//        }
    }
}
