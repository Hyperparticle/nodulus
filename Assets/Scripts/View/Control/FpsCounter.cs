using UnityEngine;

namespace View.Control
{
    public class FpsCounter : MonoBehaviour {
        
        private float _deltaTime;

        private readonly GUIStyle _style = new GUIStyle();

        private void Awake()
        {
            var h = Screen.height;
            
            _style.alignment = TextAnchor.UpperLeft;
            _style.fontSize = h * 4 / 100;
            _style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
        }

        private void Update()
        {
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;
 
            var rect = new Rect(480, 48, w, h * 2f / 100f);
            var msec = _deltaTime * 1000.0f;
            var fps = 1.0f / _deltaTime;
            var text = $"{msec:0.0} ms ({fps:0.} fps)";
            
            GUI.Label(rect, text, _style);
        }
    }
}
