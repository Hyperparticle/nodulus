using UnityEngine;
using View.Game;

namespace View.Control
{
	public class ScrollView : MonoBehaviour
	{
		private PuzzleView _puzzleView;

		private void Awake()
		{
			_puzzleView = GetComponentInChildren<PuzzleView>();
		}

		public void Enable()
		{
			const float time = 0.3f;
			
			var scale = _puzzleView.transform.localScale;
			scale.Scale(new Vector3(0.5f, 0.5f, 0.5f));

			LeanTween.scale(_puzzleView.gameObject, scale, time)
				.setEase(LeanTweenType.easeInSine);
		}
	}
}
