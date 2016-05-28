using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Game;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    public class PuzzleSpawner : MonoBehaviour {
    
        public float DestroyDelay = 0.3f;

        public NodeView  NodeScript;
        public ArcView   ArcScript;
        public FieldView FieldScript;

        private readonly IDictionary<Point, NodeView> _nodeMap = new Dictionary<Point, NodeView>();
        private readonly IDictionary<Point, ArcView> _arcMap = new Dictionary<Point, ArcView>();
        private readonly IDictionary<Point, FieldView> _fieldMap = new Dictionary<Point, FieldView>();

        private Puzzle _puzzle;

        public Puzzle SpawnBoard(int level)
        {
            _puzzle = Level.BuildLevel(level);
        
            InstantiateNodes();
            InstantiateFields();
            InstantiateArcs();
        
            return _puzzle;
        }

        public void DestroyBoard()
        {
            if (_puzzle == null) return;

            // Destroy all objects in the game board
            foreach (var node in _nodeMap.Values) { Destroy(node.gameObject, DestroyDelay); }
            foreach (var edge in _arcMap.Values) { Destroy(edge.gameObject, DestroyDelay); }
            foreach (var field in _fieldMap.Values) { Destroy(field.gameObject, DestroyDelay); }

            _nodeMap.Clear();
            _arcMap.Clear();
            _fieldMap.Clear();
        
            _puzzle = null;
        }

        private void InstantiateNodes()
        {
            var i = 0;
            foreach (var node in _puzzle.Nodes)
            {
                var nodeView = Instantiate(NodeScript);

                // Set the node's parent as this puzzle
                nodeView.transform.SetParent(transform);
                nodeView.Init(node);
                nodeView.name = "Node " + i++;
                _nodeMap.Add(node.Position, nodeView);
            }
        }

        private void InstantiateFields()
        {
            var i = 0;
            foreach (var field in _puzzle.Fields)
            {
                var fieldView = Instantiate(FieldScript);

                // Find the node at the field's position and set it as a parent of this field
                fieldView.transform.SetParent(_nodeMap[field.Position].transform);
                fieldView.Init(field, _nodeMap[field.Position], _nodeMap[field.ConnectedNode.Position]);
                fieldView.name = "Field " + i++;
                _fieldMap.Add(field.Position, fieldView);
            }
        }

        private void InstantiateArcs()
        {
            var i = 0;
            foreach (var arc in _puzzle.Arcs)
            {
                var arcView = Instantiate(ArcScript);

                // Find the node at the arc's position and set it as a perent of this arc
                arcView.transform.SetParent(_nodeMap[arc.Position].transform);
                arcView.Init(arc);
                arcView.name = "Edge " + i++;
                _arcMap.Add(arc.Position, arcView);
            }
        }
    }
}
