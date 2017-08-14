using System.Collections.Generic;
using UnityEngine;

namespace View.Items
{
    public class ItemPool : MonoBehaviour
    {
        private readonly Queue<NodeView> _nodes = new Queue<NodeView>();
        private readonly Queue<ArcView> _arcs = new Queue<ArcView>();
        private readonly Queue<FieldView> _fields = new Queue<FieldView>();
        private readonly Queue<GameObject> _gridLines = new Queue<GameObject>();

        public NodeView NodePrefab;
        public ArcView ArcPrefab;
        public FieldView FieldPrefab;
        
        public GameObject GridLinePrefab;
        public GameObject GridPointPrefab;
        
        public void Release(NodeView node, float delay = 0f)
        {
//            LeanTween.delayedCall(delay, () => {
//                // TODO: methods to reset to initial conditions
//                node.transform.parent = transform;
//                node.transform.localScale = Vector3.one;
//                _nodes.Enqueue(node);
//            });
            
            Destroy(node.gameObject, delay);
        }
        
        public void Release(ArcView arc, float delay = 0f)
        {
//            LeanTween.delayedCall(delay, () => {
//                arc.transform.parent = transform;
//                _arcs.Enqueue(arc);
//            });
            
            Destroy(arc.gameObject, delay);
        }

        public void Release(FieldView field, float delay = 0f)
        {
//            LeanTween.delayedCall(delay, () => {
//                field.transform.parent = transform;
//                _fields.Enqueue(field);
//            });
            
            Destroy(field.gameObject, delay);
        }
        
        public void ReleaseGridLines(GameObject obj, float delay = 0f)
        {
//            LeanTween.delayedCall(delay, () => {
//                _gridLines.Enqueue(obj);
//            });
            
            Destroy(obj, delay);
        }
        
        public NodeView GetNode()
        {
//            return _nodes.Count <= 0 ? Instantiate(NodePrefab) : _nodes.Dequeue();
            return Instantiate(NodePrefab);
        }

        public ArcView GetArc()
        {
//            return _arcs.Count <= 0 ? Instantiate(ArcPrefab) : _arcs.Dequeue();
            return Instantiate(ArcPrefab);
        }
        
        public FieldView GetField()
        {
//            return _fields.Count <= 0 ? Instantiate(FieldPrefab) : _fields.Dequeue();
            return Instantiate(FieldPrefab);
        }
        
        public GameObject GetGridLine()
        {
//            return _gridLines.Count <= 0 ? Instantiate(GridLinePrefab) : _gridLines.Dequeue();
            return Instantiate(GridLinePrefab);
        }
    }
}
