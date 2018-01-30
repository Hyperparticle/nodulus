using UnityEngine;
using View.Control;

namespace View.Tween
{
	/// <summary>
	/// Tweens in/out a game board's lattice (grid) view.
	/// </summary>
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

		public void WaveIn(float delay, float maxDelay, Vector3 dir, LeanTweenType ease, float animationSpeed = 1f, float delayScale = 1f)
		{
			var pos = transform.localPosition;

			// Set node far away and transparent	
			transform.Translate(WaveInTravel / 2f * dir);

			// TODO: use smooth function over linear delay
			var moveDelay = WaveInMoveDelayStart * delayScale + (WaveInMoveDelayOffsetScale * delay) / maxDelay;

			// Start a nice animation effect
			LeanTween.moveLocal(gameObject, pos, WaveInTime * animationSpeed)
				.setDelay(moveDelay)
				.setEase(ease);
		}

		public void WaveOut(float delay, float maxDelay, Vector3 dir, LeanTweenType ease, float animationSpeed = 1f, float delayScale = 1f)
		{
			// TODO: use smooth function over linear delay
			var pos = transform.localPosition + WaveOutTravel * dir;
			var moveDelay = WaveOutMoveDelayStart * delayScale + (WaveOutMoveDelayOffsetScale * delay) / maxDelay;

			// Start a nice animation effect
			LeanTween.moveLocal(gameObject, pos, WaveOutTime * animationSpeed)
				.setDelay(moveDelay)
				.setEase(ease);
		}
	}
}
