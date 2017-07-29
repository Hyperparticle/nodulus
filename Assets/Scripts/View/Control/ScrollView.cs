using UnityEngine;
using View.Game;

namespace View.Control
{
	public class ScrollView : MonoBehaviour
	{
		private PuzzleView _puzzleView;
		private PuzzleScale _puzzleScale;

		private void Awake()
		{
			_puzzleView = GetComponentInChildren<PuzzleView>();
			_puzzleScale = _puzzleView.GetComponent<PuzzleScale>();
		}

		public void EnableScroll()
		{
			const float time = 0.3f;
			const float scaleFactor = 0.5f;
			
			var scale = _puzzleView.transform.localScale * scaleFactor;
			var pos = _puzzleScale.Offset * scaleFactor;

			LeanTween.scale(_puzzleView.gameObject, scale, time)
				.setEase(LeanTweenType.easeInSine);
			
			LeanTween.moveLocal(_puzzleView.gameObject, pos, time)
				.setEase(LeanTweenType.easeInSine);
		}
	}
}
