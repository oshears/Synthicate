using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Synthicate
{
	public class UiGameMenuManager : MonoBehaviour
	{

		#region Buttons
		[SerializeField]
		GameObject defaultGameMenu, buildGameMenu;
		[SerializeField]
		GameObject finishTurnButton, buildModeButton, tradeButton, cyberActionButton, cancelButton;
		#endregion
		
		#region Scriptable Objects
		[SerializeField]
		GameManagerSO gameManagerSO;
		#endregion
		
		[SerializeField]
		UiScriptableObject userInterfaceSO;
		
		void Awake() {
			userInterfaceSO.updateUserInterfaceEvent += UpdateUserInterfaceEventHandler;
		}
		
		void UpdateUserInterfaceEventHandler()
		{

		}
		
	}
}