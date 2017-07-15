using UnityEngine;
using View.Game;

namespace View.Control
{
    public class PanScript : MonoBehaviour
    {
        public const float MouseSensitivity = 0.01f;
        public const KeyCode PanCode = KeyCode.Mouse2;

        private PuzzleScale _puzzleScale;
        private Vector3 _lastPosition;

        private void Awake()
        {
            _puzzleScale = GetComponentInChildren<PuzzleScale>();
        }

//        public void PanTo(Vector2 boardPosition)
//        {
//            var scaledPos = _puzzleScale.Scale(boardPosition) + _puzzleScale.Offset;
//
//            LeanTween.moveLocal(gameObject, -scaledPos, 1f)
//                .setEase(LeanTweenType.easeInOutSine);
//        }

//        public void PanTowards(Vector3 delta)
//        {
//            var pos = transform.localPosition + delta / 150;
//            transform.localPosition = _puzzleScale.Clamp(pos);
//            _lastPosition = Input.mousePosition;
//        }
//
//        private void Update()
//        {
//            if (Input.GetKeyDown(PanCode))
//            {
//                _lastPosition = Input.mousePosition;
//            }
//
//            // Pan if dragging
//            if (Input.GetKey(PanCode))
//            {
//                var delta = -_lastPosition + Input.mousePosition;
//                var deltaPos = delta * MouseSensitivity;
//                var pos = transform.localPosition + deltaPos;
//
//                transform.localPosition = _puzzleScale.Clamp(pos);
//
//                _lastPosition = Input.mousePosition;
//            }
//        }
    }
}
