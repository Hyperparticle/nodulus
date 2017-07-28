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
        private ScrollView _scrollView;

        private readonly IDictionary<ButtonType, Action<ScrollView>> _buttonActions = 
                new Dictionary<ButtonType, Action<ScrollView>> {
            { ButtonType.LevelSelect, scrollView => scrollView.Enable() }
        };

        private void Awake()
        {
            _scrollView = GameObject.FindGameObjectWithTag("MainView").GetComponent<ScrollView>();

            var buttons = GetComponentsInChildren<ButtonScript>();

            foreach (var button in buttons) {
                button.ButtonPressed += buttonState => _buttonActions[buttonState](_scrollView);
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
