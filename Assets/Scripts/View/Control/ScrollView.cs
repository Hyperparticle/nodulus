using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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

		private float _listBottom;
		private float _listTop;
		private Vector3 _velocity;

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

			var panRecognizer = new TKPanRecognizer();
			panRecognizer.gestureRecognizedEvent += OnPan;
			panRecognizer.gestureCompleteEvent += OnPanComplete;
			TouchKit.addGestureRecognizer(panRecognizer);
		}

		private void Update()
		{
			transform.position += _velocity;

			var clampedPos = Mathf.Clamp(transform.position.y, _listBottom, _listTop);
			transform.position = new Vector2(transform.position.x, clampedPos);
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

			_listBottom = 0f;
			_listTop = 0f;
			
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

			_listBottom = -prevOffset;
			prevOffset = _selectedPuzzleScale.Dimensions.y / 2f + margin;
			
			for (var level = _selectedPuzzleState.CurrentLevel + 1; level < Levels.LevelCount; level++) {
				GenerateLevel(level, margin, ref prevOffset, Vector2.down);
			}

			_listTop = prevOffset;
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

		private void OnPan(TKPanRecognizer recognizer)
		{
			if (!_scrollEnabled) {
				return;
			}

			_velocity = Vector2.zero;
			transform.position += Vector3.up * recognizer.deltaTranslation.y / 100f;
		}

		private void OnPanComplete(TKPanRecognizer recognizer)
		{
			if (!_scrollEnabled) {
				return;
			}
			
			// TODO: make configurable
			var delta = recognizer.deltaTranslation.y;
			var velocityMagnitude = Mathf.Abs(delta) < 5f ? 0f : Mathf.Clamp(delta, -100f, 100f);
			const float scalingFactor = 100f;
			
			_velocity = Vector3.up * velocityMagnitude / scalingFactor;
		}
	}
}
