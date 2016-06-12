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
    /// <summary>
    /// Handles all basic initialization logic to create and destroy the gameboard
    /// </summary>
    public class PuzzleView : MonoBehaviour
    {
        private const int InvalidAudio  = 0;
        private const int ForwardAudio  = 1;
        private const int TakeAudio     = 2;
        private const int PlaceAudio    = 3;
        private const int CompleteAudio = 4;

        public float WinDelay = 1.0f;

        public Puzzle Puzzle { get; private set; }

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
            _puzzleScale.Init(Puzzle.StartNode.Position, Puzzle.BoardSize);
            _boardInput.Init(_puzzleSpawner.NodeMap);
        }

        public void Init(int level)
        {
            _puzzleSpawner.DestroyBoard();

            Puzzle = _puzzleSpawner.SpawnBoard(level);
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

        

        private void HighlightFields()
        {
            foreach (var field in Puzzle.PullFields)
            {
                
            }
        }

        private void HighlightNodes()
        {
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
