using System;
using UnityEngine;
using View.Control;

namespace View.Items
{
    /// <summary>
    /// Controller for a basic GUI button.
    /// </summary>
    public class ButtonScript : MonoBehaviour
    {
        public ButtonType ButtonType;
        public float ButtonDistance = 0.5f;

        public GameObject ButtonShapePrimary;
        public GameObject ButtonShapeSecondary;
        private GameObject CurrentShape;

        public string prefsKey;

        public event Action<ButtonType> ButtonPressed;

        private static float ButtonTransitionTime => GameDef.Get.ButtonTransitionTime;
        private static LeanTweenType ButtonEase => GameDef.Get.ButtonEase;

        private void Awake()
        {
            CurrentShape = ButtonShapePrimary;
        }

        private void Start()
        {
            if (!PlayerPrefs.HasKey(prefsKey)) {
                return;
            }

            var prevShape = CurrentShape;
            CurrentShape = PlayerPrefs.GetInt(prefsKey) == 0 ? ButtonShapePrimary : ButtonShapeSecondary;
            prevShape.SetActive(false);
            CurrentShape.SetActive(true);
        }

        private void OnMouseDown()
        {
            if (LeanTween.isTweening(gameObject)) {
                return;
            }

            ButtonPressed?.Invoke(ButtonType);

            // Play the button animation
            Move();
        }

        private void Move()
        {
            var pos = transform.localPosition;

            LeanTween.moveLocalZ(gameObject, pos.z + ButtonDistance, ButtonTransitionTime)
                .setEase(ButtonEase)
                .setOnComplete(() => {
                    MoveBack(pos);

                    if (CurrentShape == null) {
                        return;
                    }
                    
                    CurrentShape.SetActive(false);
                    CurrentShape = CurrentShape == ButtonShapePrimary ? 
                        ButtonShapeSecondary : ButtonShapePrimary;
                    CurrentShape.SetActive(true);
                });
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
