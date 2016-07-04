using System.Collections;
using UnityEngine;

namespace Assets.Scripts.View.Control
{
    public class GameAudio : MonoBehaviour
    {
        public AudioClip[] AudioClips;
        private AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(Clip clip, float delay = 0f)
        {
            _audioSource.clip = AudioClips[(uint) clip];
            _audioSource.PlayDelayed(delay);
        }
    }

    public enum Clip
    {
        MovePull,
        MovePush,
        WinBoard
    }
}
