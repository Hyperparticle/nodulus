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
		
		private void Awake()
		{
			_puzzleState = GetComponent<PuzzleState>();
			_puzzleScale = GetComponent<PuzzleScale>();
			_canvas = GetComponentInChildren<Canvas>();
			_text = _canvas.GetComponentInChildren<Text>();
		}

		public void Init()
		{
			_canvas.transform.localPosition = -_puzzleScale.Offset - Vector2.up * _puzzleScale.Offset.y;
			_text.text = $"{_puzzleState.Metadata.Number}. {_puzzleState.Metadata.Name}";
		}
	}
}
