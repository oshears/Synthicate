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
		public UnityEvent<bool> setMainMenuActiveEvent;
		
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
		
		public void OnSetMainMenuActive(bool active)
		{
			setMainMenuActiveEvent.Invoke(active);
		}
	}
}