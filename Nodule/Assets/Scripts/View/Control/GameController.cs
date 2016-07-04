using UnityEngine;

namespace Assets.Scripts.View.Control
{
    public class GameController : MonoBehaviour
    {
        // Board Scaling
        public float Scaling = 2.5f;
        public float NodeScaling = 1.0f;
        public float EdgeScaling = 1.0f;
        public float BoardScaling = 1.0f;
        public float BoardPadding = 1.0f;
        public Vector3 BoardRotation = new Vector3(22.5f, 12.25f, 0f);

        // Board Input
        public float MinSwipeDistanceCm = 1.5f;
        public float MedSwipeDistanceCm = 3f;
        public float MaxSwipeDistanceCm = 4.5f;

        // Item Colors
        public Color NodeColor;
        public Color NodeFinalColor;
        public Color ArcColor;
        public Color FieldColor;

        // Colorizer
        public float DarkBrightnessScale = 0.40f;
        public float ColorTransitionTime = 0.5f;
        public LeanTweenType ColorEase = LeanTweenType.easeInOutSine;

        // Buttons
        public float ButtonTransitionTime = 0.25f;

        // Board Animations
        public float LevelDelay = 0.2f;

        // Node Animations
        public float NodeRotateTime = 0.33f;

        public float WaveInTravel = 10f;
        public float WaveInAudioDelay = 0.1f;
        public float WaveInMoveDelayStart = 1.25f;
        public float WaveInMoveDelayOffsetScale = 0.1f;
        public float WaveInTime = 1f;
        public LeanTweenType WaveInMoveEase = LeanTweenType.easeOutBack;
        public LeanTweenType WaveInColorEase = LeanTweenType.easeOutExpo;

        public float WaveOutTravel = 10f;
        public float WaveOutAudioDelay = 0f;
        public float WaveOutMoveDelayStart = 0f;
        public float WaveOutMoveDelayOffsetScale = 0.05f;
        public float WaveOutTime = 0.75f;
        public LeanTweenType WaveOutMoveEase = LeanTweenType.easeInBack;
        public LeanTweenType WaveOutColorEase = LeanTweenType.easeInExpo;
    }
}
