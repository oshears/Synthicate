using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;
using Mono.Cecil;

namespace Synthicate
{
	public class UserInterfaceDepotTradeWindow : NetworkBehaviour
	{
		
		[Header("Trade Increment / Decrement Buttons")]
		
		[SerializeField]
		UserInterfaceIncrementer[] depotResourceIncrementers;
		
		[SerializeField]
		TextMeshProUGUI tradeOfferAmount;
		
		[Header("Buttons")]
		
		[SerializeField]
		Button m_ClientTradeConfirmedButton;

		[SerializeField]
		Button m_CancelTradeButton;
		
		[Header("Error Text")]
		
		[SerializeField]
		TextMeshProUGUI m_InvalidTradeText;
		
		[Header("Depot Resource Icon")]
		
		[SerializeField]
		Image m_DepotResourceImage;
		
		[Header("Event Channels")]
		
		[SerializeField]
		EventChannelSO m_CancelTradeEventChannel;
		
		[SerializeField]
		DepotSelectedEventChannelSO m_DepotSelectedEventChannel;
		
		[Header("Scriptable Objects")]
		
		[SerializeField]
		UiScriptableObject m_UserInterfaceScriptableObject;
		
		[SerializeField]
		GameManagerSO m_GameManagerSO;
		
		int m_GivingAmount;
		int[] m_ReceivingAmounts;
		
		ResourceType m_TradeResource;
		int m_RequiredAmount;
		
		void Awake() {
			m_ReceivingAmounts = new int[]{0, 0, 0, 0, 0};
			m_DepotSelectedEventChannel.OnEventRaised += DepotSelectedEventHandler;
			
			foreach(UserInterfaceIncrementer incr in depotResourceIncrementers)
			{
				incr.e_AmountChanged += IncrementerAmountChanged;
			}
		}
		
		void OnEnable()
		{
			ResetCounts();
		}
		
		
		void Start()
		{
			
			m_ClientTradeConfirmedButton.onClick.AddListener(() => 
			{
				// ResetCounts();
				Debug.Log($"Giving: {m_GivingAmount}");
				Debug.Log($"Receiving: {m_ReceivingAmounts}");
				
				if (m_TradeResource != ResourceType.Any)
				{
					int totalRequiredAmount = 0;
					
					foreach (int receivingAmount in m_ReceivingAmounts)
					{
						totalRequiredAmount += receivingAmount;
					}
					
					if (m_GameManagerSO.clientPlayer.resources[(int) m_TradeResource] >= totalRequiredAmount)
					{
						m_GameManagerSO.clientPlayer.resources[(int) m_TradeResource] -= totalRequiredAmount;
						
						for(int i = 0; i < m_ReceivingAmounts.Length; i++)
						{
							m_GameManagerSO.clientPlayer.resources[i] += m_ReceivingAmounts[i];
						}
					}
					else
					{
						Debug.Log("Client does not have enough resources for this trade!");
					}
				}
				
				m_InvalidTradeText.text = "Insufficient Resources!";
			});
			
			m_CancelTradeButton.onClick.AddListener(() => {
				ResetCounts();
				m_CancelTradeEventChannel.RaiseEvent();
			});
			
			
		}
		
		void IncrementerAmountChanged()
		{
			m_GivingAmount = 0;
			for(int i = 0; i < depotResourceIncrementers.Length; i++)
			{
				m_ReceivingAmounts[i] = depotResourceIncrementers[i].GetAmount();
				
				m_GivingAmount += m_ReceivingAmounts[i] * 2;
			}
		}
		
		void DepotSelectedEventHandler(DepotSelection selection)
		{
			m_TradeResource = selection.Resource;
			m_RequiredAmount = selection.RequiredAmount;
			ResetCounts();
		}

		
		void ResetCounts()
		{
			m_InvalidTradeText.text = "";
			
			m_GivingAmount = 0;
			m_ReceivingAmounts = new int[]{0, 0, 0, 0, 0};
			
			for(int i = 0; i < depotResourceIncrementers.Length; i++)
			{
				depotResourceIncrementers[i].ResetAmount();
			}
		}
	}
}