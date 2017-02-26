using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Game;
using Assets.Scripts.View.Data;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    public class PuzzleSpawner : MonoBehaviour
    {
        public NodeView NodeScript;
        public ArcView ArcScript;
        public FieldView FieldScript;

        private readonly IDictionary<Point, NodeView> _nodeMap = new Dictionary<Point, NodeView>();
        private readonly FieldViewMap _fieldMap = new FieldViewMap();
        private readonly ArcViewMap _arcMap = new ArcViewMap();

        private GameBoard _gameBoard;

        public IDictionary<Point, NodeView> NodeMap
        {
            get { return _nodeMap; }
        }

        public FieldViewMap FieldMap
        {
            get { return _fieldMap; }
        }

        public ArcViewMap ArcMap
        {
            get { return _arcMap; }
        }

        public Puzzle SpawnBoard(int level)
        {
            // Create the game board model
            _gameBoard = Levels.BuildLevel(level);

            if (_gameBoard == null) {
                Debug.LogError(string.Format("The game board for level {0} is in an invalid format", level));
            }

            // Instantiate all the necessary components to view the board
            InstantiateNodes();
            InstantiateFields();
            InstantiateArcs();

            StartAnimations();

            // Wrap a puzzle around the gameboard and return it
            return new Puzzle(_gameBoard);
        }

        public void DestroyBoard()
        {
            if (_gameBoard == null) return;

            // Destroy all objects in the game board
            var i = 0;
            foreach (var node in _nodeMap.Values) {
                node.WaveOut(i++);
            }

            foreach (Transform child in transform) {
                Destroy(child.gameObject, 1.5f);
            }

            _nodeMap.Clear();
            _arcMap.Clear();
            _fieldMap.Clear();

            _gameBoard = null;
        }

        private void InstantiateNodes()
        {
            var i = 0;
            foreach (var node in _gameBoard.Nodes) {
                var nodeView = Instantiate(NodeScript);

                // Set the node's parent as this puzzle
                nodeView.transform.SetParent(transform);
                nodeView.Init(node, _gameBoard.StartIsland.Contains(node), i);
                nodeView.name = "Node " + i++;
                _nodeMap.Add(node.Position, nodeView);
            }
        }

        private void InstantiateFields()
        {
            var i = 0;
            foreach (var field in _gameBoard.Fields) {
                var fieldView = Instantiate(FieldScript);

                // Find the node at the field's position and set it as a parent of this field
                fieldView.transform.SetParent(_nodeMap[field.Position].transform);
                fieldView.Init(field, _nodeMap[field.Position], _nodeMap[field.ConnectedPosition]);
                fieldView.name = "Field " + i++;

                // Keep track of the field in grid space
                // Since fields are undirected, we should add the opposite direction as well
                _fieldMap.Add(field.Position, field.Direction, fieldView);
                _fieldMap.Add(field.ConnectedPosition, field.Direction.Opposite(), fieldView);
            }
        }

        private void InstantiateArcs()
        {
            var i = 0;
            foreach (var arc in _gameBoard.Arcs) {
                var arcView = Instantiate(ArcScript);

                // Find the node at the arc's position and set it as a perent of this arc
                var parent = _nodeMap[arc.Position].transform;
                arcView.transform.SetParent(parent);
                arcView.Init(arc, parent, _gameBoard.StartIsland.Contains(arc.ParentNode));
                arcView.name = "Arc " + i++;

                // Keep track of the arc in grid space
                // Since arcs are undirected, we should add the opposite direction as well
                _arcMap.Add(arc.Position, arc.Direction, arcView);
                _arcMap.Add(arc.ConnectedPosition, arc.Direction.Opposite(), arcView);
            }
        }

        private void StartAnimations()
        {
            var i = 0;
            foreach (var nodeView in _nodeMap.Values) {
                nodeView.WaveIn(i++);
            }
        }
    }
}
