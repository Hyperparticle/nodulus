using System;
using System.Collections.Generic;
using Core.Game;
using UnityEngine;
using UnityEngine.UI;

namespace View.Game
{
	/// <summary>
	/// Basic information about the puzzle.
	/// </summary>
	public class PuzzleInfo : MonoBehaviour
	{
		public Color EnableColor;
		public Color DisableColor;
		
		private PuzzleState _puzzleState;
		private PuzzleScale _puzzleScale;

		private Canvas _canvas;
		private Text _text;

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

			_initPos = _canvas.transform.localPosition;
			_canvas.transform.Translate(_hidePos);
		}

		public void Init()
		{
			_canvas.transform.localPosition =_showPos =
				_initPos - (Vector3) _puzzleScale.Offset - Vector3.up * _puzzleScale.Offset.y;

			var language = Levels.CurrentLanguage;
			var levelName = _puzzleState.Metadata.Name;
			if (language != null && !language.Equals("default")) {
				try {
					levelName = _puzzleState.Metadata.Localization[language];
				} catch (Exception e) {
					Debug.LogWarning($"Failed to apply localization for {language} (level " +
					                 $"{_puzzleState.Metadata.Number}) \n {e}");
				}
			}
			
			_text.text = $"{_puzzleState.Metadata.Number}. {levelName}";
			
			_canvas.transform.Translate(_hidePos);
			
			Highlight(false);
		}

		public void Show()
		{
			LeanTween.moveLocal(_canvas.gameObject, _showPos, TransitionTime)
				.setDelay(TransitionDelay)
				.setEase(LeanTweenType.easeOutBack);
		}

		public void Hide()
		{
			LeanTween.moveLocal(_canvas.gameObject, _showPos + _hidePos, TransitionTime)
				.setDelay(TransitionDelay)
				.setEase(LeanTweenType.easeOutSine);
		}

		public void Highlight(bool enable)
		{
			_text.color = enable ? EnableColor : DisableColor;
		}
	}
}
