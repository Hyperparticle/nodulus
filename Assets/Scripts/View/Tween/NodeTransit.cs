using System;
using Core.Data;
using UnityEngine;
using View.Control;
using View.Items;

namespace View.Tween
{
    /// <summary>
    /// Performs transition animations
    /// </summary>
    public class NodeTransit : MonoBehaviour
    {
        private GameAudio _gameAudio;
        private GameObject _rotor;

        public float NodeRotateTime => GameDef.Get.NodeRotateTime;

        public float WaveInTravel => GameDef.Get.WaveInTravel;
        public float WaveInAudioDelay => GameDef.Get.WaveInAudioDelay;
        public float WaveInMoveDelayStart => GameDef.Get.WaveInMoveDelayStart;
        public float WaveInMoveDelayOffsetScale => GameDef.Get.WaveInMoveDelayOffsetScale;
        public float WaveInTime => GameDef.Get.WaveInTime;
        public LeanTweenType WaveInMoveEase => GameDef.Get.WaveInMoveEase;
        public LeanTweenType WaveInColorEase => GameDef.Get.WaveInColorEase;

        public float WaveOutTravel => GameDef.Get.WaveOutTravel;
        public float WaveOutAudioDelay => GameDef.Get.WaveOutAudioDelay;
        public float WaveOutMoveDelayStart => GameDef.Get.WaveOutMoveDelayStart;
        public float WaveOutMoveDelayOffsetScale => GameDef.Get.WaveOutMoveDelayOffsetScale;
        public float WaveOutTime => GameDef.Get.WaveOutTime;
        public LeanTweenType WaveOutMoveEase => GameDef.Get.WaveOutMoveEase;
        public LeanTweenType WaveOutColorEase => GameDef.Get.WaveOutColorEase;

        private void Awake()
        {
            _gameAudio = GameObject.FindGameObjectWithTag("GameAudio").GetComponent<GameAudio>();
            _rotor = GetComponentInChildren<Colorizer>().gameObject;
        }

        public void WaveIn(int delay, Action onComplete = null, float animationSpeed = 1f, float delayScale = 1f)
        {
            onComplete = onComplete ?? (() => {});
            var pos = transform.localPosition;

            // Set node far away and transparent
            transform.Translate(WaveInTravel * Vector3.forward);

            var colorizers = GetComponentsInChildren<Colorizer>();
            foreach (var colorizer in colorizers) {
                colorizer.Fade(0f);
            }

            // TODO: use smooth function over linear delay
            var moveDelay = WaveInMoveDelayStart * delayScale + WaveInMoveDelayOffsetScale * delay;

            // Start a nice animation effect
            LeanTween.moveLocal(gameObject, pos, WaveInTime * animationSpeed)
                .setOnStart(() => _gameAudio.Play(GameClip.NodeEnter, WaveInAudioDelay))
                .setDelay(moveDelay)
                .setEase(WaveInMoveEase)
                .setOnComplete(onComplete);

            foreach (var colorizer in colorizers) {
                colorizer.Previous(WaveInTime, moveDelay, WaveInColorEase);
            }
        }

        public void WaveOut(int delay, float animationSpeed = 1f, float delayScale = 1f)
        {
            // TODO: use smooth function over linear delay
            var pos = transform.localPosition + WaveOutTravel * Vector3.forward;
            var moveDelay = WaveOutMoveDelayStart * delayScale + WaveOutMoveDelayOffsetScale * delay;

            // Start a nice animation effect
            LeanTween.moveLocal(gameObject, pos, WaveOutTime * animationSpeed)
                .setOnStart(() => _gameAudio.Play(GameClip.NodeLeave, WaveOutAudioDelay))
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
                .setEase(LeanTweenType.easeOutBack)
                .setOnComplete(onComplete);

            SlightPush(dir);
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

            SlightPush(dir);
        }

        private void SlightPush(Direction dir)
        {
            // TODO: make configurable
            const float pushAmount = 0.05f;

            LeanTween.moveLocal(_rotor, dir.Vector() * pushAmount, NodeRotateTime / 2f)
                .setEase(LeanTweenType.easeSpring)
                .setLoopPingPong(1);
        }

        public void SlightRotate(Direction dir, int arcLength)
        {
            var rotAngle = 7.5f / arcLength;
            
            var rot = Quaternion.Inverse(_rotor.transform.localRotation);
            var axis = rot * dir.Axis();

            LeanTween.rotateAroundLocal(_rotor, axis, rotAngle, NodeRotateTime / 2f)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong(1);
        }

        public void RotateFast(Direction dir)
        {
            LeanTween.cancel(_rotor);
            
            var rot = Quaternion.Inverse(_rotor.transform.localRotation);
            var axis = rot * dir.Rotated(1).Axis();

            LeanTween.rotateAroundLocal(_rotor, axis, 720f, NodeRotateTime * 3)
                .setEase(LeanTweenType.easeInOutSine);
        }
    }
}
