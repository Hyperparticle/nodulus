using UnityEngine;
using UnityEngine.UI;

namespace View.Game
{
	public class PuzzleInfo : MonoBehaviour
	{
		private PuzzleState _puzzleState;
		private PuzzleScale _puzzleScale;

		private Canvas _canvas;
		private Text _text;
		private MeshRenderer _bar;

		private Vector3 _initPos;

		private readonly Vector3 _hidePos = new Vector3(-40f, 0f, 0f);
		private Vector3 _showPos;
		
        // TODO: make configurable
		private const float TransitionTime = 1f;
		private const float TransitionDelay = 0.2f;
		
		private void Awake()
		{
			_puzzleState = GetComponentInParent<PuzzleState>();
			_puzzleScale = GetComponentInParent<PuzzleScale>();
			
			_canvas = GetComponentInChildren<Canvas>();
			_text = _canvas.GetComponentInChildren<Text>();
			_bar = _canvas.GetComponentInChildren<MeshRenderer>();

			_initPos = _canvas.transform.localPosition;
			_canvas.transform.Translate(_hidePos);
		}

		public void Init()
		{
			_canvas.transform.localPosition =_showPos =
				_initPos - (Vector3) _puzzleScale.Offset - Vector3.up * _puzzleScale.Offset.y;
	
//			_bar.transform.localScale = new Vector3(_puzzleScale.Offset.x * 2f / _canvas.transform.localScale.x, _bar.transform.localScale.y, _bar.transform.localScale.z);
			
			_text.text = $"{_puzzleState.Metadata.Number}. {_puzzleState.Metadata.Name}";
			
			_canvas.transform.Translate(_hidePos);
		}

		public void Show()
		{
//			if (LeanTween.isTweening(_canvas.gameObject)) {
             //				return;
             //			}
			
			LeanTween.moveLocal(_canvas.gameObject, _showPos, TransitionTime)
				.setDelay(TransitionDelay)
				.setEase(LeanTweenType.easeOutBack);
		}

		public void Hide()
		{
//			if (LeanTween.isTweening(_canvas.gameObject)) {
//				return;
//			}
			
			LeanTween.moveLocal(_canvas.gameObject, _showPos + _hidePos, TransitionTime)
				.setDelay(TransitionDelay)
				.setEase(LeanTweenType.easeOutSine);
		}
	}
}
