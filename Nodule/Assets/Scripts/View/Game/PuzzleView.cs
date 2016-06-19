using System.Collections;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Game;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Control;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    /// <summary>
    /// Handles all basic initialization logic to create and destroy the gameboard
    /// </summary>
    public class PuzzleView : MonoBehaviour
    {
        private const int InvalidAudio = 0;
        private const int ForwardAudio = 1;
        private const int TakeAudio = 2;
        private const int PlaceAudio = 3;
        private const int CompleteAudio = 4;

        public float WinDelay = 1.0f;

        public float AnimationSpeed = 1.0f;

        private PuzzleScale _puzzleScale;


        //private Animator _levelSelectAnimator;
        //private Text _moveText;


        //private AudioSource[] _audioSources;


        //_audioSources = GetComponents<AudioSource>();

        //_levelSelectAnimator = GameObject.FindGameObjectWithTag("LevelSelect").GetComponent<Animator>();
        //_moveText = GameObject.FindGameObjectWithTag("Moves").GetComponent<Text>();         

        //_playerScript.Init(_puzzle.Player);

        //_moveText.text = _puzzle.NumMoves.ToString();

        void Start()
        {
            _puzzleScale = GetComponent<PuzzleScale>();
        }

        public void Init(Point startNode, Point boardSize)
        {
            _puzzleScale.Init(startNode, boardSize);
        }

        // TODO
        //private void HighlightFields()
        //{
        //    foreach (var field in Puzzle.PullFields)
        //    {

        //    }
        //}

        //private void HighlightNodes()
        //{
        //}

        // TODO
        //private IEnumerator WinBoard()
        //{
        //    yield return new WaitForSeconds(WinDelay);
        //    //_levelSelectAnimator.SetTrigger("Slide In");
        //    _puzzleSpawner.DestroyBoard();
        //}
    }
}
