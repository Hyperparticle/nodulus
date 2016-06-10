using System;
using System.Collections;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Game;
using Assets.Scripts.View.Control;
using Assets.Scripts.View.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.View.Game
{
    public class PuzzleView : MonoBehaviour
    {
        private const int InvalidAudio  = 0;
        private const int ForwardAudio  = 1;
        private const int TakeAudio     = 2;
        private const int PlaceAudio    = 3;
        private const int CompleteAudio = 4;

        public float WinDelay = 1.0f;
    
        private Puzzle _puzzle;
        private ArcView _inversion;

        private PuzzleSpawner _puzzleSpawner;
        private PuzzleScale _puzzleScale;
        private BoardInput _boardInput;
        //private Animator _levelSelectAnimator;
        //private Text _moveText;
        private int _currentLevel;

        //private AudioSource[] _audioSources;

        void Awake()
        {
            _puzzleSpawner = GetComponent<PuzzleSpawner>();
            _puzzleScale = GetComponent<PuzzleScale>();
            _boardInput = GetComponent<BoardInput>();

            //_audioSources = GetComponents<AudioSource>();

            //_levelSelectAnimator = GameObject.FindGameObjectWithTag("LevelSelect").GetComponent<Animator>();
            //_moveText = GameObject.FindGameObjectWithTag("Moves").GetComponent<Text>();

            // Start with level 0
            Init(0);

            // Init all scripts that require additional information on startup
            _puzzleScale.Init(_puzzle.StartNode.Position, _puzzle.BoardSize);
            _boardInput.Init(_puzzleSpawner.NodeMap);
        }

        public void Init(int level)
        {
            _puzzleSpawner.DestroyBoard();

            _puzzle = _puzzleSpawner.SpawnBoard(level);
            _currentLevel = level;

            //_playerScript.Init(_puzzle.Player);

            //_moveText.text = _puzzle.NumMoves.ToString();

            HighlightFields();
        }

        public void NextLevel()
        {
            Init(_currentLevel == Level.LevelCount - 1 ? _currentLevel : _currentLevel + 1);
        }

        public void PrevLevel()
        {
            Init(_currentLevel == 0 ? 0 : _currentLevel - 1);
        }

        public void Swipe(NodeView nodeView, Direction direction)
        {
            if (nodeView == null || direction == Direction.None) { return; }

            // Try to obtain an arc corresponding to the node's position and the 
            // swipe's (opposite) direction.
            ArcView arcView;
            var pointDir = new PointDir(nodeView.Position, direction.Opposite());
            if (!_puzzleSpawner.ArcMap.TryGetValue(pointDir, out arcView)) { return; }

            arcView.transform.parent = nodeView.Rotor;

            var result = _puzzle.PullArc(arcView.Arc, direction.Opposite());

            // TODO: validation
            if (!result)
            {
                Debug.Log("Failed");
                return;
            }

            nodeView.Rotate(direction, () => arcView.ResetParent());
        }

        public void Tap(FieldView fieldView)
        {
            if (fieldView == null) { return; }


        }

        private void HighlightFields()
        {
            //foreach (var field in _puzzle.Fields)
            //    field.FieldScript.Highlight(false);

            //foreach (var placeMove in _puzzle.Reversions)
            //    placeMove.Field.FieldScript.Highlight(true);

            //foreach (var edge in _puzzle.Edges)
            //    edge.EdgeScript.Highlight = false;

            //foreach (var field in _playerScript.Player.Fields.Where(field => field.HasEdge))
            //    field.Edge.EdgeScript.Highlight = true;
        }

        private void HighlightNodes()
        {
            //foreach (var node in _puzzle.Nodes)
            //{
            //    node.NodeScript.Highlight = false;
            //}

            //foreach (var node in _puzzle.Player.Island)
            //{
            //    node.NodeScript.Highlight = true;
            //}
        }

        private IEnumerator WinBoard()
        {
            yield return new WaitForSeconds(WinDelay);
            //_levelSelectAnimator.SetTrigger("Slide In");
            _puzzleSpawner.DestroyBoard();
        }
    


        private static PuzzleView _puzzleView;
        public static PuzzleView Get()
        {
            return _puzzleView ??
                   (_puzzleView = GameObject.FindGameObjectWithTag("PuzzleGame").GetComponent<PuzzleView>());
        }
    }
}
