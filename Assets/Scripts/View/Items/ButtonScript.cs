using System;
using UnityEngine;
using View.Control;

namespace View.Items
{
    public class ButtonScript : MonoBehaviour
    {
        public ButtonType ButtonType;
        public float ButtonDistance = 0.5f;

        public event Action<ButtonType> ButtonPressed;

        private static float ButtonTransitionTime => GameDef.Get.ButtonTransitionTime;
        private static LeanTweenType ButtonEase => GameDef.Get.ButtonEase;

        private void OnMouseDown()
        {
            if (LeanTween.isTweening(gameObject)) {
                return;
            }

            // Invoke the event
            ButtonPressed?.Invoke(ButtonType);

            // Play the button animation
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

    public enum ButtonType
    {
        LevelSelect,
        RestartLevel,
        Settings,
        MusicToggle,
        SfxToggle
    }
}
