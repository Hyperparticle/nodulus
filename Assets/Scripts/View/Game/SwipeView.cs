using UnityEngine;

namespace View.Game
{
    public class SwipeView : MonoBehaviour {

        public GameObject Particles;

        private GameObject _particleObject;
        private ParticleSystem _particleSystem;
        private ParticleSystem.EmissionModule _emission;

        private void Start() {
            _particleObject = Instantiate(Particles, transform);
            _particleObject.name = "Particles";

            _particleSystem = _particleObject.GetComponent<ParticleSystem>();
            _emission = _particleSystem.emission;
            _emission.enabled = false;
        }

//        private void Update()
//        {
//            if (Input.GetMouseButton(0)) {
//                _emission.enabled = true;
//                var touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//                _particleObject.transform.position = touch + Vector3.forward * 2f;
//            } else {
//                _emission.enabled = false;
//            }
//        }
    }
}
