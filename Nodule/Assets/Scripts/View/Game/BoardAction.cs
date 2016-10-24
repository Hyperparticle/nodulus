using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Control;
using Assets.Scripts.View.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.View.Game
{
    /// <summary>
    /// Given an action, this component modifies the puzzle model and view accordingly.
    /// </summary>
    public class BoardAction : MonoBehaviour
    {
        public float LevelDelay { get { return GameDef.Get.LevelDelay; } }

        private PuzzleView _puzzleView;
        private PuzzleState _puzzleState;
        //private PanScript _panScript;
        private Text _moveText;
        public GameAudio GameAudio;

        // A lock to prevent multiple moves to be played at the same time
        private bool _viewUpdating;

        void Awake()
        {
            _puzzleView = GetComponent<PuzzleView>();
            _puzzleState = GetComponent<PuzzleState>();
            //_panScript = GameObject.FindGameObjectWithTag("MainView").GetComponent<PanScript>();
            _moveText = GameObject.FindGameObjectWithTag("MoveText").GetComponent<Text>();

            _puzzleView.ViewUpdated += OnViewUpdated;
        }

        /// <summary>
        /// Handle a move action on a node in the given direction
        /// </summary>
        public void Play(NodeView nodeView, Direction dir)
        {
            // If a pull move has been played, rotate the node
            if (_viewUpdating) {
                return;
            }

            _viewUpdating = true;

            // Try to play the move
            var movePlayed = _puzzleState.Play(nodeView, dir);

            // Check for special rotation case
            var canRotate = !_puzzleState.HasArcAt(nodeView.Position, dir) &&
                            !_puzzleState.HasArcAt(nodeView.Position, dir.Opposite()) &&
                            !(_puzzleState.IsPulled && nodeView.Position.Equals(_puzzleState.PullPosition));

            // Modify the game view accordingly
            if (!movePlayed && canRotate) {
                // Even though no move has been played, if there are no arcs parallel 
                // to the swipe, we can have the nodes/arcs rotate for effect
                // (but not in the case where there is a pulled arc on the node)
                _puzzleView.Rotate(nodeView, dir, true);
            } else if (movePlayed && _puzzleState.IsPulled) {
                _puzzleView.Rotate(nodeView, _puzzleState.PulledArcView, dir, true);
                GameAudio.Play(Clip.MovePull);
            } else if (movePlayed && !_puzzleState.IsPulled) {
                // If a push move has been played, move the arc to the node, then rotate it
                _puzzleView.MoveRotate(nodeView, _puzzleState.PulledArcView, dir);
                GameAudio.Play(Clip.MovePush);
            } else {
                _viewUpdating = false;
            }

            // Update node highlighting
            _puzzleView.Highlight(_puzzleState.NonPlayerNodes, false);
            _puzzleView.Highlight(_puzzleState.PlayerNodes, true);

            // Update arc highlighting
            _puzzleView.Highlight(_puzzleState.NonPlayerArcs, false);
            _puzzleView.Highlight(_puzzleState.PlayerArcs, true);

            // Update field highlighting
            _puzzleView.Highlight(_puzzleState.NonPushFields, false);
            _puzzleView.Highlight(_puzzleState.PushFields, true);

            if (_puzzleState.Win) {
                GameAudio.Play(Clip.WinBoard);
                _puzzleState.NextLevel(LevelDelay);
            }

            // Pan camera towards island
            //_panScript.PanTo(_puzzleState.IslandAverage);
        }

        private void OnViewUpdated()
        {
            _viewUpdating = false;

            // Update MoveText
            _moveText.text = _puzzleState.NumMoves.ToString();
        }
    }
}
