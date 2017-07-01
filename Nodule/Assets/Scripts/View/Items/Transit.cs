using System;
using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Control;
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

        public float NodeRotateTime { get { return GameDef.Get.NodeRotateTime; } }

        public float WaveInTravel { get { return GameDef.Get.WaveInTravel; } }
        public float WaveInAudioDelay { get { return GameDef.Get.WaveInAudioDelay; } }
        public float WaveInMoveDelayStart { get { return GameDef.Get.WaveInMoveDelayStart; } }
        public float WaveInMoveDelayOffsetScale { get { return GameDef.Get.WaveInMoveDelayOffsetScale; } }
        public float WaveInTime { get { return GameDef.Get.WaveInTime; } }
        public LeanTweenType WaveInMoveEase { get { return GameDef.Get.WaveInMoveEase; } }
        public LeanTweenType WaveInColorEase { get { return GameDef.Get.WaveInColorEase; } }

        public float WaveOutTravel { get { return GameDef.Get.WaveOutTravel; } }
        public float WaveOutAudioDelay { get { return GameDef.Get.WaveOutAudioDelay; } }
        public float WaveOutMoveDelayStart { get { return GameDef.Get.WaveOutMoveDelayStart; } }
        public float WaveOutMoveDelayOffsetScale { get { return GameDef.Get.WaveOutMoveDelayOffsetScale; } }
        public float WaveOutTime { get { return GameDef.Get.WaveOutTime; } }
        public LeanTweenType WaveOutMoveEase { get { return GameDef.Get.WaveOutMoveEase; } }
        public LeanTweenType WaveOutColorEase { get { return GameDef.Get.WaveOutColorEase; } }

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
            transform.Translate(WaveInTravel * Vector3.forward);

            var colorizers = GetComponentsInChildren<Colorizer>();
            foreach (var colorizer in colorizers) {
                colorizer.Fade(0f);
            }

            // TODO: use smooth function over linear delay
            var moveDelay = WaveInMoveDelayStart + WaveInMoveDelayOffsetScale * delay;

            // Start a nice animation effect
            LeanTween.moveLocal(gameObject, pos, WaveInTime)
                .setOnStart(() => _audioSource.PlayDelayed(WaveInAudioDelay))
                .setDelay(moveDelay)
                .setEase(WaveInMoveEase);

            foreach (var colorizer in colorizers) {
                colorizer.Previous(WaveInTime, moveDelay, WaveInColorEase);
            }
        }

        public void WaveOut(int delay)
        {
            // TODO: use smooth function over linear delay
            var pos = transform.localPosition + WaveOutTravel * Vector3.forward;
            var moveDelay = WaveOutMoveDelayStart + WaveOutMoveDelayOffsetScale * delay;

            // Start a nice animation effect
            LeanTween.moveLocal(gameObject, pos, WaveOutTime)
                .setDelay(moveDelay)
                .setEase(WaveOutMoveEase);

            var colorizers = GetComponentsInChildren<Colorizer>();
            foreach (var colorizer in colorizers) {
                colorizer.Fade(WaveOutTime, moveDelay, WaveOutColorEase);
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
            LeanTween.rotateAroundLocal(_rotor, axis, 90f, NodeRotateTime)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(onComplete);
        }

        public void Shake(Direction dir, Action onComplete)
        {
            // Grab the axis of the direction, and rotate it relative to the current rotation.
            // This is accomplished by getting the rotation that undoes the current rotation, 
            // and applying it to the absolute axis to get the relative axis we want
            var rot = Quaternion.Inverse(_rotor.transform.localRotation);
            var axis = rot * dir.Axis();
            
            LeanTween.rotateAroundLocal(_rotor, axis, 30f, NodeRotateTime)
                .setEase(LeanTweenType.easeShake)
                .setOnComplete(onComplete);
        }

        public void RotateFast()
        {
            var rot = Quaternion.Inverse(_rotor.transform.localRotation);

            LeanTween.rotateAroundLocal(_rotor, Direction.Right.Axis() + Direction.Down.Axis(), 360f, NodeRotateTime * 2)
                .setEase(LeanTweenType.easeInOutSine);
            LeanTween.scale(_rotor, Vector3.one * 2, NodeRotateTime * 2);
        }
    }
}
