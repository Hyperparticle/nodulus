using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    public class BoardActions : MonoBehaviour
    {
        private PuzzleSpawner _puzzleSpawner;
        private PuzzleView _puzzleView;

        void Start()
        {
            _puzzleSpawner = GetComponent<PuzzleSpawner>();
            _puzzleView = GetComponent<PuzzleView>();
        }

        public void Swipe(NodeView nodeView, Direction direction)
        {
            if (nodeView == null || direction == Direction.None) { return; }

            // Try to obtain an arc corresponding to the node's position and the 
            // swipe's (opposite) direction.
            var pointDir = new PointDir(nodeView.Position, direction.Opposite());

            if (_puzzleSpawner.HasArcAt(pointDir))
            {
                var arcView = _puzzleSpawner.GetArc(pointDir);
                var result = _puzzleView.Puzzle.PullArc(arcView.Arc, direction.Opposite());

                if (!result)
                {
                    Debug.Log("Failed");
                    return;
                }
            }
            else if (_puzzleSpawner.HasArcAt(pointDir.Opposite)) { return; }

            var arcs = _puzzleSpawner.GetArcs(nodeView.Position);
            foreach (var arc in arcs)
            {
                arc.transform.parent = nodeView.Rotor;
            }

            nodeView.Rotate(direction);
        }

        public void Tap(FieldView fieldView)
        {
            if (fieldView == null) { return; }

            Debug.Log(fieldView);
        }
    }
}
