using UnityEngine;

namespace View.Game
{
	public class CursorView : MonoBehaviour {

		public Texture2D CursorTexture;

		private const CursorMode CursorMode = UnityEngine.CursorMode.Auto;

		private void Start() {
			var hotSpot = new Vector2 (CursorTexture.width, CursorTexture.height) / 2f;
			Cursor.SetCursor(CursorTexture, hotSpot, CursorMode);
		}
	
	}
}
