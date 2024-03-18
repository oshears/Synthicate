using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;

namespace Synthicate
{
	public class UserInterfaceTradeInitWindow : NetworkBehaviour
	{
		
		[SerializeField]
		UiPlayerSelectButton[] m_PlayerButtons;
		
		[SerializeField]
		TextMeshProUGUI m_TitleText;
		
		[SerializeField]
		TextMeshProUGUI m_DescriptionText;
		
		[Header("Event Channels")]
		
		[SerializeField]
		TradeInitEventChannel m_InitiateTradeEventChannel;
		
		[SerializeField]
		EventChannelSO m_SelectTradePartnerEventChannel;
		
		[Header("Scriptable Objects")]
		
		[SerializeField]
		UiScriptableObject m_UiScriptableObject;
		GameManagerSO m_GameManagerSO;
		
		
		public void Start()
		{
			m_InitiateTradeEventChannel.OnEventRaised += InitiateTradeEventHandler;
			DisableAllButtons();
		}

		public void InitiateTradeEventHandler(TradeInitWindowType windowType)
		{
			DisableAllButtons();
			
			if (windowType == TradeInitWindowType.Hacking)
			{
				m_TitleText.text = "Initiate Hack";
				m_DescriptionText.text = "Who would you like to hack?";
			}
			else
			{
				m_TitleText.text = "Trade Request";
				m_DescriptionText.text = "Who would you like to trade with?";
			}
			
			for(int i = 0; i < m_GameManagerSO.playerList.Count; i++)
			{
				if (m_GameManagerSO.clientPlayer.GetId() != m_GameManagerSO.playerList[i].GetId())
				{
					m_PlayerButtons[i].gameObject.SetActive(true);
					m_PlayerButtons[i].SetPlayerName(m_GameManagerSO.playerList[i].GetName());
				}
			}
			
		}
		
		public void DisableAllButtons()
		{
			for(int i = 0; i < m_PlayerButtons.Length; i++)
			{
				m_PlayerButtons[i].gameObject.SetActive(false);
			}
		}
	}
}