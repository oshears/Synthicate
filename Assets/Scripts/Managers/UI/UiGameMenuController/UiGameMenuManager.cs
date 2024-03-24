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
		
		// [SerializeField] // GameObject m_DefaultGameMenu;
		
		// [SerializeField] // GameObject m_CancelGameMenu;
		
		// [SerializeField] // GameObject m_CyberActionGameMenu;
		
		[SerializeField] GameObject m_TradeInitMenu;
		
		[SerializeField] GameObject m_TradeMenu;
		
		[SerializeField] UserInterfaceDepotTradeWindow m_DepotTradeMenu;
		
		[SerializeField] GameObject m_AnyDepotTradeMeu;
		
		[Header("Windows")]
		[SerializeField] GameObject m_PlayersView;
		[SerializeField] GameObject m_PlayerResourceView;
		
		[Header("Buttons")]
		
		[SerializeField] GameObject finishTurnButton;
		
		[SerializeField] GameObject buildModeButton;
		
		[SerializeField] GameObject tradeButton;
		
		[SerializeField] GameObject cyberActionButton;
		
		[SerializeField] GameObject cancelButton;
		
		[SerializeField] GameObject hackButton;
		
		[SerializeField] GameObject m_BuyEventCardButton;


		[Header("Scriptable Objects")]
				
		[SerializeField] GameManagerSO gameManagerSO;
		
		[SerializeField] UiScriptableObject userInterfaceSO;
		
		[Header("Event Channels")]
		
		[SerializeField] GameMenuStateEventChannel m_GameMenuStateEventChannel;
		
		[SerializeField] EventChannelSO m_CancelButtonEventChannel;
		
		[SerializeField] EventChannelSO m_CyberActionButtionEventChannel;
		
		[SerializeField] EventChannelSO m_TradeButtonEventChannel;
		
		[SerializeField] EventChannelSO m_BuildModeButtonEventChannel;
		
		[SerializeField] EventChannelSO m_FinishTurnButtonEventChannel; 
		
		[SerializeField] EventChannelSO m_HackButtonEventChannel;
		
		[SerializeField] EventChannelSO m_BuyEventCardEventChannel;
		
		[SerializeField] EventChannelSO m_UpdateUiEventChannel;
		
		void Awake() {
			m_UpdateUiEventChannel.OnEventRaised += UpdateUserInterfaceEventHandler;
			
			m_GameMenuStateEventChannel.OnEventRaised += GameMenuStateEventChannelHandler;
			
			
			cancelButton.GetComponent<Button>().onClick.AddListener(m_CancelButtonEventChannel.RaiseEvent);
			cyberActionButton.GetComponent<Button>().onClick.AddListener(m_CyberActionButtionEventChannel.RaiseEvent);
			tradeButton.GetComponent<Button>().onClick.AddListener(m_TradeButtonEventChannel.RaiseEvent);
			buildModeButton.GetComponent<Button>().onClick.AddListener(m_BuildModeButtonEventChannel.RaiseEvent);
			finishTurnButton.GetComponent<Button>().onClick.AddListener(m_FinishTurnButtonEventChannel.RaiseEvent);
			hackButton.GetComponent<Button>().onClick.AddListener(m_HackButtonEventChannel.RaiseEvent);
			m_BuyEventCardButton.GetComponent<Button>().onClick.AddListener(m_BuyEventCardEventChannel.RaiseEvent);
		}
		
		void Start()
		{
			// m_DefaultGameMenu.SetActive(true);
			m_TradeInitMenu.SetActive(false);
			m_TradeMenu.SetActive(false);
			SetViewsVisible(false);
			DisableMenuButtons();
		}
		
		void UpdateUserInterfaceEventHandler()
		{

		}
		
		void GameMenuStateEventChannelHandler(GameMenuType screen)
		{
			DisableCanvases();
			DisableMenuButtons();
			
			SetViewsVisible(true);
			
			if (screen == GameMenuType.MainMenu)
			{
				SetViewsVisible(false);
			}
			else if (screen == GameMenuType.Hidden)
			{
				SetViewsVisible(false);
			}
			else if (screen == GameMenuType.PlayerTurnScreen)
			{
				finishTurnButton.SetActive(true);
				buildModeButton.SetActive(true);
				tradeButton.SetActive(true);
				cyberActionButton.SetActive(true);
			}
			else if (screen == GameMenuType.PlayerBuildModeScreen)
			{
				cancelButton.SetActive(true);
			}
			else if (screen  == GameMenuType.TradeInitScreen)
			{
				m_TradeInitMenu.SetActive(true);
				cancelButton.SetActive(true);
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
			else if (screen  == GameMenuType.CyberActions)
			{
				// m_CyberActionGameMenu.SetActive(true);
				finishTurnButton.SetActive(true);
				hackButton.SetActive(true);
				m_BuyEventCardButton.SetActive(true);
				cancelButton.SetActive(true);
			}
			else if (screen == GameMenuType.PlayerHackScreen)
			{
				cancelButton.SetActive(true);
			}
			else if (screen == GameMenuType.DepotTrade)
			{
				m_DepotTradeMenu.SetActive(true);
			}
			else if (screen == GameMenuType.AnyDepotTrade)
			{
				m_AnyDepotTradeMeu.SetActive(true);
			}
			
		}
		
		void DisableMenuButtons()
		{
			finishTurnButton.SetActive(false);
			buildModeButton.SetActive(false);
			tradeButton.SetActive(false);
			cyberActionButton.SetActive(false);
			hackButton.SetActive(false);
			m_BuyEventCardButton.SetActive(false);
			cancelButton.SetActive(false);
		}
		
		void DisableCanvases()
		{
			// m_DefaultGameMenu.SetActive(true);
			// m_CyberActionGameMenu.SetActive(false);
			m_TradeInitMenu.SetActive(false);
			m_TradeMenu.SetActive(false);
			m_AnyDepotTradeMeu.SetActive(false);
			m_DepotTradeMenu.SetActive(false);
		}
		
		void SetViewsVisible(bool visible)
		{
			m_PlayersView.SetActive(visible);
			m_PlayerResourceView.SetActive(visible);
		}
		
	}
}