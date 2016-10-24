using Assets.Scripts.View.Game;
using UnityEngine;

namespace Assets.Scripts.View.Control
{
    public class PanScript : MonoBehaviour
    {
        public const float MouseSensitivity = 0.01f;
        public const KeyCode PanCode = KeyCode.Mouse2;

        public Vector2 StartNodePos = new Vector2(-7.5f, 0f);

        //private PuzzleScale _puzzleScale;

        //private Vector2 _currentPos;

        private Vector2 _boardDimensions;

        //private Vector2 _startPosition;
        private Vector2 _minClamp;
        private Vector2 _maxClamp;

        private Vector3 _lastPosition;

        void Awake()
        {
            _boardDimensions = PuzzleScale.Get.Dimensions;
            //_puzzleScale = GetComponentInChildren<PuzzleScale>();
        }

        //public void PanTo(Vector2 boardPosition)
        //{
        //    //var scaledPos = _puzzleScale.Scale(boardPosition);
        //    //var delta = scaledPos - _currentPos; // Adjusted relative to the current position

        //    //_currentPos = scaledPos;

        //    //Debug.Log(scaledPos);

        //    //LeanTween.moveLocal(gameObject, scaledPos, 1f)
        //    //    .setEase(LeanTweenType.easeInOutSine);
        //}

        void Start()
        {
            _minClamp = _boardDimensions / -2f;
            _maxClamp = _boardDimensions / 2f;
        }

        void Update()
        {
            if (Input.GetKeyDown(PanCode))
            {
                _lastPosition = Input.mousePosition;
            }

            // Pan if dragging
            if (Input.GetKey(PanCode))
            {
                var delta = -_lastPosition + Input.mousePosition;
                var deltaPos = delta * MouseSensitivity;
                var pos = transform.localPosition + deltaPos;

                transform.localPosition = Clamp(pos);

                _lastPosition = Input.mousePosition;
            }
        }

        public void SetBounds(Vector2 min, Vector2 max)
        {
            _minClamp = min;
            _maxClamp = max;
        }

        private Vector2 Clamp(Vector2 pos)
        {
            return new Vector2(
                Mathf.Clamp(pos.x, _minClamp.x, _maxClamp.x),
                Mathf.Clamp(pos.y, _minClamp.y, _maxClamp.y)
            );
        }
    }
}
