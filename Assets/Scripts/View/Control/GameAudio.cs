using UnityEngine;

namespace View.Control
{
    public class GameAudio : MonoBehaviour
    {
        public AudioClip[] MusicClips;
        public AudioClip[] SfxClips;

        private void Start()
        {
            // TODO: make configurable
            const float fadeTime = 3f;
            const float volume = 0.6f;
            Play(MusicClip.Ambient01, fadeTime: fadeTime, volume: volume);
        }

        public void Play(GameClip clip, float delay = 0f, float volume = 1f)
        {
            if (!enabled) {
                return;
            }
            
            var audioClip = SfxClips[(uint) clip];
            LeanAudio.play(audioClip, volume, delay);
        }
        
        public void Play(MusicClip clip, float fadeTime = 0f, float delay = 0f, float volume = 1f)
        {
            if (!enabled) {
                return;
            }
            
            var audioClip = MusicClips[(uint) clip];
            
            var audioSource = LeanAudio.play(audioClip, 0f, delay, true);

            LeanTween.value(0f, volume, fadeTime)
                .setDelay(delay)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnUpdate(v => {
                    audioSource.volume = v;
                });
        }
    }

    public enum GameClip
    {
        GameStart,
        WinBoard,
        NodeEnter,
        NodeLeave,
        MovePushHigh,
        MovePullHigh,
        MovePullMid,
        MovePushMid,
        MovePullLow,
        MovePushLow,
        ArcMove,
        NodeRotate90,
        InvalidRotate
    }

    public enum MusicClip
    {
        Ambient01
    }
}
