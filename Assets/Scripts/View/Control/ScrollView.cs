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

		private bool _scrollEnabled;

		private int _selectedLevel;
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

		private MenuRotator _menuRotator;
		private GameAudio _gameAudio;

		private void Awake()
		{
            // Set the maximum number of simultaneous tweens
            LeanTween.init(30000);

			_menuRotator = GameObject.FindGameObjectWithTag("ButtonSelect").GetComponent<MenuRotator>();
			_gameAudio = GameObject.FindGameObjectWithTag("GameAudio").GetComponent<GameAudio>();
		}

		private void Start()
		{
			var panRecognizer = new TKPanRecognizer();
			panRecognizer.gestureRecognizedEvent += OnPan;
			panRecognizer.gestureCompleteEvent += OnPanComplete;
			TouchKit.addGestureRecognizer(panRecognizer);
			
			var tapRecognizer = new TKTapRecognizer();
			tapRecognizer.gestureRecognizedEvent += OnTap;
			TouchKit.addGestureRecognizer(tapRecognizer);
			
			// Start with the initially defined start level
			_selectedLevel = StartLevel;
			
			GenerateLevelsList();

			if (_levels[_selectedLevel] == null) {
				Debug.LogError($"Failed to start at level ({_selectedLevel})");
				return;
			}
			
			var puzzleState = _levels[_selectedLevel].GetComponent<PuzzleState>();
			var puzzleScale = _levels[_selectedLevel].GetComponent<PuzzleScale>();
			
			puzzleState.BoardEnabled = true;
			puzzleScale.PuzzleInit += OnPuzzleInit;
			_levels[_selectedLevel].GetComponent<BoardAction>().PuzzleWin += OnPuzzleWin;
			puzzleState.LevelStateChanged += OnLevelStateChanged;
			
			var bounds = _levelBounds[_selectedLevel];
			var mid = (bounds.Item1 + bounds.Item2) / 2f;

			transform.position = Vector3.up * mid;
			
			puzzleState.Init(_selectedLevel);
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
			var closestLevel = FindLevel(transform.position.y);
			
			InterpolateCameraZoom(_selectedLevel);
			Magnetize(_selectedLevel);
			
			if (closestLevel == _selectedLevel) {
				return;
			}
			
			_selectedLevel = closestLevel;
			
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
			var yPos = transform.position.y;
			var delta = -mid + yPos;
			var closestNextLevel = delta < 0
				? (currentLevel <= 0 ? 0 : currentLevel - 1)
				: (currentLevel >= _levelBounds.Length - 1 ? _levelBounds.Length - 1 : currentLevel + 1);
			var nextBounds = _levelBounds[closestNextLevel];
			var nextMid = (nextBounds.Item1 + nextBounds.Item2) / 2f;
			var deltaRatio = Mathf.Abs(delta) > Mathf.Abs(nextMid - mid) ? 1f : Mathf.Abs(delta) / Mathf.Abs(nextMid - mid);
			
			var puzzleScale = _levels[_selectedLevel].GetComponent<PuzzleScale>();
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
			
			var delta = -mid + transform.position.y;

			_magnetVelocity = Vector3.down * delta * magnetScale;
		}

		private void RevealLevels(int currentLevel)
		{
			// Instantiate adjacent levels
			var prevLevel = currentLevel <= 0 ? 0 : currentLevel - 1;
			var nextLevel = currentLevel >= _levels.Length - 1 ? _levels.Length - 1 : currentLevel + 1;

			var prev = _levels[prevLevel].GetComponent<PuzzleState>();
			var next = _levels[nextLevel].GetComponent<PuzzleState>();

			prev.BoardEnabled = false;
			next.BoardEnabled = false;
			
			prev.InitSaved();
			next.InitSaved();
		}

		public void EnableScroll()
		{
			if (_scrollEnabled) {
				_scrollEnabled = false;
				DisableScroll();
				return;
			}
			
			_menuRotator.Toggle();
			
			var puzzleState = _levels[_selectedLevel].GetComponent<PuzzleState>();
			var puzzleScale = puzzleState.GetComponent<PuzzleScale>();
			var boardAction = puzzleState.GetComponent<BoardAction>();
			
			puzzleState.BoardEnabled = false;
			
			RevealLevels(_selectedLevel);

			puzzleScale.PuzzleInit -= OnPuzzleInit;
			boardAction.PuzzleWin -= OnPuzzleWin;
			puzzleState.LevelStateChanged -= OnLevelStateChanged;
			
			var zoom = CameraScript.CameraZoomToFit(puzzleScale.Dimensions, puzzleScale.Margin, _scaleRatio);
			_cameraZoomId = CameraScript.ZoomCamera(zoom, CameraZoomTime, LeanTweenType.easeInSine);

			_panVelocity = Vector3.up / VelocityScalingFactor;
			_scrollEnabled = true;
			
			// TODO: make configurable
			const float volume = 0.3f;
			_gameAudio.Play(GameClip.MenuSelect, volume: volume);
		}

		public void DisableScroll(float delay = 0f)
		{
			_menuRotator.Toggle();
			
			var level = _levels[_selectedLevel];
			
			var prevLevel = _selectedLevel - 1;
			var nextLevel = _selectedLevel + 1;
			
			_levels.ElementAtOrDefault(prevLevel)?.GetComponent<PuzzleState>()?.DestroyBoard(false);
			_levels.ElementAtOrDefault(nextLevel)?.GetComponent<PuzzleState>()?.DestroyBoard(false);
			
			level.GetComponent<PuzzleScale>().PuzzleInit += OnPuzzleInit;
			level.GetComponent<BoardAction>().PuzzleWin += OnPuzzleWin;
			level.GetComponent<PuzzleState>().LevelStateChanged += OnLevelStateChanged;
			level.GetComponent<PuzzleState>().BoardEnabled = true;
			level.GetComponent<PuzzleView>().ResumeView();
			
			var puzzleScale = _levels[_selectedLevel].GetComponent<PuzzleScale>();
			CameraScript.FitToDimensions(
				puzzleScale.Dimensions, 
				puzzleScale.Margin, 
				CameraZoomTime,
				LeanTweenType.easeInSine
			);
			
			var bounds = _levelBounds[_selectedLevel];
			var mid = (bounds.Item1 + bounds.Item2) / 2f;
			
			// TODO: make configurable
			const float time = 0.5f;
			LeanTween.moveLocal(gameObject, Vector3.up * mid, time)
				.setEase(LeanTweenType.easeInOutSine)
				.setDelay(delay);
		}

		public void RestartLevel()
		{
			if (_scrollEnabled) {
				return;
			}
			
			_levels[_selectedLevel].GetComponent<PuzzleState>().RestartLevel();
		}

		private void GenerateLevelsList()
		{
			_levelBounds = new Tuple<float, float>[Levels.LevelCount];
			_levels = new GameObject[Levels.LevelCount];
			
			// TODO: make configurable
			const float margin = 1.5f;

			_listBottom = margin;
			
			// Keep track of the last board's position as the offset for the next board
			var prevOffset = 0f;

			// Generate all levels at the appropriate offsets
			for (var level = 0; level < Levels.LevelCount; level++) {
				GenerateLevel(level, margin, ref prevOffset);
			}
			
			_listTop = prevOffset - margin;
		}

		private void GenerateLevel(int level, float margin, ref float prevOffset)
		{
			var puzzleGame = Instantiate(PuzzleGamePrefab);
			puzzleGame.name = $"PuzzleGame ({level})";
			puzzleGame.transform.SetParent(transform);

			// TODO: get board dimensions from puzzle scale before it is fully initialized
			var puzzleScale = puzzleGame.GetComponent<PuzzleScale>();

			var board = Levels.BuildLevel(level);

			if (board == null) {
				Debug.LogWarning($"Failed to create level ({level})");
				return;
			}
			
			var boardSize = (Vector2) board.Size * puzzleScale.Scaling / 2f;;
			
			_levels[level] = puzzleGame;

			puzzleGame.GetComponent<PuzzleState>().BoardEnabled = false;
			
			puzzleGame.GetComponent<BoardInput>().enabled = false;

			var boardHeight = boardSize.y * puzzleScale.Scaling / 2f;

			var boardStartBounds = prevOffset;
			prevOffset += boardHeight + margin;

			// TODO: make configurable
			const float animationSpeed = 0.4f;
			const float delayScale = 0f;
			
			puzzleGame.transform.localPosition += Vector3.down * prevOffset;
			puzzleGame.GetComponent<PuzzleState>().Save(level, animationSpeed, delayScale);
			
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
			
			var puzzleScale = _levels[_selectedLevel].GetComponent<PuzzleScale>();
			CameraScript.FitToDimensions(puzzleScale.Dimensions, puzzleScale.Margin);
		}

		private void OnPuzzleWin(int level)
		{
			_levels[_selectedLevel].GetComponent<PuzzleState>().BoardEnabled = false;
			_levels[_selectedLevel].GetComponent<GameBoardAudio>().enabled = true;
			_levels[_selectedLevel].GetComponent<PuzzleScale>().PuzzleInit -= OnPuzzleInit;
			_levels[_selectedLevel].GetComponent<BoardAction>().PuzzleWin -= OnPuzzleWin;
			_levels[_selectedLevel].GetComponent<PuzzleState>().LevelStateChanged -= OnLevelStateChanged;
			
			_selectedLevel = level >= _levels.Length - 1 ? _levels.Length - 1 : level + 1;
			
			var puzzleState = _levels[_selectedLevel].GetComponent<PuzzleState>();
			
			_levels[_selectedLevel].GetComponent<PuzzleState>().BoardEnabled = false;
			_levels[_selectedLevel].GetComponent<PuzzleScale>().PuzzleInit += OnPuzzleInit;
			_levels[_selectedLevel].GetComponent<BoardAction>().PuzzleWin += OnPuzzleWin;
			_levels[_selectedLevel].GetComponent<PuzzleState>().LevelStateChanged += OnLevelStateChanged;
			
			// Move to the new board
			var bounds = _levelBounds[_selectedLevel];
			var mid = (bounds.Item1 + bounds.Item2) / 2f;
			
			// TODO: make configurable
			const float time = 0.5f;
			const float moveDelay = 1f;
			LeanTween.moveLocal(gameObject, Vector3.up * mid, time)
				.setEase(LeanTweenType.easeInOutSine)
				.setDelay(moveDelay);
			
			// Initialize the new board
			// TODO: make configurable
			const float initDelay = 0.5f;
			LeanTween.delayedCall(initDelay, () => {
				_levels[_selectedLevel].GetComponent<PuzzleState>().BoardEnabled = true;
				
				puzzleState.Init(_selectedLevel);
				
				_levels[_selectedLevel].GetComponent<PuzzleView>().ResumeView();
			});
			
			if (_selectedLevel < _levels.Length - 1) {
				_levels[_selectedLevel + 1].GetComponent<PuzzleState>().DestroyBoard(false);
			}
			
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
			
			var clampedPos = Mathf.Clamp(transform.position.y, _listBottom, _listTop);
			transform.position = new Vector2(transform.position.x, clampedPos);
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

		private void OnLevelStateChanged(Level level)
		{
			Debug.Log(level.Moves);
		}

		private int FindLevel(float yPos)
		{
			// TODO: this is linear search, can be binary search
			var bounds = _levelBounds[_selectedLevel];
			if (yPos < bounds.Item1) {
				for (var level = _selectedLevel - 1; level >= 0; level--) {
					bounds = _levelBounds[level];
					if (yPos <= bounds.Item2) {
						return level;
					}
				}
			} else if (yPos > bounds.Item2) {
				for (var level = _selectedLevel + 1; level < Levels.LevelCount; level++) {
					bounds = _levelBounds[level];
					if (yPos >= bounds.Item1) {
						return level;
					}
				}
				
			}

			return _selectedLevel;
		}

		public void ShowSettings()
		{
			
		}

		private void OnTap(TKTapRecognizer recognizer)
		{
			if (!_scrollEnabled) {
				return;
			}

			var touch = (Vector2) Camera.main.ScreenToViewportPoint(recognizer.touchLocation()) - Vector2.one / 2f;
			// TODO: make configurable
			const float maxTouchDistance = 0.25f;
			
			if (touch.magnitude > maxTouchDistance) {
				return;
			}

			var velocity = (_panVelocity + _magnetVelocity).magnitude;
			
			// TODO: make configurable
			const float threshold = 0.20f;
			
			if (velocity > threshold) {
				return;
			}
			
			EnableScroll();
		}
	}
}
