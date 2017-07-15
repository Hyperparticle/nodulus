using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using View.Game;
using View.Items;

namespace View.Control
{
    public class NavigationScript : MonoBehaviour
    {
        private PuzzleState _puzzleState;

        private readonly IDictionary<ButtonState, Action<PuzzleState>> _buttonActions = new Dictionary<ButtonState, Action<PuzzleState>> {
            { ButtonState.Left, puzzleState => puzzleState.PrevLevel() },
            { ButtonState.Right, puzzleState => puzzleState.NextLevel() },
            { ButtonState.Select, puzzleState => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex) }
        };

        private void Awake()
        {
            _puzzleState = GameObject.FindGameObjectWithTag("PuzzleGame")
                .GetComponent<PuzzleState>();

            var buttons = GetComponentsInChildren<ButtonScript>();

            foreach (var button in buttons) {
                button.ButtonPressed += buttonState => _buttonActions[buttonState](_puzzleState);
            }
        }

        private void Start()
        {
            // Slide right
            var pos = transform.localPosition;
            transform.Translate(20 * Vector3.left);

            LeanTween.moveLocal(gameObject, pos, 1f)
                .setDelay(0.25f)
                .setEase(LeanTweenType.easeOutSine);
        }

    }
}
