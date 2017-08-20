using System.Collections.Generic;
using System.Linq;
using Core.Data;
using Core.Game;
using UnityEngine;
using View.Control;
using View.Data;
using View.Items;

namespace View.Game
{
    public class PuzzleSpawner : MonoBehaviour
    {
        private GameBoard _gameBoard;

        private LatticeView _lattice;
        private PuzzleScale _puzzleScale;
        private ItemPool _itemPool;
        private GameAudio _gameAudio;

        public IDictionary<Point, NodeView> NodeMap { get; } = new Dictionary<Point, NodeView>();

        public FieldViewMap FieldMap { get; } = new FieldViewMap();

        public ArcViewMap ArcMap { get; } = new ArcViewMap();

        public bool FinishedSpawn { get; private set; }

        public int LevelCount => Levels.LevelCount;

        private void Awake()
        {
            _lattice = GetComponentInChildren<LatticeView>();
            _puzzleScale = GetComponent<PuzzleScale>();
            _itemPool = GetComponent<ItemPool>();
            _gameAudio = GameObject.FindGameObjectWithTag("GameAudio").GetComponent<GameAudio>();
        }

        public Puzzle SpawnBoard(int level, float animationSpeed = 1f, float delayScale = 1f)
        {
            // Create the game board model
            var newGameBoard = Levels.BuildLevel(level);

            if (newGameBoard != null) {
                _gameBoard = newGameBoard;
            } else {
                Debug.LogError($"The game board for level {level} is in an invalid format");
            }

            // Instantiate all the necessary components to view the board
            FinishedSpawn = false;
            InstantiateNodes();
            InstantiateFields();
            InstantiateArcs();

            StartAnimations(animationSpeed, delayScale);

            // Wrap a puzzle around the gameboard and return it
            return new Puzzle(_gameBoard);
        }

        public void DestroyBoard(bool playSound = true)
        {
            if (_gameBoard == null) return;
            
            // Destroy the lattice grid
            _lattice.DestroyGrid();

            // Destroy all objects in the game board
            var i = 0;
            foreach (var node in NodeMap.Values) {
                node.WaveOut(i++, NodeMap.Count, playSound: playSound);
            }

            foreach (Transform child in transform) {
                var node = child.gameObject.GetComponent<NodeView>();
                var arc = child.gameObject.GetComponent<ArcView>();
                var field = child.gameObject.GetComponent<FieldView>();

                // Only destroy board pieces
                if (node != null) {
                    _itemPool.Release(node, 3f);
                } else if (arc != null) {
                    _itemPool.Release(arc, 3f);
                } else if (field != null) {
                    _itemPool.Release(field, 3f);
                }
            }

            NodeMap.Clear();
            ArcMap.Clear();
            FieldMap.Clear();

            _gameBoard = null;
        }

        private void InstantiateNodes()
        {
            var i = 0;
            foreach (var node in _gameBoard.Nodes) {
                var nodeView = _itemPool.GetNode();

                // Set the node's parent as this puzzle
                nodeView.transform.SetParent(transform);
                nodeView.Init(node, _gameBoard.StartIsland.Contains(node), i);
                nodeView.name = "Node " + i++;
                NodeMap.Add(node.Position, nodeView);
            }
        }

        private void InstantiateFields()
        {
            var i = 0;
            foreach (var field in _gameBoard.Fields) {
                var fieldView = _itemPool.GetField();

                // Find the node at the field's position and set it as a parent of this field
                fieldView.transform.SetParent(NodeMap[field.Position].transform);
                fieldView.Init(field, NodeMap[field.Position], NodeMap[field.ConnectedPosition]);
                fieldView.name = "Field " + i++;

                // Keep track of the field in grid space
                // Since fields are undirected, we should add the opposite direction as well
                FieldMap.Add(field.Position, field.Direction, fieldView);
                FieldMap.Add(field.ConnectedPosition, field.Direction.Opposite(), fieldView);
            }
        }

        private void InstantiateArcs()
        {
            var i = 0;
            foreach (var arc in _gameBoard.Arcs) {
                var arcView = _itemPool.GetArc();

                // Find the node at the arc's position and set it as a perent of this arc
                var parent = NodeMap[arc.Position].transform;
                arcView.transform.SetParent(parent);
                arcView.Init(arc, parent, _gameBoard.StartIsland.Contains(arc.ParentNode));
                arcView.name = "Arc " + i++;

                // Keep track of the arc in grid space
                // Since arcs are undirected, we should add the opposite direction as well
                ArcMap.Add(arc.Position, arc.Direction, arcView);
                ArcMap.Add(arc.ConnectedPosition, arc.Direction.Opposite(), arcView);
            }
        }

        private void StartAnimations(float animationSpeed = 1f, float delayScale = 1f)
        {
            // TODO: make configurable
            const float delay = 1.1f;
            const float volume = 0.3f;
            _gameAudio.Play(GameClip.GameStart, delay * delayScale, volume);
            
            _lattice.Init(
                _gameBoard.Size.y + 1,
                _gameBoard.Size.x + 1,
                _puzzleScale.Scaling,
                animationSpeed,
                delayScale
            );
            
            var i = 0;
            foreach (var nodeView in NodeMap.Values) {
                if (i < NodeMap.Count - 1) {
                    nodeView.WaveIn(i++, NodeMap.Count, animationSpeed: animationSpeed, delayScale: delayScale);
                } else {
                    // On completion of the last node, the puzzle has finished spawning
                    nodeView.WaveIn(i++, NodeMap.Count, () => FinishedSpawn = true, animationSpeed, delayScale);
                }
            }
        }
    }
}
