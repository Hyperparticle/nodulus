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

        //private PlayerScript _playerScript;
        private PuzzleSpawner _puzzleSpawner;
        private PuzzleScale _puzzleScale;
        private BoardInput _boardInput;
        //private Animator _levelSelectAnimator;
        //private Text _moveText;
        private int _currentLevel;

        //private AudioSource[] _audioSources;

        //private Vector3 _initPosition;

 
        void Awake()
        {
            //_playerScript = GetComponentInChildren<PlayerScript>();
            _puzzleSpawner = GetComponent<PuzzleSpawner>();
            _puzzleScale = GetComponent<PuzzleScale>();
            _boardInput = GetComponent<BoardInput>();

            //_audioSources = GetComponents<AudioSource>();

            //_levelSelectAnimator = GameObject.FindGameObjectWithTag("LevelSelect").GetComponent<Animator>();
            //_moveText = GameObject.FindGameObjectWithTag("Moves").GetComponent<Text>();
            //_initPosition = transform.localPosition;

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

            //_puzzleSpawner.CreateBackdrop(this);


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
            if (direction == Direction.None || nodeView == null) return;

            // TODO: validation
            nodeView.Rotate(direction);

            //Debug.Log(nodeView);
            //Debug.Log(direction);
        }

        public void Pull(ArcView arcView, Direction direction)
        {
            //if (arcView.Transitioning || !_puzzle.PullArc(arcView.Edge, direction))
            //{
            //    _audioSources[InvalidAudio].Play();
            //    return;
            //}

            _inversion = arcView;
        
            //_moveText.text = _puzzle.NumMoves.ToString();
            //_audioSources[TakeAudio].Play();
            HighlightFields();
            HighlightNodes();
        }

        public void Push(FieldView fieldView)
        {
            //if (_inversion == null || _inversion.Transitioning || !_puzzle.Push(fieldView.Field))
            //{
            //    _audioSources[InvalidAudio].Play();
            //    return;
            //}

            //var opposite = !_playerScript.Player.ParentInIsland(fieldView.Field);
            //_inversion.Push(fieldView, opposite);

            //_moveText.text = _puzzle.NumMoves.ToString();

            if (_puzzle.Win)
            {
                //_audioSources[CompleteAudio].Play();
                StartCoroutine(WinBoard());
            }
            else
            {
                //_audioSources[PlaceAudio].Play();
                HighlightFields();
                HighlightNodes();
            }
        }

        public void Play(NodeView node)
        {
            //var reversion = _puzzle.Reversions.FirstOrDefault(placeMove => placeMove.Field.ContainsNode(node.Node));
            //if (reversion != null)
                //Push(reversion.Field.FieldScript);

            //var inversion = _puzzle.Inversions.FirstOrDefault(takeMove => takeMove.Edge.ContainsNode(node.Node));
            //if (reversion != null)
            //    Push(reversion.Field.FieldView);
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
