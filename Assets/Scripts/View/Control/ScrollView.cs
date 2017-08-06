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
		private bool _isPanning;
		private Vector3 _panVelocity;
		private Vector3 _magnetVelocity;

		// TODO: make configurable
		private const float CameraZoomTime = 1f;
		private readonly Vector3 _scaleRatio = new Vector3(0.9f, 0.5f);
		private const float VelocityScalingFactor = 50f;

		private int _cameraZoomId;

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

		private void FixedUpdate()
		{
			if (!_scrollEnabled) {
				return;
			}
			
			// TODO: make configurable
			const float damping = 0.98f;
			
			transform.Translate(_panVelocity + _magnetVelocity);
			_panVelocity *= damping;

			var clampedPos = Mathf.Clamp(transform.position.y, _listBottom, _listTop);
			transform.position = new Vector2(transform.position.x, clampedPos);
		}

		private void Update()
		{
			if (!_scrollEnabled) {
				return;
			}

			// Find the nearest level and select it
			var closestLevel = FindLevel(-transform.position.y);
			var currentLevel = _selectedLevel.GetComponent<PuzzleState>().CurrentLevel;
			
			InterpolateCameraZoom(currentLevel);
			Magnetize(currentLevel);
			
			if (closestLevel == currentLevel) {
				return;
			}
			
			_selectedLevel = _levels[closestLevel];
			
			RevealLevels(closestLevel);
		}

		private void InterpolateCameraZoom(int currentLevel)
		{
			if (LeanTween.isTweening(_cameraZoomId)) {
				return;
			}
			
			var bounds = _levelBounds[currentLevel];
			var mid = (bounds.Item1 + bounds.Item2) / 2f;
			
			// Interpolate camera zoom between levels
			var delta = mid + transform.position.y;
			var closestNextLevel = delta < 0
				? (currentLevel <= 0 ? 0 : currentLevel - 1)
				: (currentLevel >= _levelBounds.Length - 1 ? _levelBounds.Length - 1 : currentLevel + 1);
			var nextBounds = _levelBounds[closestNextLevel];
			var nextMid = (nextBounds.Item1 + nextBounds.Item2) / 2f;
			var deltaRatio = Mathf.Abs(delta) > Mathf.Abs(nextMid - mid) ? 1f : Mathf.Abs(delta) / Mathf.Abs(nextMid - mid);
			
			var puzzleScale = _selectedLevel.GetComponent<PuzzleScale>();
			var selectedLevelZoom = CameraScript.CameraZoomToFit(puzzleScale.Dimensions, puzzleScale.Margin, _scaleRatio);

			var nextPuzzleScale = _levels[closestNextLevel].GetComponent<PuzzleScale>();
			var nextLevelZoom = CameraScript.CameraZoomToFit(nextPuzzleScale.Dimensions, nextPuzzleScale.Margin, _scaleRatio);
			
			Camera.main.orthographicSize = LeanTween.easeInOutSine(selectedLevelZoom, nextLevelZoom, deltaRatio);
		}

		private void Magnetize(int currentLevel)
		{
			// TODO: make configurable
			const float velocityThreshold = 0.3f;
			const float magnetScale = 0.05f;
			
			if (_isPanning || _panVelocity.magnitude > velocityThreshold) {
				return;
			}
			
			var bounds = _levelBounds[currentLevel];
			var mid = (bounds.Item1 + bounds.Item2) / 2f;
			
			var delta = mid + transform.position.y;

			_magnetVelocity = Vector3.down * delta * magnetScale;
		}

		private void RevealLevels(int currentLevel)
		{
			// Instantiate adjacent levels
			var prevLevel = currentLevel <= 0 ? 0 : currentLevel - 1;
			var nextLevel = currentLevel >= _levels.Length - 1 ? _levels.Length - 1 : currentLevel + 1;
			
			_levels[prevLevel].GetComponent<PuzzleState>().InitSaved();
			_levels[nextLevel].GetComponent<PuzzleState>().InitSaved();
		}

		public void EnableScroll()
		{
			if (_scrollEnabled) {
				_scrollEnabled = false;
				DisableScroll();
				return;
			}
			
			var puzzleState = _selectedLevel.GetComponent<PuzzleState>();
			
			puzzleState.BoardEnabled = false;
			
			_levelBounds = new Tuple<float, float>[Levels.LevelCount];
			_levels = new GameObject[Levels.LevelCount];
			
			GenerateLevelsList();
			
			RevealLevels(puzzleState.CurrentLevel);
			
			var puzzleScale = _selectedLevel.GetComponent<PuzzleScale>();
			var zoom = CameraScript.CameraZoomToFit(puzzleScale.Dimensions, puzzleScale.Margin, _scaleRatio);
			_cameraZoomId = CameraScript.ZoomCamera(zoom, CameraZoomTime, LeanTweenType.easeInSine);

			_panVelocity = Vector3.up / VelocityScalingFactor;
			_scrollEnabled = true;
		}

		public void DisableScroll()
		{
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
			_selectedLevel.GetComponent<PuzzleView>().ResumeView();
			
			var puzzleScale = _selectedLevel.GetComponent<PuzzleScale>();
			CameraScript.FitToDimensions(
				puzzleScale.Dimensions, 
				puzzleScale.Margin, 
				CameraZoomTime,
				LeanTweenType.easeInSine
			);
			
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
			
			_levels[puzzleState.CurrentLevel] = _selectedLevel;
			
			// Keep track of the last board's position as the offset for the next board
			// TODO: make configurable
			const float margin = 1.5f;
			
			var prevOffset = _listTop = -margin;
			
			for (var level = Levels.LevelCount - 1; level >= puzzleState.CurrentLevel + 1; level--) {
				GenerateLevel(level, margin, ref prevOffset);
			}

			var boardHeight = puzzleScale.Dimensions.y / 2f;

			var boardStartBounds = prevOffset;
			prevOffset += boardHeight + margin;
			
			transform.Translate(Vector2.down * prevOffset);
			_selectedLevel.transform.position += Vector3.up * prevOffset;

			prevOffset += boardHeight + margin;
			var boardEndBounds = prevOffset;
			
			_levelBounds[puzzleState.CurrentLevel] = new Tuple<float, float>(boardStartBounds, boardEndBounds);
			
			for (var level = puzzleState.CurrentLevel - 1; level >= 0; level--) {
				GenerateLevel(level, margin, ref prevOffset);
			}

			_listBottom = -prevOffset + margin;
		}

		private void GenerateLevel(int level, float margin, ref float prevOffset)
		{
			var puzzleGame = Instantiate(PuzzleGamePrefab);
			puzzleGame.name = $"PuzzleGame ({level})";
			puzzleGame.transform.SetParent(transform);

			// TODO: get board dimensions from puzzle scale before it is fully initialized
			var puzzleScale = puzzleGame.GetComponent<PuzzleScale>();
			var boardSize = (Vector2) Levels.BuildLevel(level).Size * puzzleScale.Scaling / 2f;;
			puzzleGame.transform.localPosition = Vector3.left * boardSize.x;
			
			_levels[level] = puzzleGame;

			puzzleGame.GetComponent<PuzzleState>().BoardEnabled = false;
			
			puzzleGame.GetComponent<BoardInput>().enabled = false;

			var boardHeight = boardSize.y * puzzleScale.Scaling / 2f;

			var boardStartBounds = prevOffset;
			prevOffset += boardHeight + margin;

			// TODO: make configurable
			const float animationSpeed = 0.4f;
			const float delayScale = 0f;
				
			puzzleGame.GetComponent<PuzzleState>().Save(level, Vector2.up * prevOffset, animationSpeed, delayScale);
			
			// Add half the board height as the starting point for the next board to spawn
			prevOffset += boardHeight + margin;
			var boardEndBounds = prevOffset;
			
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

			_isPanning = true;

			_panVelocity = Vector2.zero;
			_magnetVelocity = Vector2.zero;
			
			// TODO: make configurable
			const float scalingFactor = 50f;
			transform.Translate(Vector3.up * recognizer.deltaTranslation.y / scalingFactor); 
		}

		private void OnPanComplete(TKPanRecognizer recognizer)
		{
			if (!_scrollEnabled) {
				return;
			}

			_isPanning = false;
			
			// TODO: make configurable
			var delta = recognizer.deltaTranslation.y;
			var velocityMagnitude = Mathf.Abs(delta) < 5f ? 0f : Mathf.Clamp(delta, -50f, 50f);
			
			_panVelocity = Vector3.up * velocityMagnitude / VelocityScalingFactor;
		}

		private int FindLevel(float yPos)
		{
			var current = _selectedLevel.GetComponent<PuzzleState>().CurrentLevel;

			// TODO: this is linear search, can be binary search
			var bounds = _levelBounds[current];
			if (yPos < bounds.Item1) {
				for (var level = current + 1; level < Levels.LevelCount; level++) {
					bounds = _levelBounds[level];
					if (yPos >= bounds.Item1) {
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
