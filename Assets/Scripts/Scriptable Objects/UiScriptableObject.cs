using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
	[CreateAssetMenu(fileName = "UiScriptableObject", menuName = "ScriptableObjects/UiScriptableObject")]
	public class UiScriptableObject : ScriptableObject
	{
		public UnityEvent singlePlayerButton;
		public UnityEvent multiPlayerButton;
		public UnityEvent quitButton;
		
		public void OnSinglePlayerButtonEvent()
		{
			singlePlayerButton.Invoke();
		}
		
		public void OnMultiPlayerButtonEvent()
		{
			multiPlayerButton.Invoke();
		}
		
		public void OnQuitButtonEvent()
		{
			quitButton.Invoke();
		}
	}
}