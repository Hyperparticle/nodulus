using UnityEngine;

namespace View.Control
{
    public class GameAudio : MonoBehaviour
    {
        public AudioClip[] AudioClips;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(GameClip clip, float delay = 0f, float volume = 1f)
        {
            LeanTween.delayedCall(gameObject, delay, () => {
                _audioSource.PlayOneShot(AudioClips[(uint) clip], volume);
            });
        }
    }

    public enum GameClip
    {
        MovePull,
        MovePush,
        WinBoard,
        GameStart,
        NodeEnter,
        NodeLeave,
        NodeRotate90,
        InvalidRotate,
        ArcMove
    }
}
