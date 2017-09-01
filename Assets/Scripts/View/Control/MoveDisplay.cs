using System;
using UnityEngine;
using UnityEngine.UI;

namespace View.Control
{
    public class MoveDisplay : MonoBehaviour
    {
        private Text _moveText;

        private int _rotateId;
        
        private void Awake()
        {
            _moveText = GameObject.FindGameObjectWithTag("MoveText").GetComponent<Text>();
        }

        public void UpdateText(string text, bool resetAnimation = false, bool immediate = false)
        {
            if (LeanTween.isTweening(_rotateId)) {
                return;
            }
            
            // TODO: make configurable
            const float time = 0.8f;
            const float normalDelay = 2f;
            const float immediateDelay = 1f;

            var delay = immediate ? immediateDelay : normalDelay;

            if (resetAnimation && !_moveText.text.Equals(text)) {
                _rotateId = LeanTween.rotateAroundLocal(gameObject, Vector3.right, 360f, time)
                    .setDelay(delay)
                    .setEase(LeanTweenType.easeInOutBack)
                    .setOnUpdate(rotation => {
                        rotation = rotation < 0 ? rotation + 360 : rotation;
                        if (rotation > 180f && rotation < 270f) {
                            _moveText.text = text;
                        }
                    })
                    .id;
            } else {
                _moveText.text = text;
            }
        }
    }
}
