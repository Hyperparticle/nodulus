using Assets.Scripts.View.Game;
using UnityEngine;

namespace Assets.Scripts.View.Control
{
    public class PanScript : MonoBehaviour
    {
        public const float MouseSensitivity = 0.01f;
        public const KeyCode PanCode = KeyCode.Mouse2;

        private Vector2 _boardDimensions;

        private Vector2 _minClamp;
        private Vector2 _maxClamp;

        private Vector3 _lastPosition;


        void Start()
        {
            _boardDimensions = GameObject.FindGameObjectWithTag("PuzzleGame")
                .GetComponent<PuzzleScale>()
                .Dimensions;

            _minClamp = _boardDimensions / -2f;
            _maxClamp = _boardDimensions /  2f;
        }

        void Update()
        {
            if (Input.GetKeyDown(PanCode))
            {
                _lastPosition = Input.mousePosition;
            }

            // Pan the camera if dragging
            if (Input.GetKey(PanCode))
            {
                var delta = -_lastPosition + Input.mousePosition;
                var deltaPos = delta * MouseSensitivity;
                var pos = transform.position + deltaPos;

                transform.localPosition = Clamp(pos);

                _lastPosition = Input.mousePosition;
            }
        }

        private Vector2 Clamp(Vector2 pos)
        {
            return new Vector2(Mathf.Clamp(pos.x, _minClamp.x, _maxClamp.x), Mathf.Clamp(pos.y, _minClamp.y, _maxClamp.y));
        }
    }
}
