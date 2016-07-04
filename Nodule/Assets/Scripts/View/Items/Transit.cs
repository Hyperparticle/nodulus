using System;
using Assets.Scripts.Core.Data;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    /// <summary>
    /// Performs transition animations
    /// </summary>
    public class Transit : MonoBehaviour
    {
        private AudioSource _audioSource;
        private GameObject _rotor;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.time = 0.5f;

            _rotor = GetComponentInChildren<Colorizer>().gameObject;
        }

        public void WaveIn(int delay)
        {
            var pos = transform.localPosition;

            // Set node far away and transparent
            transform.Translate(10*Vector3.forward);

            var colorizers = GetComponentsInChildren<Colorizer>();
            foreach (var colorizer in colorizers) {
                colorizer.Fade(0f);
            }

            // TODO: use smooth function over linear delay
            var moveDelay = 1.25f + 0.1f*delay;

            // Start a nice animation effect
            LeanTween.moveLocal(gameObject, pos, 1f)
                .setOnStart(() => _audioSource.PlayDelayed(0.1f))
                .setDelay(moveDelay)
                .setEase(LeanTweenType.easeOutBack);

            foreach (var colorizer in colorizers) {
                colorizer.Previous(1f, moveDelay, LeanTweenType.easeOutExpo);
            }
        }

        public void WaveOut(int delay)
        {
            // TODO: use smooth function over linear delay
            var pos = transform.localPosition + 10*Vector3.forward;
            var random = 0f; //Random.Range(0f, 0.25f);
            var moveDelay = 0.05f*delay + random;

            // Start a nice animation effect
            LeanTween.moveLocal(gameObject, pos, 0.75f)
                .setDelay(moveDelay)
                .setEase(LeanTweenType.easeInBack);

            var colorizers = GetComponentsInChildren<Colorizer>();
            foreach (var colorizer in colorizers) {
                colorizer.Fade(0.75f, moveDelay, LeanTweenType.easeInExpo);
            }
        }

        public void Rotate90(Direction dir, Action onComplete)
        {
            // Grab the axis of the direction, and rotate it relative to the current rotation.
            // This is accomplished by getting the rotation that undoes the current rotation, 
            // and applying it to the absolute axis to get the relative axis we want
            var rot = Quaternion.Inverse(_rotor.transform.localRotation);
            var axis = rot * dir.Axis();

            // Rotate 90 degrees in the direction specified
            // TODO: remove magic constants
            LeanTween.rotateAroundLocal(_rotor, axis, 90f, 0.33f)
                .setEase(LeanTweenType.easeInOutSine)
                //.setOnComplete(OnRotateComplete)
                .setOnComplete(onComplete);
        }
    }
}
