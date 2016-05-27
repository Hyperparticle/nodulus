using System.Collections.Generic;
using Assets.Scripts.Core.Game;
using Assets.Scripts.View.Game;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Builders
{
    public class PuzzleSpawner : MonoBehaviour {
    
        public float DestroyDelay = 0.3f;

        public NodeView  NodePrefab;
        public ArcView   ArcPrefab;
        public FieldView FieldPrefab;

        //public DrawGrid BackdropPrefab;

        private readonly ICollection<NodeView>  _nodes = new HashSet<NodeView>();
        private readonly ICollection<ArcView>   _edges = new HashSet<ArcView>();
        private readonly ICollection<FieldView> _fields = new HashSet<FieldView>();
        //private DrawGrid _backdrop;

        private Puzzle _puzzle;

        public Puzzle SpawnBoard(int level)
        {
            _puzzle = Level.BuildLevel(level);
        
            InstantiateNodes();
            InstantiateFields();
            InstantiateEdges();
        
            return _puzzle;
        }

        public void DestroyBoard()
        {
            if (_puzzle == null) return;

            foreach (var node in _nodes) { Destroy(node.gameObject, DestroyDelay); }
            foreach (var edge in _edges) { Destroy(edge.gameObject, DestroyDelay); }
            foreach (var field in _fields) { Destroy(field.gameObject, DestroyDelay); }
            //Destroy(_backdrop.gameObject, DestroyDelay);

            _nodes.Clear();
            _edges.Clear();
            _fields.Clear();
        
            _puzzle = null;
        }

        private void InstantiateNodes()
        {
            var i = 0;
            foreach (var node in _puzzle.Nodes)
            {
                var nodeScript = Instantiate(NodePrefab);
                nodeScript.transform.SetParent(transform.parent.transform);
                nodeScript.Init(node);
                nodeScript.name = "Node " + i++;
                nodeScript.Node.NodeScript = nodeScript;
                _nodes.Add(nodeScript);
            }
        }

        private void InstantiateFields()
        {
            var i = 0;
            foreach (var field in _puzzle.Fields)
            {
                var fieldScript = Instantiate(FieldPrefab);
                fieldScript.transform.parent = field.ParentNode.NodeScript.transform;
                fieldScript.Init(field, field.ParentNode.NodeScript, field.ConnectedNode.NodeScript);
                fieldScript.name = "Field " + i++;
                fieldScript.Field.FieldScript = fieldScript;
                _fields.Add(fieldScript);
            }
        }

        private void InstantiateEdges()
        {
            var i = 0;
            foreach (var edge in _puzzle.Arcs)
            {
                var edgeScript = Instantiate(ArcPrefab);
                edgeScript.transform.SetParent(edge.ParentNode.NodeScript.Rotor);
                edgeScript.Init(edge);
                edgeScript.name = "Edge " + i++;
                edgeScript.Edge.EdgeScript = edgeScript;
                _edges.Add(edgeScript);
            }
        }

        //public DrawGrid CreateBackdrop(PuzzleView puzzleView)
        //{
        //    _backdrop = Instantiate(BackdropPrefab);
        //    _backdrop.transform.parent = transform.parent.transform;
        //    _backdrop.name = "Backdrop";
        //    _backdrop.Init(_puzzle.BoardSize, puzzleView.Scaling);
        
        //    return _backdrop;
        //}
    }
}
