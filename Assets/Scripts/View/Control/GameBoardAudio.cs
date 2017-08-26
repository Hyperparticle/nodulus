using UnityEngine;

namespace View.Control
{
    public class GameBoardAudio : MonoBehaviour
    {
        private GameAudio _gameAudio;

        private void Awake()
        {
            _gameAudio = GameObject.FindGameObjectWithTag("GameAudio").GetComponent<GameAudio>();
        }

        public void Play(GameClip clip, float delay = 0f, float volume = 1f, float startTime = 0f)
        {
            if (!enabled) {
                return;
            }
            
            _gameAudio.Play(clip, delay, volume, startTime);
        }
        
        public void Play(MusicClip clip, float fadeTime = 0f, float delay = 0f, float volume = 1f, float startTime = 0f)
        {
            if (!enabled) {
                return;
            }
            
            _gameAudio.Play(clip, fadeTime, delay, volume, startTime);
        }
    }
}
