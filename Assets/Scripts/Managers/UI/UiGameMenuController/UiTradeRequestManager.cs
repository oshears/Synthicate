using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Synthicate
{
	public class UiTradeRequestManager : UiUpdatableElementMonoBehavior
	{
		// [SerializeField] // GameObject[] initiateTradePlayerButtons, initiateTradePlayerIcons;

		// [SerializeField] // GameObject[] tradeIncrementButtons, tradeDecrementButtons, tradeOfferAmounts, peerTradeAmounts;
		
		[Header("Resource Incrementers")]
		
		[SerializeField] UserInterfaceIncrementer[] m_ResourceIncrementers;
		
		[Header("Invalid Trade Text")]
		[SerializeField] TextMeshProUGUI m_InvalidTradeText;
		
		[Header("Confirmed Icons")]

		[SerializeField] Image m_PeerTradeConfirmedIcon;
		
		[SerializeField] Image m_ClientTradeConfirmedIcon;
		
		[Header("Buttons")]
		
		[SerializeField] Button m_ClientTradeConfirmedButton;

		[SerializeField] Button m_CancelTradeButton;
		
		[Header("Event Channels")]
		
		[SerializeField] EventChannelSO m_CancelTradeEventChannel;
		[SerializeField] BoolEventChannelSO m_PeerTradeRequestConfirmedEventChannel;
		[SerializeField] BoolEventChannelSO m_ClientTradeRequestConfirmedEventChannel;
		[SerializeField] EventChannelSO m_TradeExecutedEventChannel;
		
		
		[Header("Scriptable Object")]
		
		[SerializeField] GameManagerSO m_GameManagerSO;
		
		
		int[] m_GivingAmounts;
		int[] m_ReceivingAmounts;
		
		override protected void Awake() {
			base.Awake();
			
			m_GivingAmounts = new int[]{0, 0, 0, 0, 0};
			m_ReceivingAmounts = new int[]{0, 0, 0, 0, 0};
			
			foreach(UserInterfaceIncrementer incr in m_ResourceIncrementers)
			{
				incr.e_AmountChanged += IncrementerAmountChanged;
			}
		}
		
		
		
		void Start()
		{
			
			
			// // Increment Buttons
			// for(int i = 0; i < tradeIncrementButtons.Length; i++)
			// {
			// 	IncrementButtonEventSetup(i);
			// }
			
			// // Decrement Buttons
			// for(int i = 0; i < tradeDecrementButtons.Length; i++)
			// {
			// 	DecrementButtonEventSetup(i);
			// }
			
			m_ClientTradeConfirmedButton.onClick.AddListener(() => {
				// m_InvalidTradeText.text = "Insufficient Resources!";
				Debug.Log("Confirm Clicked!");
				
			});
			
			m_CancelTradeButton.onClick.AddListener(() => {
				ResetCounts();
				m_CancelTradeEventChannel.RaiseEvent();
			});
		}
		
		
		override protected void InitilizeUserInterfaceEventHandler()
		{
			
		}
		
		override protected void UpdateUserInterfaceEventHandler()
		{
			
		}
		
		void IncrementerAmountChanged()
		{
			
		}
		
		void ResetCounts()
		{
			m_InvalidTradeText.text = "";
			
			m_GivingAmounts = new int[]{0, 0, 0, 0, 0};
			m_ReceivingAmounts = new int[]{0, 0, 0, 0, 0};
			
			for(int i = 0; i < m_ResourceIncrementers.Length; i++)
			{
				// tradeOfferAmounts[i].GetComponent<TextMeshProUGUI>().text = "0";
				// peerTradeAmounts[i].GetComponent<TextMeshProUGUI>().text = "0";
				m_ResourceIncrementers[i].ResetAmount();
			}
		}
		
	}
}