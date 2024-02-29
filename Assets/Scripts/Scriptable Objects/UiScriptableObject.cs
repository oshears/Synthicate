using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
	[CreateAssetMenu(fileName = "UiScriptableObject", menuName = "ScriptableObjects/UiScriptableObject")]
	public class UiScriptableObject : ScriptableObject
	{
		public delegate void UserInterfaceEvent();
		public event UserInterfaceEvent singlePlayerButton;
		public event UserInterfaceEvent multiPlayerButton;
		public event UserInterfaceEvent quitButton;
		public UnityEvent<bool> setMainMenuActiveEvent;
		public UnityEvent<bool> setGameMenuActiveEvent;
		public event UserInterfaceEvent updateUserInterfaceEvent;
		
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
		
		public void OnSetGameMenuActive(bool active)
		{
			setGameMenuActiveEvent.Invoke(active);
		}
		
		public void OnUpdateUserInterface()
		{
			updateUserInterfaceEvent.Invoke();
		}
	}
}