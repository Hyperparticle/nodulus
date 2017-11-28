using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using View.Items;

namespace View.Control
{
    public class NavigationScript : MonoBehaviour
    {
        private ScrollView _scrollView;

        private Transform _moveDisplay;
        private Transform _scrollHelper;
        private MenuRotator _buttonSelect;

        private Vector3 _moveDisplayStart;
        private Vector3 _scrollHelperStart;
        private Vector3 _buttonSelectStart;
        
        private readonly Vector3 _moveDisplayEnd = new Vector3(-4f, 1.5f, 0f);
        private readonly Vector3 _buttonSelectEnd = new Vector3(3f, 1.5f, 0f);
        private readonly Vector3 _scrollHelperEnd = new Vector3(5f, 0f, 0f);
        
        // TODO: make configurable
        private const float TransitionTime = 1f;
        private const float TransitionDelay = 0.2f;

        private readonly IDictionary<ButtonType, Action<ScrollView>> _buttonActions = 
                new Dictionary<ButtonType, Action<ScrollView>> {
            { ButtonType.LevelSelect, scrollView => scrollView.EnableScroll() },
            { ButtonType.RestartLevel, scrollView => scrollView.RestartLevel() },
            { ButtonType.Settings, scrollView => scrollView.ShowSettings() },
            { ButtonType.MusicToggle, scrollView => scrollView.ToggleMusic() },
            { ButtonType.SfxToggle, scrollView => scrollView.ToggleSfx() },
        };

        private void Awake()
        {
            _scrollView = GameObject.FindGameObjectWithTag("MainView").GetComponent<ScrollView>();

            _moveDisplay = GetComponentInChildren<MoveDisplay>().transform;
            _scrollHelper = GameObject.FindGameObjectWithTag("ScrollHelper").transform;
            _buttonSelect = GetComponentInChildren<MenuRotator>();

            var buttons = GameObject.FindGameObjectsWithTag("Button")
                .Select(obj => obj.GetComponent<ButtonScript>());

            foreach (var button in buttons) {
                button.ButtonPressed += buttonState => _buttonActions[buttonState](_scrollView);
            }
        }

        private void Start()
        {
            _moveDisplayStart = _moveDisplay.localPosition;
            _scrollHelperStart = _scrollHelper.localPosition;
            _buttonSelectStart = _buttonSelect.transform.localPosition;
            
            _moveDisplay.Translate(_moveDisplayEnd);
            _scrollHelper.Translate(_scrollHelperEnd);
            _buttonSelect.transform.Translate(_buttonSelectEnd);
            
            Show();
        }

        public void Show()
        {
            LeanTween.moveLocal(_moveDisplay.gameObject, _moveDisplayStart, TransitionTime)
                .setDelay(TransitionDelay)
                .setEase(LeanTweenType.easeOutSine);
            
            LeanTween.moveLocal(_scrollHelper.gameObject, _scrollHelperStart + _scrollHelperEnd, TransitionTime)
                .setDelay(TransitionDelay)
                .setEase(LeanTweenType.easeOutSine);
            
            LeanTween.moveLocal(_buttonSelect.gameObject, _buttonSelectStart, TransitionTime)
                .setDelay(TransitionDelay)
                .setEase(LeanTweenType.easeOutSine);
            
            _buttonSelect.ShowLevelButtons();
        }

        public void Hide()
        {
            LeanTween.moveLocal(_moveDisplay.gameObject, _moveDisplayStart + _moveDisplayEnd, TransitionTime)
                .setDelay(TransitionDelay)
                .setEase(LeanTweenType.easeOutSine);
            
            LeanTween.moveLocal(_scrollHelper.gameObject, _scrollHelperStart, TransitionTime)
                .setDelay(TransitionDelay)
                .setEase(LeanTweenType.easeOutSine);
            
            _buttonSelect.ShowSettingsButtons();
        }

    }
}
