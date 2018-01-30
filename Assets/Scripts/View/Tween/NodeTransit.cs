using System;
using System.Linq;
using Core.Data;
using UnityEngine;
using View.Control;
using View.Items;

namespace View.Tween
{
    /// <summary>
    /// Performs transition animations on nodes.
    /// </summary>
    public class NodeTransit : MonoBehaviour
    {
        private GameBoardAudio _gameAudio;
        private GameObject _rotor;
        private GameObject _checkmark;

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
            _rotor = GetComponentInChildren<Colorizer>().gameObject;
        }

        public void Init()
        {
            _gameAudio = GetComponentInParent<GameBoardAudio>();

            if (_rotor.transform.childCount > 0) {
                _checkmark = _rotor.transform.GetChild(0).gameObject;
            }
        }

        public void WaveIn(int delay, int maxDelay, Action onComplete = null, float animationSpeed = 1f, float delayScale = 1f)
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
            var moveDelay = WaveInMoveDelayStart * delayScale + (WaveInMoveDelayOffsetScale * delay) / maxDelay;

            // Start a nice animation effect
            LeanTween.moveLocal(gameObject, pos, WaveInTime * animationSpeed)
                .setOnStart(() => {
                    _gameAudio.Play(GameClip.NodeEnter, WaveInAudioDelay);

                    if (_checkmark == null) {
                        return;
                    }
                    _checkmark.SetActive(true);
                    LeanTween.moveLocal(_checkmark, Vector3.back * 0.5f, WaveInTime * animationSpeed / 2f)
                        .setEase(LeanTweenType.easeInOutSine);
                })
                .setDelay(moveDelay)
                .setEase(WaveInMoveEase)
                .setOnComplete(onComplete);

            foreach (var colorizer in colorizers) {
                colorizer.Previous(WaveInTime, moveDelay, WaveInColorEase);
            }
        }

        public void WaveOut(int delay, int maxDelay, float animationSpeed = 1f, float delayScale = 1f, Action onComplete = null,
            bool playSound = true)
        {
            onComplete = onComplete ?? (() => {});
            
            // TODO: use smooth function over linear delay
            var pos = transform.localPosition + WaveOutTravel * Vector3.forward;
            var moveDelay = WaveOutMoveDelayStart * delayScale + (WaveOutMoveDelayOffsetScale * delay) / maxDelay;

            // Start a nice animation effect
            LeanTween.moveLocal(gameObject, pos, WaveOutTime * animationSpeed)
                .setOnStart(() => {
                    if (playSound) {
                        _gameAudio.Play(GameClip.NodeLeave, WaveOutAudioDelay);
                    }
                    
                    if (_checkmark == null) {
                        return;
                    }
                    LeanTween.moveLocal(_checkmark, Vector3.zero, WaveOutTime * animationSpeed)
                        .setEase(LeanTweenType.easeInOutSine)
                        .setOnComplete(() => _checkmark.SetActive(false));
                })
                .setDelay(moveDelay)
                .setEase(WaveOutMoveEase)
                .setOnComplete(onComplete);

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

            LeanTween.rotateAroundLocal(_rotor, axis, 900f, NodeRotateTime * 3)
                .setEase(LeanTweenType.easeInOutSine);
        }

        public void PushDown()
        {
            // TODO: make configurable
            const float pushAmount = 0.1f;
            const float time = 0.08f;
            var delay = NodeRotateTime - 0.18f;

            var pos = _rotor.transform.localPosition + Vector3.forward * pushAmount;

            LeanTween.moveLocal(_rotor, pos, time)
                .setDelay(delay)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong(1);
        }
    }
}
