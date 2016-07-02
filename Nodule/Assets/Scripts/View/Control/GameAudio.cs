using UnityEngine;
using System.Collections;

public class GameAudio : MonoBehaviour
{
    public AudioClip[] AudioClips;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play(Clip clip)
    {
        _audioSource.clip = AudioClips[(uint) clip];
        _audioSource.Play();
    }
}

public enum Clip
{
    MovePull,
    MovePush
}
