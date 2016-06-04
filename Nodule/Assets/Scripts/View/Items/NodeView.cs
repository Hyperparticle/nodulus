using System.Linq;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Game;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class NodeView : MonoBehaviour
    {
        public Transform Rotor;

        private ScaleScript _nodeScale;
        private Colorizer _colorizer;
        private GameObject _rotor;

        public Node Node { get; private set; }

        public void Init(Node node, bool inStartIsland)
        {
            _nodeScale = GetComponent<ScaleScript>();
            _colorizer = GetComponentInChildren<Colorizer>();
            _rotor = _colorizer.gameObject;

            Node = node;

            _nodeScale.SetNode(node);

            if (!inStartIsland) { _colorizer.Darken(); }
            if (node.Final) { _colorizer.SetSecondary(); }
        }

        public bool Rotate(Direction direction)
        {
            if (LeanTween.isTweening(_colorizer.gameObject)) { return false; }

            LeanTween.rotateAroundLocal(_rotor, direction.Axis(), 90f, 0.5f)
               .setEase(LeanTweenType.easeInOutSine)
               .setOnComplete(() => {
                   _rotor.transform.localRotation = Quaternion.identity;
               });

            return true;
        }
    }
}

