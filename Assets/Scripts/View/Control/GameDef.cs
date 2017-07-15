using UnityEngine;

namespace View.Control
{
    public class GameDef : MonoBehaviour
    {
        private void Awake()
        {
            Get = this;
        }

        // TODO: Move all constants to a configuration file

        // Board Scaling
        public float Scaling = 2.5f;
        public float NodeScaling = 1.0f;
        public float EdgeScaling = 1.0f;
        public float BoardScaling = 1.0f;
        public float BoardPadding = 1.0f;
        public Vector3 BoardRotation = new Vector3(22.5f, 12.25f, 0f);

        // Board Input
        public float MinSwipeDistanceCm = 0.5f;
        public float MedSwipeDistanceCm = 3f;
        public float MaxSwipeDistanceCm = 4.5f;

        // Item Colors
        // TODO: add color themes
        public Color NodeColor = new Color32(195, 38, 11, 255);
        public Color NodeFinalColor = new Color32(9, 94, 193, 255);
        public Color ArcColor = new Color32(76, 176, 12, 255);
        public Color FieldColor = new Color32(32, 163, 21, 100);

        // Colorizer
        public float DarkBrightnessScale = 0.40f;
        public float ColorTransitionTime = 0.5f;
        public LeanTweenType ColorEase = LeanTweenType.easeInOutSine;

        // Moves
        public float NodeRotateTime = 0.33f;
        public float ArcMoveTime = 0.25f;
        public LeanTweenType ArcMoveEase = LeanTweenType.easeInOutSine;

        // Buttons
        public float ButtonTransitionTime = 0.25f;
        public LeanTweenType ButtonEase = LeanTweenType.easeInOutSine;

        // Board Animations
        public float LevelDelay = 0.2f;

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

        public static GameDef Get { get; private set; }
    }
}
