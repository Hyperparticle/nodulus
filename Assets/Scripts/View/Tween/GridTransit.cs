using UnityEngine;
using View.Control;

namespace View.Tween
{
	public class GridTransit : MonoBehaviour
	{
		public float WaveInTravel => GameDef.Get.WaveInTravel / 1.5f;
		public float WaveInMoveDelayStart => 0.4f;
		public float WaveInMoveDelayOffsetScale => GameDef.Get.WaveInMoveDelayOffsetScale;
		public float WaveInTime => GameDef.Get.WaveInTime;
		public LeanTweenType WaveInMoveEase => GameDef.Get.WaveInMoveEase;
		public LeanTweenType WaveInColorEase => GameDef.Get.WaveInColorEase;

		public float WaveOutTravel => GameDef.Get.WaveOutTravel / 1.5f;
		public float WaveOutMoveDelayStart => 0f;
		public float WaveOutMoveDelayOffsetScale => GameDef.Get.WaveOutMoveDelayOffsetScale;
		public float WaveOutTime => GameDef.Get.WaveOutTime;
		public LeanTweenType WaveOutMoveEase => GameDef.Get.WaveOutMoveEase;
		public LeanTweenType WaveOutColorEase => GameDef.Get.WaveOutColorEase;

		public void WaveIn(float delay, Vector3 dir, LeanTweenType ease)
		{
			var pos = transform.localPosition;

			// Set node far away and transparent	
			transform.Translate(WaveInTravel / 2f * dir);

			// TODO: use smooth function over linear delay
			var moveDelay = WaveInMoveDelayStart + WaveInMoveDelayOffsetScale * delay;

			// Start a nice animation effect
			LeanTween.moveLocal(gameObject, pos, WaveInTime)
				.setDelay(moveDelay)
				.setEase(ease);
		}

		public void WaveOut(float delay, Vector3 dir, LeanTweenType ease)
		{
			// TODO: use smooth function over linear delay
			var pos = transform.localPosition + WaveOutTravel * dir;
			var moveDelay = WaveOutMoveDelayStart + WaveOutMoveDelayOffsetScale * delay;

			// Start a nice animation effect
			LeanTween.moveLocal(gameObject, pos, WaveOutTime)
				.setDelay(moveDelay)
				.setEase(ease);
		}
	}
}
