using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;

namespace Synthicate
{
	public class UserInterfaceTradeInitWindow : MonoBehaviour
	{
		
		[SerializeField]
		UiPlayerSelectButton[] m_PlayerButtons;
		
		[SerializeField]
		GameObject[] m_PlayerButtonCanvas;
		
		[SerializeField]
		TextMeshProUGUI m_TitleText;
		
		[SerializeField]
		TextMeshProUGUI m_DescriptionText;
		
		[Header("Event Channels")]
		
		[SerializeField]
		TradeInitEventChannel m_InitiateTradeEventChannel;
		
		[SerializeField]
		IntEventChannelSO m_SelectTradePartnerEventChannel;
		
		[Header("Scriptable Objects")]
		
		[SerializeField]
		UiScriptableObject m_UiScriptableObject;
		
		[SerializeField]
		GameManagerSO m_GameManagerSO;
		
		public void Awake()
		{
			m_InitiateTradeEventChannel.OnEventRaised += InitiateTradeEventHandler;
		}
		
		public void OnEnable()
		{
			DisableAllButtons();
			
			for(int i = 0; i < m_PlayerButtons.Length; i++)
			{
				SetupButton(i);
			}
		}
		
		public void OnDisable()
		{
			for(int i = 0; i < m_PlayerButtons.Length; i++)
			{
				m_PlayerButtons[i].m_Button.onClick.RemoveAllListeners();
			}
		}
		
		public void Start()
		{
			
		}
		
		public void SetupButton(int i)
		{
			m_PlayerButtons[i].m_Button.onClick.AddListener(() => {  
				m_SelectTradePartnerEventChannel.RaiseEvent(i);
			});
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
					m_PlayerButtonCanvas[i].SetActive(true);
					m_PlayerButtons[i].SetPlayerName(m_GameManagerSO.playerList[i].GetName());
				}
			}
			
			// DEBUG: Just for testing
			for(int i = 0; i < 3; i++)
			{
				Debug.Log($"Enabling Trade Init Player: {i}");
				if (m_PlayerButtonCanvas[i].activeSelf == false)
				{
					m_PlayerButtonCanvas[i].SetActive(true);
					m_PlayerButtons[i].SetPlayerName($"Dummy Player {i}");
				}
			}
			// END DEBUG:
			
		}
		
		public void DisableAllButtons()
		{
			for(int i = 0; i < m_PlayerButtons.Length; i++)
			{
				m_PlayerButtonCanvas[i].SetActive(false);
			}
		}
	}
}