using UnityEngine;
using View.Items;

namespace View.Control
{
    public class MenuRotator : MonoBehaviour
    {
        public ButtonScript TopButton;
        public ButtonScript BottomButton;

        private GameObject _currentButton;
        private GameObject _disabledButton;

        private void Awake()
        {
            _currentButton = TopButton.gameObject;
            _disabledButton = BottomButton.gameObject;
        }

        private void Start()
        {
            _disabledButton.GetComponent<BoxCollider>().enabled = false;
        }

        public void Toggle()
        {
            // TODO: make configurable
            const float time = 0.8f;
            const float delay = 0.2f;
            
            _currentButton.GetComponent<BoxCollider>().enabled = false;

            LeanTween.rotateAroundLocal(gameObject, Vector3.right, 180f, time)
                .setDelay(delay)
                .setEase(LeanTweenType.easeInOutBack)
                .setOnComplete(() => {
                    var tmp = _currentButton;
                    _currentButton = _disabledButton;
                    _disabledButton = tmp;
                    
                    _currentButton.GetComponent<BoxCollider>().enabled = true;
                });
        }
    }
}
