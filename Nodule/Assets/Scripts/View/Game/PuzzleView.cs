using System.Collections;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Game;
using Assets.Scripts.View.Builders;
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

        public float Scaling = 2.5f;
        public float NodeScaling = 1.0f;
        public float EdgeScaling = 1.0f;
        public float BoardScaling = 1.0f;
        public float BoardPadding = 1.0f;

        public Color NodeColor;
        public Color EdgeColor;
        public Color FieldColor;
        public Color NodeHighlightColor;
        public Color EdgeHighlightColor;
        public Color FieldHighlightColor;
        public Color NodeFinalColor;
        public float WinDelay = 1.0f;
    
        private Puzzle _puzzle;
        private ArcView _inversion;

        //private PlayerScript _playerScript;
        private PuzzleSpawner _puzzleSpawner;
        //private Animator _levelSelectAnimator;
        private Text _moveText;
        private int _currentLevel;

        private AudioSource[] _audioSources;

        private Vector3 _initPosition;

        void Awake()
        {
            //_playerScript = GetComponentInChildren<PlayerScript>();
            _puzzleSpawner = GetComponentInChildren<PuzzleSpawner>();

            _audioSources = GetComponents<AudioSource>();

            //_levelSelectAnimator = GameObject.FindGameObjectWithTag("LevelSelect").GetComponent<Animator>();
            //_moveText = GameObject.FindGameObjectWithTag("Moves").GetComponent<Text>();
            _initPosition = transform.localPosition;

            Init(0);
        }

        public void Init(int level)
        {
            _puzzleSpawner.DestroyBoard();

            transform.localScale = Vector3.one;
        
            _puzzle = _puzzleSpawner.SpawnBoard(level);
            _currentLevel = level;

            //_playerScript.Init(_puzzle.Player);

            //_moveText.text = _puzzle.NumMoves.ToString();

            _puzzleSpawner.CreateBackdrop(this);

            //BoardScaling = CameraScript.Fit(Dimensions, BoardPadding, BoardPadding + 2.0f);
            //transform.localScale = Vector3.one * BoardScaling;
            //transform.localPosition = -Dimensions * BoardScaling / 2 + _initPosition;

            transform.localPosition = -(Vector3)_puzzle.StartNode.Position * Scaling;

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

        public void Invert(ArcView arcView, Direction direction)
        {
            if (arcView.Transitioning || !_puzzle.PullArc(arcView.Edge, direction))
            {
                _audioSources[InvalidAudio].Play();
                return;
            }

            _inversion = arcView;
            _inversion.Invert(direction);
        
            //_moveText.text = _puzzle.NumMoves.ToString();
            _audioSources[TakeAudio].Play();
            HighlightFields();
            HighlightNodes();
        }

        public void Revert(FieldView fieldView)
        {
            if (_inversion == null || _inversion.Transitioning || !_puzzle.Revert(fieldView.Field))
            {
                _audioSources[InvalidAudio].Play();
                return;
            }

            var opposite = !_playerScript.Player.ParentInIsland(fieldView.Field);
            _inversion.Revert(fieldView, opposite);

            //_moveText.text = _puzzle.NumMoves.ToString();

            if (_puzzle.Win)
            {
                _audioSources[CompleteAudio].Play();
                StartCoroutine(WinBoard());
            }
            else
            {
                _audioSources[PlaceAudio].Play();
                HighlightFields();
                HighlightNodes();
            }
        }

        public void Play(NodeView node)
        {
            var reversion = _puzzle.Reversions.FirstOrDefault(placeMove => placeMove.Field.ContainsNode(node.Node));
            if (reversion != null)
                Revert(reversion.Field.FieldScript);

            //var inversion = _puzzle.Inversions.FirstOrDefault(takeMove => takeMove.Edge.ContainsNode(node.Node));
            //if (reversion != null)
            //    Revert(reversion.Field.FieldView);
        }

        private void HighlightFields()
        {
            foreach (var field in _puzzle.Fields)
                field.FieldScript.Highlight(false);

            foreach (var placeMove in _puzzle.Reversions)
                placeMove.Field.FieldScript.Highlight(true);

            foreach (var edge in _puzzle.Edges)
                edge.EdgeScript.Highlight = false;

            foreach (var field in _playerScript.Player.Fields.Where(field => field.HasEdge))
                field.Edge.EdgeScript.Highlight = true;
        }

        private void HighlightNodes()
        {
            foreach (var node in _puzzle.Nodes)
            {
                node.NodeScript.Highlight = false;
            }

            foreach (var node in _puzzle.Player.Island)
            {
                node.NodeScript.Highlight = true;
            }
        }

        private IEnumerator WinBoard()
        {
            yield return new WaitForSeconds(WinDelay);
            //_levelSelectAnimator.SetTrigger("Slide In");
            _puzzleSpawner.DestroyBoard();
        }
    
        private Vector3 Dimensions
        {
            get
            {
                var width  = _puzzle.BoardSize.x*Scaling;
                var height = _puzzle.BoardSize.y*Scaling;
                return new Vector2(width, height);
            }
        }
    }
}
