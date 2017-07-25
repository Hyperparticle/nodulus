using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using View.Game;

namespace Editor
{
	[CustomEditor(typeof(PuzzleState))]
	public class PuzzleStateEditor : UnityEditor.Editor
	{
		private string _text = "0";
		
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (!Application.isPlaying) {
				return;
			}

			var puzzleState = (PuzzleState) target;

			GUILayout.BeginHorizontal();
			GUILayout.Label("Level", GUILayout.Width(50));
			_text = GUILayout.TextField(_text, 4, GUILayout.Width(40));
			_text = Regex.Replace(_text, @"[^0-9]", "");

			int level;
			int.TryParse(_text, out level);

			if (GUILayout.Button("Create", GUILayout.Width(75))) {
				puzzleState.Init(level);
			}
			
			GUILayout.EndHorizontal();
		}
	}
}
