using System;
using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    /// <summary>
    /// Handles all basic initialization logic to create and destroy the gameboard
    /// </summary>
    public class PuzzleView : MonoBehaviour
    {
        //private const int InvalidAudio = 0;
        //private const int ForwardAudio = 1;
        //private const int TakeAudio = 2;
        //private const int PlaceAudio = 3;
        //private const int CompleteAudio = 4;

        //public float WinDelay = 1.0f;

        //public float AnimationSpeed = 1.0f;

        private PuzzleScale _puzzleScale;
        private PuzzleState _puzzleState;

        //private Animator _levelSelectAnimator;
        //private Text _moveText;


        //private AudioSource[] _audioSources;


        //_audioSources = GetComponents<AudioSource>();

        //_levelSelectAnimator = GameObject.FindGameObjectWithTag("LevelSelect").GetComponent<Animator>();
        //_moveText = GameObject.FindGameObjectWithTag("Moves").GetComponent<Text>();         

        //_playerScript.Init(_puzzle.Player);

        //_moveText.text = _puzzle.NumMoves.ToString();

        void Awake()
        {
            _puzzleScale = GetComponent<PuzzleScale>();
            _puzzleState = GetComponent<PuzzleState>();
        }

        public void Init(Point startNode, Point boardSize)
        {
            _puzzleScale.Init(startNode, boardSize);
        }

        // TODO
        //private IEnumerator WinBoard()
        //{
        //    yield return new WaitForSeconds(WinDelay);
        //    //_levelSelectAnimator.SetTrigger("Slide In");
        //    _puzzleSpawner.DestroyBoard();
        //}

        public void Rotate(NodeView nodeView, ArcView arcView, Direction direction)
        {
            arcView.transform.parent = nodeView.Rotor;

            Rotate(nodeView, direction, arcView.ResetParent);
        }

        public void Rotate(NodeView nodeView, Direction direction)
        {
            Rotate(nodeView, direction, () => { });
        }

        private void Rotate(NodeView nodeView, Direction direction, Action onComplete)
        {
            // Set all connecting arcs as the parent of this node
            // so that all arcs will rotate accordingly
            var arcViews = _puzzleState.GetArcs(nodeView.Position);

            foreach (var arc in arcViews) {
                arc.transform.parent = nodeView.Rotor;
            }

            // Finally, rotate the node!
            nodeView.Rotate(direction, () => {
                foreach (var arc in arcViews) {
                    arc.ResetParent();
                }

                onComplete();
            });
        }

        public void MoveArc(NodeView nodeView, ArcView arcView)
        {
            // TODO
            //var arcs = _puzzleSpawner.GetArcs(nodeView.Position);

            //nodeView.Rotate(direction, () =>
            //{
            //    foreach (var arcView in arcViews)
            //    {
            //        arcView.ResetParent();
            //    }
            //});
        }
    }
}
