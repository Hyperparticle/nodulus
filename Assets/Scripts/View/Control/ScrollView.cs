using Core.Game;
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
			const float time = 0.4f;
			const float zoom = 10f;
			var cameraFitRatio = new Vector2(1f, 0.3f);
			
			var scaleFactor = CameraScript.CameraFitScale(
				_selectedPuzzleScale.Dimensions,
				cameraFitRatio,
				zoom
			);
			
			var scale = _selectedPuzzleView.transform.localScale * scaleFactor;
			var pos = (Vector3) _selectedPuzzleScale.Offset * scaleFactor;
			
			LeanTween.moveLocal(_selectedPuzzleView.gameObject, pos, time)
				.setEase(LeanTweenType.easeInSine);

			LeanTween.scale(_selectedPuzzleView.gameObject, scale, time)
				.setEase(LeanTweenType.easeInSine)
				.setOnComplete(() => GenerateLevelsList(scale));
			
			// TODO: magic numbers
			CameraScript.ZoomCamera(zoom, time, LeanTweenType.easeInSine);
		}

		public void DisableScroll()
		{
			CameraScript.FitToDimensions(_selectedPuzzleScale.Dimensions, _selectedPuzzleScale.Margin);
		}

		private void GenerateLevelsList(Vector3 scale)
		{
			// Keep track of the last board's position as the offset for the next board
			const float margin = 0f; // TODO: magic number
			var prevOffset = Vector3.zero;
			var nextOffset = Vector3.up * (_selectedPuzzleScale.Dimensions.y + margin);
			
			for (var level = _selectedPuzzleState.CurrentLevel - 1; level >= 0; level--) {
                var puzzleGame = Instantiate(PuzzleGamePrefab);
                puzzleGame.name = $"PuzzleGame ({level})";
                puzzleGame.transform.SetParent(transform);

				var cameraFitRatio = new Vector2(1f, 0.3f);
			
				var scaleFactor = CameraScript.CameraFitScale(
					_selectedPuzzleScale.Dimensions,
					cameraFitRatio
				);
				
                puzzleGame.transform.localScale *= scaleFactor;

//				puzzleGame.transform.position += Vector3.up * scaleFactor;
				
                puzzleGame.GetComponent<PuzzleState>().Init(level, nextOffset);
				
				
				
				// TODO: get board dimensions from puzzle scale before it is fully initialized
				var puzzleScale = puzzleGame.GetComponent<PuzzleScale>();
				var boardHeight = Levels.BuildLevel(level).Size.Y * puzzleScale.Scaling;

				var offset = Vector3.up * (boardHeight + margin);
				nextOffset += offset;
			}
		}

		private void OnPuzzleInit()
		{
			if (_scrollEnabled) {
				return;
			}
			
			CameraScript.FitToDimensions(_selectedPuzzleScale.Dimensions, _selectedPuzzleScale.Margin);
		}
		
	}
}
