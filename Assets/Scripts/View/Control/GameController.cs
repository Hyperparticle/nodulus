using UnityEngine;

namespace View.Control
{
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
