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
		GameObject m_DefaultGameMenu;
		
		[SerializeField]
		GameObject m_BuildGameMenu;
		
		[SerializeField]
		GameObject m_TradeInitMenu;
		
		[SerializeField]
		GameObject m_TradeMenu;
		
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
		
		[SerializeField]
		EventChannelSO m_CancelButtonEventChannel;
		
		[SerializeField]
		EventChannelSO m_CyberActionButtionEventChannel;
		
		[SerializeField]
		EventChannelSO m_TradeButtonEventChannel;
		
		[SerializeField]
		EventChannelSO m_BuildModeButtonEventChannel;
		
		[SerializeField]
		EventChannelSO m_FinishTurnButtonEventChannel; 
		
		
		void Awake() {
			userInterfaceSO.updateUserInterfaceEvent += UpdateUserInterfaceEventHandler;
			
			m_GameMenuStateEventChannel.OnEventRaised += GameMenuStateEventChannelHandler;
			
			
			cancelButton.GetComponent<Button>().onClick.AddListener(m_CancelButtonEventChannel.RaiseEvent);
			cyberActionButton.GetComponent<Button>().onClick.AddListener(m_CyberActionButtionEventChannel.RaiseEvent);
			tradeButton.GetComponent<Button>().onClick.AddListener(m_TradeButtonEventChannel.RaiseEvent);
			buildModeButton.GetComponent<Button>().onClick.AddListener(m_BuildModeButtonEventChannel.RaiseEvent);
			finishTurnButton.GetComponent<Button>().onClick.AddListener(m_FinishTurnButtonEventChannel.RaiseEvent);
		}
		
		void Start()
		{
			m_DefaultGameMenu.SetActive(true);
			m_BuildGameMenu.SetActive(false);
			m_TradeInitMenu.SetActive(false);
			m_TradeMenu.SetActive(false);
			
			DisableMenuButtons();
		}
		
		void UpdateUserInterfaceEventHandler()
		{

		}
		
		void GameMenuStateEventChannelHandler(GameMenuType screen)
		{
			DisableCanvases();
			DisableMenuButtons();
			
			if (screen == GameMenuType.PlayerTurnScreen)
			{
				finishTurnButton.SetActive(true);
				buildModeButton.SetActive(true);
				tradeButton.SetActive(true);
				cyberActionButton.SetActive(true);
			}
			else if (screen == GameMenuType.PlayerBuildModeScreen)
			{
				m_BuildGameMenu.SetActive(true);
				cancelButton.SetActive(true);
			}
			else if (screen  == GameMenuType.TradeInitScreen)
			{
				m_TradeInitMenu.SetActive(true);
			}
			else if (screen  == GameMenuType.TradeRequesterScreen)
			{
				m_TradeMenu.SetActive(true);
			}
			else if (screen  == GameMenuType.TradeReceiverScreen)
			{
				m_TradeMenu.SetActive(true);
			}
			else if (screen  == GameMenuType.PlayerDiceHackTargetScreen)
			{
				// m_TradeMenu.SetActive(true);
				m_TradeInitMenu.SetActive(true);
			}
			
		}
		
		void DisableMenuButtons()
		{
			finishTurnButton.SetActive(false);
			buildModeButton.SetActive(false);
			tradeButton.SetActive(false);
			cyberActionButton.SetActive(false);
			cancelButton.SetActive(false);
		}
		
		void DisableCanvases()
		{
			m_DefaultGameMenu.SetActive(true);
			m_BuildGameMenu.SetActive(false);
			m_TradeInitMenu.SetActive(false);
			m_TradeMenu.SetActive(false);
		}
		
	}
}