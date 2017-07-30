using System.Collections.Generic;
using Core.Game;
using UnityEngine;
using View.Game;

namespace View.Control
{
	public class ScrollView : MonoBehaviour
	{
		public GameObject PuzzleGamePrefab;

        public int StartLevel;
		
		private PuzzleView _selectedPuzzleView;
		private PuzzleScale _selectedPuzzleScale;
		private PuzzleState _selectedPuzzleState;

		private bool _scrollEnabled;

		private readonly List<GameObject> _levels = new List<GameObject>();

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
				return;
			}
			
			_scrollEnabled = true;
			
			// TODO: make configurable
			const float time = 0.3f;
			
			_selectedPuzzleView.GetComponent<BoardInput>().enabled = false;
			
			GenerateLevelsList();

			var scaleRatio = new Vector3(0.9f, 0.5f);
			var zoom = CameraScript.CameraZoomToFit(
				_selectedPuzzleScale.Dimensions,
				_selectedPuzzleScale.Margin,
				scaleRatio
			);
			CameraScript.ZoomCamera(zoom, time, LeanTweenType.easeInSine);
		}

		public void DisableScroll()
		{
			foreach (var level in _levels) {
				level.GetComponent<PuzzleSpawner>().DestroyBoard();
				Destroy(level, 5f); // TODO: magic number
			}
			_levels.Clear();
			
			_selectedPuzzleView.GetComponent<BoardInput>().enabled = true;
			
			CameraScript.FitToDimensions(_selectedPuzzleScale.Dimensions, _selectedPuzzleScale.Margin);
		}

		private void GenerateLevelsList()
		{
			// Keep track of the last board's position as the offset for the next board
			// TODO: make configurable
			const float margin = 3f; 
			var prevOffset = _selectedPuzzleScale.Dimensions.y / 2f + margin;
			
			for (var level = _selectedPuzzleState.CurrentLevel - 1; level >= 0; level--) {
				GenerateLevel(level, margin, ref prevOffset, Vector2.up);
			}
			
			prevOffset = _selectedPuzzleScale.Dimensions.y / 2f + margin;
			
			for (var level = _selectedPuzzleState.CurrentLevel + 1; level < Levels.LevelCount; level++) {
				GenerateLevel(level, margin, ref prevOffset, Vector2.down);
			}
		}

		private void GenerateLevel(int level, float margin, ref float prevOffset, Vector2 direction)
		{
			var puzzleGame = Instantiate(PuzzleGamePrefab);
			puzzleGame.name = $"PuzzleGame ({level})";
			puzzleGame.transform.SetParent(transform);
				
			_levels.Add(puzzleGame);

			puzzleGame.GetComponent<BoardInput>().enabled = false;

			// TODO: get board dimensions from puzzle scale before it is fully initialized
			var puzzleScale = puzzleGame.GetComponent<PuzzleScale>();
			var boardHeight = Levels.BuildLevel(level).Size.y * puzzleScale.Scaling / 2f;
				
			prevOffset += boardHeight;
				
			puzzleGame.GetComponent<PuzzleState>().Init(level, direction * prevOffset);
				
			// Add half the board height as the starting point for the next board to spawn
			prevOffset += boardHeight + margin;
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
