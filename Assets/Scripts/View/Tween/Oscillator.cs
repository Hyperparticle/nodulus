using UnityEngine;

namespace View.Tween
{
    public class Oscillator : MonoBehaviour
    {
        public float MoveTime = 1f;
        public LeanTweenType MoveEase = LeanTweenType.easeInOutSine;
        public Vector3 MoveDirection = Vector3.right;

        private void Start()
        {
            LeanTween.moveLocal(gameObject, transform.localPosition + MoveDirection, MoveTime)
                .setEase(MoveEase)
                .setLoopPingPong(-1);
        }
    }
}
