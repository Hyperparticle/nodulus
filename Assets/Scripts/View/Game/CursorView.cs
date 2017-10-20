using UnityEngine;

namespace View.Game
{
	public class CursorView : MonoBehaviour {

		public Texture2D CursorTexture;
		public Texture2D CursorTextureHot;

		private const CursorMode CursorMode = UnityEngine.CursorMode.Auto;

		private void Start() {
			var hotSpot = new Vector2 (CursorTexture.width, CursorTexture.height) / 2f;
			Cursor.SetCursor(CursorTexture, hotSpot, CursorMode);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Mouse0)) {
				var hotSpot = new Vector2(CursorTextureHot.width, CursorTextureHot.height) / 2f;
				Cursor.SetCursor(CursorTextureHot, hotSpot, CursorMode);
			} else if (Input.GetKeyUp(KeyCode.Mouse0)) {
				var hotSpot = new Vector2(CursorTexture.width, CursorTexture.height) / 2f;
				Cursor.SetCursor(CursorTexture, hotSpot, CursorMode);
			}
		}
	}
}
