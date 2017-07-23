using Core.Data;
using UnityEngine;
using View.Control;
using View.Items;

namespace View.Tween
{
	public class GridTransit : MonoBehaviour
	{
		public float WaveInTravel { get { return GameDef.Get.WaveInTravel; } }
		public float WaveInAudioDelay { get { return GameDef.Get.WaveInAudioDelay; } }
		public float WaveInMoveDelayStart { get { return GameDef.Get.WaveInMoveDelayStart; } }
		public float WaveInMoveDelayOffsetScale { get { return GameDef.Get.WaveInMoveDelayOffsetScale; } }
		public float WaveInTime { get { return GameDef.Get.WaveInTime; } }
		public LeanTweenType WaveInMoveEase { get { return GameDef.Get.WaveInMoveEase; } }
		public LeanTweenType WaveInColorEase { get { return GameDef.Get.WaveInColorEase; } }

		public float WaveOutTravel { get { return GameDef.Get.WaveOutTravel; } }
		public float WaveOutAudioDelay { get { return GameDef.Get.WaveOutAudioDelay; } }
		public float WaveOutMoveDelayStart { get { return GameDef.Get.WaveOutMoveDelayStart; } }
		public float WaveOutMoveDelayOffsetScale { get { return GameDef.Get.WaveOutMoveDelayOffsetScale; } }
		public float WaveOutTime { get { return GameDef.Get.WaveOutTime; } }
		public LeanTweenType WaveOutMoveEase { get { return GameDef.Get.WaveOutMoveEase; } }
		public LeanTweenType WaveOutColorEase { get { return GameDef.Get.WaveOutColorEase; } }

		public void WaveIn(float delay, Vector3 dir)
		{
			var pos = transform.localPosition;

			// Set node far away and transparent
			transform.Translate(WaveInTravel * dir);

			// TODO: use smooth function over linear delay
			var moveDelay = WaveInMoveDelayStart + WaveInMoveDelayOffsetScale * delay;

			// Start a nice animation effect
			LeanTween.moveLocal(gameObject, pos, WaveInTime)
				.setDelay(moveDelay)
				.setEase(WaveInMoveEase);
		}

		public void WaveOut(float delay, Vector3 dir)
		{
			// TODO: use smooth function over linear delay
			var pos = transform.localPosition + WaveOutTravel * dir;
			var moveDelay = WaveOutMoveDelayStart + WaveOutMoveDelayOffsetScale * delay;

			// Start a nice animation effect
			LeanTween.moveLocal(gameObject, pos, WaveOutTime)
				.setDelay(moveDelay)
				.setEase(WaveOutMoveEase);
		}
	}
}
