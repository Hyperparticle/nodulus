using UnityEngine;

namespace View.Control
{
	/// <summary>
	/// The main game controller.
	/// </summary>
	public class GameController : MonoBehaviour
	{
		private void Update() 
		{
			if (Input.GetKeyDown(KeyCode.Escape)) {
				Application.Quit();
			}
		}
	}
}
