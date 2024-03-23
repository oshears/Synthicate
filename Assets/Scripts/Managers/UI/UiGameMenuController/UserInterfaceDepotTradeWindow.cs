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
	public class UserInterfaceDepotTradeWindow : MonoBehaviour
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
		
		[SerializeField]
		EventChannelSO m_TradeExecutedEventChannel;
		
		[Header("Scriptable Objects")]
		
		[SerializeField]
		UiScriptableObject m_UserInterfaceScriptableObject;
		
		[SerializeField]
		GameManagerSO m_GameManagerSO;
		
		int m_GivingAmount;
		int[] m_ReceivingAmounts;
		
		ResourceType m_TradeResource;
		
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
			m_ClientTradeConfirmedButton.onClick.AddListener(ClientTradeConfirmedButtonEventHandler);
			m_CancelTradeButton.onClick.AddListener(CancelTradeButtonEventHandler);
		}
		
		void CancelTradeButtonEventHandler()
		{
			ResetCounts();
			m_CancelTradeEventChannel.RaiseEvent();
		}
		
		void ClientTradeConfirmedButtonEventHandler()
		{
			// ResetCounts();
			Debug.Log($"Giving: {m_GivingAmount}");
			Debug.Log($"Receiving: {m_ReceivingAmounts}");
			
			// If trading at a standard depot
			if (m_TradeResource != ResourceType.Any)
			{
				// Calculate total number of resources a player must give
				// int totalRequiredAmount = 0;
				// foreach (int receivingAmount in m_ReceivingAmounts)
				// {
				// 	totalRequiredAmount += receivingAmount;
				// }
				
				// Check that the player has the required number of resources for the trade
				if (m_GameManagerSO.clientPlayer.resources[(int) m_TradeResource] >= m_GivingAmount)
				{
					
					// Remove the resources from the player's inventory
					Debug.Log("Requested Resource: " + m_TradeResource);
					m_GameManagerSO.clientPlayer.RemoveResources(m_TradeResource,m_GivingAmount);
					
					// Give the new resources to the player
					m_GameManagerSO.clientPlayer.AddResources(m_ReceivingAmounts);
					
					// Raise trade executed event
					ResetCounts();
					m_TradeExecutedEventChannel.RaiseEvent();
				}
				else
				{
					Debug.Log("Client does not have enough resources for this trade!");
					m_InvalidTradeText.text = "Insufficient Resources!";
				}
			}
			
			m_InvalidTradeText.text = "Insufficient Resources!";
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
			if (selection.Resource == ResourceType.Any) return;
			
			ResetCounts();
			m_TradeResource = selection.Resource;
			tradeOfferAmount.text = $"{selection.RequiredAmount}";
			m_DepotResourceImage.sprite = m_UserInterfaceScriptableObject.ResourceSprites[(int) selection.Resource];
		}

		
		void ResetCounts()
		{
			m_InvalidTradeText.text = "";
			tradeOfferAmount.text = "";
			
			m_GivingAmount = 0;
			m_ReceivingAmounts = new int[]{0, 0, 0, 0, 0};
			
			for(int i = 0; i < depotResourceIncrementers.Length; i++)
			{
				depotResourceIncrementers[i].ResetAmount();
			}
		}
	}
}