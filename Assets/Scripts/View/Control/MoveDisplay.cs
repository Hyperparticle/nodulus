using System;
using UnityEngine;
using UnityEngine.UI;

namespace View.Control
{
    public class MoveDisplay : MonoBehaviour
    {
        private Text _moveText;
        
        private void Awake()
        {
            _moveText = GameObject.FindGameObjectWithTag("MoveText").GetComponent<Text>();
        }


        public void UpdateText(string text, bool resetAnimation = false)
        {
            // TODO: make configurable
            const float time = 0.8f;
            const float delay = 2f;

            if (resetAnimation && !_moveText.text.Equals(text)) {
                LeanTween.rotateAroundLocal(gameObject, Vector3.right, 360f, time)
                    .setDelay(delay)
                    .setEase(LeanTweenType.easeInOutBack)
                    .setOnUpdate(rotation => {
                        rotation = rotation < 0 ? rotation + 360 : rotation;
                        if (rotation > 180f && rotation < 270f) {
                            _moveText.text = text;
                        }
                    });
            } else {
                _moveText.text = text;
            }
        }
    }
}
