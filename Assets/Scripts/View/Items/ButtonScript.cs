using System;
using UnityEngine;
using View.Control;

namespace View.Items
{
    public class ButtonScript : MonoBehaviour
    {
        public ButtonState ButtonState;

        public event Action<ButtonState> ButtonPressed;

        public float ButtonTransitionTime { get { return GameDef.Get.ButtonTransitionTime; } }
        public LeanTweenType ButtonEase { get { return GameDef.Get.ButtonEase; } }
        private const float ButtonDistance = 0.5f;

        private void OnMouseDown()
        {
            if (LeanTween.isTweening(gameObject)) {
                return;
            }

            if (ButtonPressed != null) {
                ButtonPressed(ButtonState);
            }

            Move();
        }

        private void Move()
        {
            var pos = transform.localPosition;

            LeanTween.moveLocalZ(gameObject, pos.z + ButtonDistance, ButtonTransitionTime)
                .setEase(ButtonEase)
                .setOnComplete(() => MoveBack(pos));
        }

        private void MoveBack(Vector3 pos)
        {
            LeanTween.moveLocalZ(gameObject, pos.z, ButtonTransitionTime)
                .setEase(ButtonEase);
        }
    }

    public enum ButtonState
    {
        Left,
        Select,
        Right
    }
}
