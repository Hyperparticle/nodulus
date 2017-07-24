using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data;
using UnityEngine;
using UnityEngine.UI;
using View.Control;
using View.Items;

namespace View.Game
{
    /// <summary>
    /// Given an action, this component modifies the puzzle model and view accordingly.
    /// </summary>
    public class BoardAction : MonoBehaviour
    {
        public float LevelDelay => GameDef.Get.LevelDelay;

        private PuzzleView _puzzleView;
        private PuzzleState _puzzleState;
        private Text _moveText;
        private GameAudio _gameAudio;
        
        private readonly Queue<Tuple<NodeView, Direction>> _moveQueue 
            = new Queue<Tuple<NodeView, Direction>>(); 
            

        // A lock to prevent multiple moves to be played at the same time
        private bool _viewUpdating;

        private const int MaxMovesInQueue = 2;

        private void Awake()
        {
            _puzzleView = GetComponent<PuzzleView>();
            _puzzleState = GetComponent<PuzzleState>();
            _moveText = GameObject.FindGameObjectWithTag("MoveText").GetComponent<Text>();
            _gameAudio = GameObject.FindGameObjectWithTag("GameAudio").GetComponent<GameAudio>();

            _puzzleView.ViewUpdated += OnViewUpdated;
        }

        /// <summary>
        /// Handle a move action on a node in the given direction
        /// </summary>
        public void Play(NodeView nodeView, Direction dir)
        {
            if (_viewUpdating || LeanTween.isTweening(nodeView.gameObject)) {
                // If the animations are running, queue up the move
                if (_moveQueue.Count < MaxMovesInQueue) {
                    _moveQueue.Enqueue(new Tuple<NodeView, Direction>(nodeView, dir));
                }
                
                return;
            }

            _viewUpdating = true;

            // Try to play the move
            var movePlayed = _puzzleState.Play(nodeView, dir);
            
            // Modify the game view accordingly
            if (!movePlayed) {
                // Even though no move has been played, if there are no arcs parallel 
                // to the swipe, we can have the nodes/arcs rotate for effect
                // (but not in the case where there is a pulled arc on the node)
                var canRotate = !_puzzleState.HasArcAt(nodeView.Position, dir) &&
                                !_puzzleState.HasArcAt(nodeView.Position, dir.Opposite());
                var pushMove = _puzzleState.IsPulled && nodeView.Position.Equals(_puzzleState.PullPosition);

                if (pushMove) {
                    // In the case of an invalid push move, shake the node
                    _puzzleView.Shake(nodeView, dir);
                    _gameAudio.Play(GameClip.InvalidRotate);
                } else if (canRotate) {
                    _puzzleView.Rotate(nodeView, dir, true);
                    _gameAudio.Play(GameClip.NodeRotate90);
                } else {
                    _puzzleView.Shake(nodeView, dir);
                    _gameAudio.Play(GameClip.InvalidRotate);
                }
            } else if (_puzzleState.IsPulled) {
                // Pull move
                _puzzleView.Rotate(nodeView, _puzzleState.PulledArcView, dir, true);
                _puzzleState.PulledArcView.PullSound();
                _puzzleView.Shake(dir);
                HighlightAll();
            } else {
                // Push move
                // If a push move has been played, move the arc to the node, then rotate it
                _puzzleView.MoveRotate(_puzzleState.PushNodePath, _puzzleState.PulledArcView, dir);
                _puzzleView.Shake(dir);
                HighlightAll();
            }
        }

        private void HighlightAll()
        {
            // Update node highlighting
            _puzzleView.Highlight(_puzzleState.NonPlayerNodes, false);
            _puzzleView.Highlight(_puzzleState.PlayerNodes, true);

            // Update arc highlighting
            _puzzleView.Highlight(_puzzleState.NonPlayerArcs, false);
            _puzzleView.Highlight(_puzzleState.PlayerArcs, true);

            // Update field highlighting
            _puzzleView.Highlight(_puzzleState.NonPushFields, false);
            _puzzleView.Highlight(_puzzleState.PushFields, true);
            
        }

        private void OnViewUpdated()
        {
            _viewUpdating = false;

            // Update MoveText
            _moveText.text = _puzzleState.NumMoves.ToString();

            if (_puzzleState.Win) {
                _gameAudio.Play(GameClip.WinBoard);
                LeanTween.cancel(gameObject);

                foreach (var node in _puzzleState.PlayerNodes.Where(nodeView => nodeView.Node.Final)) {
                    _puzzleView.ConnectArcs(node);
                    node.WinAnimation();
                }

                _puzzleState.NextLevel(LevelDelay);
            } else if (_moveQueue.Count > 0) {
                var move = _moveQueue.Dequeue();
                Play(move.Item1, move.Item2);
            }
        }
    }
}
