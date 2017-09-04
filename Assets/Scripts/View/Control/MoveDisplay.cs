using System;
using UnityEngine;
using UnityEngine.UI;

namespace View.Control
{
    public class MoveDisplay : MonoBehaviour
    {
        private Text _moveText;
        private Text _highscoreText;
        private Transform _highscoreDisplay;

        // TODO: make configurable
        private const float TransitionTime = 1f;
        private const float TransitionDelay = 0.2f;
        
        private Vector3 _highscoreDisplayStart;
        private readonly Vector3 _highscoreDisplayEnd = new Vector3(-6f, 6f, 0f);

        private int _rotateId;
        
        private void Awake()
        {
            _moveText = GameObject.FindGameObjectWithTag("MoveText").GetComponent<Text>();
            _highscoreText = GameObject.FindGameObjectWithTag("HighscoreText").GetComponent<Text>();
            _highscoreDisplay = GameObject.FindGameObjectWithTag("HighscoreDisplay").transform;
            
            _highscoreDisplayStart = _highscoreDisplay.localPosition;
        }

        private void Start()
        {
            _highscoreDisplay.Translate(_highscoreDisplayEnd);
        }

        public void UpdateText(long moves, long highscore, bool resetAnimation = false, bool immediate = false)
        {
            if (LeanTween.isTweening(_rotateId)) {
                return;
            }
            
            // TODO: make configurable
            const float time = 0.8f;
            const float normalDelay = 2f;
            const float immediateDelay = 1f;
            
            var delay = immediate ? immediateDelay : normalDelay;

            highscore = highscore <= 0 ? 9999 : highscore;

            var movesString = moves.ToString();
            var highscoreString = highscore.ToString();

            const float maxTextLength = 4;
            
            if (movesString.Length > maxTextLength) {
                movesString = "lol";
            }

            if (highscoreString.Length > maxTextLength) {
                highscoreString = "9999";
            }
            
//            if (!LeanTween.isTweening(_highscoreDisplay.gameObject)) {
                if (highscore == 9999) {
                    LeanTween.cancel(_highscoreDisplay.gameObject);
                    LeanTween.moveLocal(_highscoreDisplay.gameObject, _highscoreDisplayStart + _highscoreDisplayEnd, TransitionTime)
                        .setDelay(TransitionDelay)
                        .setEase(LeanTweenType.easeOutSine);
                } else {
                    LeanTween.cancel(_highscoreDisplay.gameObject);
                    LeanTween.moveLocal(_highscoreDisplay.gameObject, _highscoreDisplayStart, TransitionTime)
                        .setDelay(delay + time)
                        .setEase(LeanTweenType.easeOutSine);
                }
//            }
//            else {
//                _highscoreText.text = highscoreString;
//            }
            
//            if (!_highscoreText.text.Equals(highscoreString)) {
//                RotateDisplay(_highscoreDisplay.gameObject, immediate, (() => {
//                    _highscoreText.text = highscoreString;
//                }));
//            }

            if (resetAnimation && (!_moveText.text.Equals(movesString) || !_highscoreText.text.Equals(highscoreString))) {
                _rotateId = RotateDisplay(gameObject, time, delay, () => {
                    _moveText.text = movesString;
                    _highscoreText.text = highscoreString;
                });
            } else {
                _moveText.text = movesString;
            }
        }

        private static int RotateDisplay(GameObject obj, float time, float delay, Action update)
        {
            return LeanTween.rotateAroundLocal(obj, Vector3.right, 360f, time)
                .setDelay(delay)
                .setEase(LeanTweenType.easeInOutBack)
                .setOnUpdate(rotation => {
                    rotation = rotation < 0 ? rotation + 360f : rotation;
                    if (!(rotation > 180f) || !(rotation < 270f)) {
                        return;
                    }

                    update();
                })
                .id;
        }
    }
}
