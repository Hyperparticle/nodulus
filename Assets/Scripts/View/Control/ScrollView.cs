using System;
using System.Collections.Generic;
using System.Linq;
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
		
		private GameObject _selectedLevel;

		private bool _scrollEnabled;

		private GameObject[] _levels;
		private Tuple<float, float>[] _levelBounds;

		private float _listBottom;
		private float _listTop;
		private Vector3 _velocity;

		private void Awake()
		{
			_selectedLevel = GetComponentInChildren<PuzzleView>().gameObject;
			_selectedLevel.GetComponent<PuzzleScale>().PuzzleInit += OnPuzzleInit;
		}

		private void Start()
		{
            // Start with the initially defined start level
            _selectedLevel.GetComponent<PuzzleState>().Init(StartLevel);

			var panRecognizer = new TKPanRecognizer();
			panRecognizer.gestureRecognizedEvent += OnPan;
			panRecognizer.gestureCompleteEvent += OnPanComplete;
			TouchKit.addGestureRecognizer(panRecognizer);
		}

		private void Update()
		{
			if (!_scrollEnabled) {
				return;
			}
			
			// TODO: make configurable
			const float damping = 0.98f;
			
			transform.position += _velocity;
			_velocity *= damping;

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
			
			_selectedLevel.GetComponent<PuzzleState>().BoardEnabled = false;
			
			_levelBounds = new Tuple<float, float>[Levels.LevelCount];
			_levels = new GameObject[Levels.LevelCount];
			
			GenerateLevelsList();

			// TODO: make configurable
			const float time = 0.3f;
			
			var puzzleScale = _selectedLevel.GetComponent<PuzzleScale>();
			var scaleRatio = new Vector3(0.9f, 0.5f);
			var zoom = CameraScript.CameraZoomToFit(puzzleScale.Dimensions, puzzleScale.Margin, scaleRatio);
			CameraScript.ZoomCamera(zoom, time, LeanTweenType.easeInSine);
			
			_scrollEnabled = true;
		}

		public void DisableScroll()
		{
			_selectedLevel = _levels[FindLevel(-transform.position.y)];
			
			foreach (var level in _levels.Where(level => !level.Equals(_selectedLevel))) {
				level.transform.parent = null;
				level.GetComponent<PuzzleSpawner>().DestroyBoard();
				Destroy(level, 5f); // TODO: magic number
			}
			
			_levels = new GameObject[0];
			_levelBounds = new Tuple<float, float>[0];

			_listBottom = 0f;
			_listTop = 0f;
			
			_selectedLevel.GetComponent<PuzzleScale>().PuzzleInit += OnPuzzleInit;
			_selectedLevel.GetComponent<PuzzleState>().BoardEnabled = true;
			
			var puzzleScale = _selectedLevel.GetComponent<PuzzleScale>();
			CameraScript.FitToDimensions(puzzleScale.Dimensions, puzzleScale.Margin);
			
			// TODO: make configurable
			const float time = 0.5f;
			LeanTween.moveLocal(gameObject, Vector3.zero, time)
				.setEase(LeanTweenType.easeInOutSine);
			LeanTween.moveLocal(_selectedLevel, puzzleScale.Offset, time)
				.setEase(LeanTweenType.easeInOutSine);
		}

		private void GenerateLevelsList()
		{
			var puzzleScale = _selectedLevel.GetComponent<PuzzleScale>();
			var puzzleState = _selectedLevel.GetComponent<PuzzleState>();
			
			// Keep track of the last board's position as the offset for the next board
			// TODO: make configurable
			const float margin = 2f;
			
			var prevOffset = puzzleScale.Dimensions.y / 2f + margin;

			_levels[puzzleState.CurrentLevel] = _selectedLevel;
			_levelBounds[puzzleState.CurrentLevel] = new Tuple<float, float>(
				-prevOffset, prevOffset
			);
			
			for (var level = puzzleState.CurrentLevel + 1; level < Levels.LevelCount; level++) {
				GenerateLevel(level, margin, ref prevOffset, Vector2.down);
			}

			_listTop = prevOffset;
			
			prevOffset = puzzleScale.Dimensions.y / 2f + margin;
			
			for (var level = puzzleState.CurrentLevel - 1; level >= 0; level--) {
				GenerateLevel(level, margin, ref prevOffset, Vector2.up);
			}

			_listBottom = -prevOffset;
		}

		private void GenerateLevel(int level, float margin, ref float prevOffset, Vector2 direction)
		{
			var puzzleGame = Instantiate(PuzzleGamePrefab);
			puzzleGame.name = $"PuzzleGame ({level})";
			puzzleGame.transform.SetParent(transform);
				
			_levels[level] = puzzleGame;

			puzzleGame.GetComponent<PuzzleState>().BoardEnabled = false;
			
			puzzleGame.GetComponent<BoardInput>().enabled = false;

			// TODO: get board dimensions from puzzle scale before it is fully initialized
			var puzzleScale = puzzleGame.GetComponent<PuzzleScale>();
			var boardHeight = Levels.BuildLevel(level).Size.y * puzzleScale.Scaling / 2f;

			var sign = Mathf.Sign(direction.y);
			var boardStartBounds = sign * prevOffset;
			prevOffset += boardHeight + margin;

			// TODO: make configurable
			const float animationSpeed = 0.6f;
			const float delayScale = 0f;
				
			puzzleGame.GetComponent<PuzzleState>().Init(level, direction * prevOffset, animationSpeed, delayScale);
			
			// Add half the board height as the starting point for the next board to spawn
			prevOffset += boardHeight + margin;
			var boardEndBounds = sign * prevOffset;
			
			_levelBounds[level] = new Tuple<float, float>(boardStartBounds, boardEndBounds);
		}

		private void OnPuzzleInit()
		{
			if (_scrollEnabled) {
				return;
			}
			
			var puzzleScale = _selectedLevel.GetComponent<PuzzleScale>();
			CameraScript.FitToDimensions(puzzleScale.Dimensions, puzzleScale.Margin);
		}

		private void OnPan(TKPanRecognizer recognizer)
		{
			if (!_scrollEnabled) {
				return;
			}

			_velocity = Vector2.zero;
			
			// TODO: make configurable
			const float scalingFactor = 50f;
			transform.position += Vector3.up * recognizer.deltaTranslation.y / scalingFactor;
		}

		private void OnPanComplete(TKPanRecognizer recognizer)
		{
			if (!_scrollEnabled) {
				return;
			}
			
			// TODO: make configurable
			var delta = recognizer.deltaTranslation.y;
			var velocityMagnitude = Mathf.Abs(delta) < 5f ? 0f : Mathf.Clamp(delta, -100f, 100f);
			const float scalingFactor = 50f;
			
			_velocity = Vector3.up * velocityMagnitude / scalingFactor;
		}

		private int FindLevel(float yPos)
		{
			var current = _selectedLevel.GetComponent<PuzzleState>().CurrentLevel;

			// TODO: this is linear search, can be binary search
			var bounds = _levelBounds[current];
			if (yPos < bounds.Item1) {
				for (var level = current + 1; level < Levels.LevelCount; level++) {
					bounds = _levelBounds[level];
					if (yPos >= bounds.Item2) {
						return level;
					}
				}
			} else if (yPos > bounds.Item2) {
				for (var level = current - 1; level >= 0; level--) {
					bounds = _levelBounds[level];
					if (yPos <= bounds.Item2) {
						return level;
					}
				}
			}

			return current;
		}
	}
}
