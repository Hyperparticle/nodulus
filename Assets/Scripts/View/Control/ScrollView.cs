using UnityEngine;
using View.Game;

namespace View.Control
{
	public class ScrollView : MonoBehaviour
	{
		public GameObject PuzzleGamePrefab;

		private PuzzleView _selectedPuzzleView;
		private PuzzleScale _selectedPuzzleScale;
		private PuzzleState _selectedPuzzleState;

		private bool _scrollEnabled;
		
        public int StartLevel;

		private void Awake()
		{
			_selectedPuzzleView = GetComponentInChildren<PuzzleView>();
			_selectedPuzzleScale = _selectedPuzzleView.GetComponent<PuzzleScale>();
			_selectedPuzzleState = _selectedPuzzleView.GetComponent<PuzzleState>();
			
			_selectedPuzzleScale.PuzzleInit += OnPuzzleInit;
		}

		private void Start()
		{
            // Start with the initially defined start level
            _selectedPuzzleState.Init(StartLevel);
		}

		public void EnableScroll()
		{
			if (_scrollEnabled) {
				_scrollEnabled = false;
				DisableScroll();
			}
			
			_scrollEnabled = true;
			
			// TODO: make configurable
			const float time = 0.3f;
			const float scaleFactor = 0.5f;
			
			var scale = _selectedPuzzleView.transform.localScale * scaleFactor;
			var pos = (Vector3) _selectedPuzzleScale.Offset * scaleFactor;
			
			LeanTween.moveLocal(_selectedPuzzleView.gameObject, pos, time)
				.setEase(LeanTweenType.easeInSine);

			LeanTween.scale(_selectedPuzzleView.gameObject, scale, time)
				.setEase(LeanTweenType.easeInSine)
				.setOnComplete(() => {
					var puzzleGame = Instantiate(PuzzleGamePrefab);
					puzzleGame.transform.SetParent(transform);
					puzzleGame.transform.localScale = scale;
					
					var level = _selectedPuzzleState.CurrentLevel - 1;
					var p = puzzleGame.transform.localPosition + Vector3.up * _selectedPuzzleScale.Dimensions.y;
					
					puzzleGame.GetComponent<PuzzleState>().Init(level, p);
				});
		}

		public void DisableScroll()
		{
			CameraScript.FitToDimensions(_selectedPuzzleScale.NodeScaling, _selectedPuzzleScale.Dimensions);
		}

		private void OnPuzzleInit()
		{
			if (_scrollEnabled) {
				return;
			}
			
			CameraScript.FitToDimensions(_selectedPuzzleScale.NodeScaling, _selectedPuzzleScale.Dimensions);
		}
		
	}
}
