using UnityEngine;

namespace View.Control
{
    public class GameAudio : MonoBehaviour
    {
        private const float MusicVolume = 0.2f;
        
        public AudioClip[] MusicClips;
        public AudioClip[] SfxClips;

        private AudioSource _musicSource;
        private bool _musicEnabled = true;
        public bool MusicEnabled
        {
            get { return _musicEnabled; }
            set {
                _musicEnabled = value;
                _musicSource.volume = value ? MusicVolume : 0f;
            }
        }

        public bool SfxEnabled { get; set; } = true;

        private void Start()
        {
            StartMusic();
        }

        public void Play(GameClip clip, float delay = 0f, float volume = 1f, float startTime = 0f)
        {
            if (!enabled || !SfxEnabled) {
                return;
            }
            
            var audioClip = SfxClips[(uint) clip];
            LeanAudio.play(audioClip, volume, delay, time: startTime);
        }
        
        public void Play(MusicClip clip, float fadeTime = 0f, float delay = 0f, float volume = 1f, float startTime = 0f)
        {
            if (!enabled || !MusicEnabled) {
                return;
            }
            
            var audioClip = MusicClips[(uint) clip];
            
            _musicSource = LeanAudio.play(audioClip, 0f, delay, true, startTime);

            LeanTween.value(0f, volume, fadeTime)
                .setDelay(delay)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnUpdate(v => {
                    _musicSource.volume = v;
                });
        }

        private void StartMusic()
        {
            // TODO: make configurable
            const float fadeTime = 3f;
            const float startTime = 32f;
            Play(MusicClip.Ambient02, fadeTime: fadeTime, volume: MusicVolume, startTime: startTime);
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
        ArcMoveHigh,
        NodeRotate90,
        InvalidRotate,
        MenuSelect,
        GameEnd,
        LevelEnable,
        ArcMoveMid,
        ArcMoveLow
    }

    public enum MusicClip
    {
        Ambient01,
        Ambient02
    }
}
