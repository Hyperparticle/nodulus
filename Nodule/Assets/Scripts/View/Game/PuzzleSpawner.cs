using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Game;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    public class PuzzleSpawner : MonoBehaviour {
    
        public NodeView  NodeScript;
        public ArcView   ArcScript;
        public FieldView FieldScript;

        private readonly IDictionary<Point, NodeView> _nodeMap = new Dictionary<Point, NodeView>();
        private readonly ICollection<ArcView> _arcSet = new HashSet<ArcView>();
        private readonly ICollection<FieldView> _fieldSet = new HashSet<FieldView>();

        private GameBoard _gameBoard;

        public IDictionary<Point, NodeView> NodeMap { get { return _nodeMap; } }

        public Puzzle SpawnBoard(int level)
        {
            _gameBoard = Level.BuildLevel(level);

            InstantiateNodes();
            InstantiateFields();
            InstantiateArcs();

            return new Puzzle(_gameBoard);
        }

        public void DestroyBoard()
        {
            if (_gameBoard == null) return;

            // Destroy all objects in the game board
            foreach (var node in _nodeMap.Values) { Destroy(node.gameObject); }
            foreach (var arc in _arcSet) { Destroy(arc.gameObject); }
            foreach (var field in _fieldSet) { Destroy(field.gameObject); }

            _nodeMap.Clear();
            _arcSet.Clear();
            _fieldSet.Clear();
        
            _gameBoard = null;
        }

        private void InstantiateNodes()
        {
            var i = 0;
            foreach (var node in _gameBoard.Nodes)
            {
                var nodeView = Instantiate(NodeScript);

                // Set the node's parent as this puzzle
                nodeView.transform.SetParent(transform);
                nodeView.Init(node, _gameBoard.StartIsland.Contains(node));
                nodeView.name = "Node " + i++;
                _nodeMap.Add(node.Position, nodeView);
            }
        }

        private void InstantiateFields()
        {
            var i = 0;
            foreach (var field in _gameBoard.Fields)
            {
                var fieldView = Instantiate(FieldScript);

                // Find the node at the field's position and set it as a parent of this field
                fieldView.transform.SetParent(_nodeMap[field.Position].transform);
                fieldView.Init(field, _nodeMap[field.Position], _nodeMap[field.ConnectedNode.Position]);
                fieldView.name = "Field " + i++;
                _fieldSet.Add(fieldView);
            }
        }

        private void InstantiateArcs()
        {
            var i = 0;
            foreach (var arc in _gameBoard.Arcs)
            {
                var arcView = Instantiate(ArcScript);

                // Find the node at the arc's position and set it as a perent of this arc
                arcView.transform.SetParent(_nodeMap[arc.Position].transform);
                arcView.Init(arc, _gameBoard.StartIsland.Contains(arc.ParentNode));
                arcView.name = "Arc " + i++;
                _arcSet.Add(arcView);
            }
        }
    }
}
