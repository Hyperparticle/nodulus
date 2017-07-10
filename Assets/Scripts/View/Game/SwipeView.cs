using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeView : MonoBehaviour {

    public GameObject particles;

    private GameObject _particleObject;
    private ParticleSystem _particleSystem;
    private ParticleSystem.EmissionModule _emission;

    void Start() {
        _particleObject = Instantiate(particles, transform);
        _particleObject.name = "Particles";

        _particleSystem = _particleObject.GetComponent<ParticleSystem>();
        _emission = _particleSystem.emission;
        _emission.enabled = false;
    }

    void Update()
    {
        //if (Input.GetMouseButton(0)) {
        //    _emission.enabled = true;
        //    var touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    _particleObject.transform.position = touch + Vector3.forward * 2f;
        //} else {
        //    _emission.enabled = false;
        //}
	}
}
