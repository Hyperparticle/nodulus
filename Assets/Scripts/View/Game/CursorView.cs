using UnityEngine;

namespace View.Game
{
	public class CursorView : MonoBehaviour {

		public Texture2D CursorTexture;

		private const CursorMode CursorMode = UnityEngine.CursorMode.Auto;
		private readonly Vector2 _hotSpot = Vector2.one * 32f;

		private void Start() {
			Cursor.SetCursor(CursorTexture, _hotSpot, CursorMode);
		}
	
	}
}
