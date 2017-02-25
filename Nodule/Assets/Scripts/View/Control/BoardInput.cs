using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Game;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Control
{
    /// <summary>
    /// Handles all initial inputs to the game board (i.e., screen swipes and taps)
    /// and converts them to an action.
    /// </summary>
    public class BoardInput : MonoBehaviour
    {
        private BoardAction _boardAction;
        private PuzzleScale _puzzleScale;
        private PanScript _panScript;

        private IDictionary<Point, NodeView> _nodeMap;

        public float MinSwipeDistanceCm { get { return GameDef.Get.MinSwipeDistanceCm; } }
        public float MedSwipeDistanceCm { get { return GameDef.Get.MedSwipeDistanceCm; } }
        public float MaxSwipeDistanceCm { get { return GameDef.Get.MedSwipeDistanceCm; } }

        void Awake()
        {
            _puzzleScale = GetComponent<PuzzleScale>();
            _boardAction = GetComponent<BoardAction>();
            _panScript = GetComponent<PanScript>();
        }

        void Start()
        {
            // Add event handlers for swiping the screen
            var swipeRecognizers = new[] {
                new TKSwipeRecognizer(MinSwipeDistanceCm) { minimumNumberOfTouches = 0, maximumNumberOfTouches = 1 },
                new TKSwipeRecognizer(MedSwipeDistanceCm) { minimumNumberOfTouches = 0, maximumNumberOfTouches = 1 },
                new TKSwipeRecognizer(MaxSwipeDistanceCm) { minimumNumberOfTouches = 0, maximumNumberOfTouches = 1 } 
            };

            foreach (var swipe in swipeRecognizers) {
                swipe.gestureRecognizedEvent += OnSwipe;
                TouchKit.addGestureRecognizer(swipe);
            }

            // Add an event handler for tapping the screen
            var tapRecognizer = new TKTapRecognizer();
            tapRecognizer.gestureRecognizedEvent += OnTap;
            TouchKit.addGestureRecognizer(tapRecognizer);

            var panRecognizer = new TKPanRecognizer() { minimumNumberOfTouches = 2, maximumNumberOfTouches = 3 };
            panRecognizer.gestureRecognizedEvent += OnPan;
            TouchKit.addGestureRecognizer(panRecognizer);
        }

        public void Init(IDictionary<Point, NodeView> nodeMap)
        {
            _nodeMap = nodeMap;
        }

        /// <summary>
        /// Called every time the screen is tapped
        /// </summary>
        private void OnTap(TKTapRecognizer recognizer) {}

        /// <summary>
        /// Called every time the screen is swiped
        /// </summary>
        private void OnSwipe(TKSwipeRecognizer recognizer)
        {
            // Find the nearest node to the swipe (within 1 grid unit), and the swipe direction
            var node = GetNearestNode(recognizer);
            var swipeDirection = recognizer.completedSwipeDirection.ToDirection();

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
        private NodeView GetNearestNode(TKSwipeRecognizer recognizer)
        {
            // TODO: make this more robust

            // Obtain the gesture positions
            var startTouch = Camera.main.ScreenToWorldPoint(recognizer.startPoint);
            var endTouch = Camera.main.ScreenToWorldPoint(recognizer.endPoint);

            // Find the midpoint
            var mid = startTouch + (endTouch - startTouch)/2f;
            var scaledPos = (Vector2) (mid - transform.position);

            // Remove any scaling, and round the position to the nearest integer
            var pos = (scaledPos)/_puzzleScale.Scaling;
            var point = Point.Round(pos);

            // Retrieve the node, if it exists
            NodeView node;
            _nodeMap.TryGetValue(point, out node);
            return node;
        }

        private void OnPan(TKPanRecognizer recognizer)
        {
            _panScript.PanTowards(recognizer.deltaTranslation);
        }
    }
}
