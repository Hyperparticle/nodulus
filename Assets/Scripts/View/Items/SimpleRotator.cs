using UnityEngine;

namespace View.Items
{
	public class SimpleRotator : MonoBehaviour
	{
		// TODO: make configurable
		public float Delay = 1f;
		public float Time = 5f;

		public LeanTweenType Ease = LeanTweenType.easeOutBack;
		
		public Vector3 StartRotation;
		public Vector3 EndRotation;

		private void Start()
		{
			Ease = LeanTweenType.easeInOutSine;
			transform.localEulerAngles = -StartRotation;
			
			LeanTween.rotateLocal(gameObject, EndRotation, Time)
				.setDelay(Delay)
				.setEase(Ease);
		}
	}
}
