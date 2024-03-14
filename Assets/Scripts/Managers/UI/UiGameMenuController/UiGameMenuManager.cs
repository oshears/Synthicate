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

		[Header("Menus")]
		
		[SerializeField]
		GameObject defaultGameMenu, buildGameMenu;
		
		[Header("Buttons")]
		
		[SerializeField]
		GameObject finishTurnButton;
		
		[SerializeField]
		GameObject buildModeButton;
		
		[SerializeField]
		GameObject tradeButton;
		
		[SerializeField]
		GameObject cyberActionButton;
		
		[SerializeField]
		GameObject cancelButton;


		[Header("Scriptable Objects")]
				
		[SerializeField]
		GameManagerSO gameManagerSO;
		
		[SerializeField]
		UiScriptableObject userInterfaceSO;
		
		[Header("Event Channels")]
		[SerializeField]
		GameMenuStateEventChannel m_GameMenuStateEventChannel;
		
		void Awake() {
			userInterfaceSO.updateUserInterfaceEvent += UpdateUserInterfaceEventHandler;
			
			m_GameMenuStateEventChannel.OnEventRaised += GameMenuStateEventChannelHandler;
		}
		
		void UpdateUserInterfaceEventHandler()
		{

		}
		
		void GameMenuStateEventChannelHandler(GameMenu.Screens screen)
		{
			bool isClientTurn = screen == GameMenu.Screens.PlayerTurnScreen;
			// bool isClientSetupTurn = screen == GameMenu.Screens.PlayerSetupTurnScreen;
			finishTurnButton.SetActive(isClientTurn);
			buildModeButton.SetActive(isClientTurn);
			tradeButton.SetActive(isClientTurn);
			cyberActionButton.SetActive(isClientTurn);
			cancelButton.SetActive(isClientTurn);
		}
		
	}
}