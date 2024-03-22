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
		// [SerializeField]
		// GameObject[] initiateTradePlayerButtons, initiateTradePlayerIcons;

		// [SerializeField]
		// GameObject[] tradeIncrementButtons, tradeDecrementButtons, tradeOfferAmounts, peerTradeAmounts;
		
		[Header("Resource Incrementers")]
		
		[SerializeField]
		UserInterfaceIncrementer[] m_ResourceIncrementers;
		
		[Header("Confirmed Icons")]

		[SerializeField]
		GameObject m_PeerTradeConfirmedIcon;
		
		[SerializeField]
		GameObject m_ClientTradeConfirmedIcon;
		
		[Header("Buttons")]
		
		[SerializeField]
		GameObject m_ClientTradeConfirmedButton;

		[SerializeField]
		GameObject m_CancelTradeButton;
		
		[Header("Event Channels")]
		
		[SerializeField]
		EventChannelSO m_CancelTradeEventChannel;
		
		int[] m_GivingAmounts;
		int[] m_ReceivingAmounts;
		
		override protected void Awake() {
			base.Awake();
			
			m_GivingAmounts = new int[]{0, 0, 0, 0, 0};
			m_ReceivingAmounts = new int[]{0, 0, 0, 0, 0};
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
			
			m_CancelTradeButton.GetComponent<Button>().onClick.AddListener(() => {
				ResetCounts();
				m_CancelTradeEventChannel.RaiseEvent();
			});
		}
		
		
		void IncrementButtonEventSetup(int value)
		{
			// tradeIncrementButtons[value].GetComponent<Button>().onClick.AddListener(() => {
			// 	m_GivingAmounts[value] += 1;
			// 	tradeOfferAmounts[value].GetComponent<TextMeshProUGUI>().text = $"{m_GivingAmounts[value]}";
			// });
		
		}
		
		void DecrementButtonEventSetup(int value)
		{
			// tradeDecrementButtons[value].GetComponent<Button>().onClick.AddListener(() => {
			// 	m_GivingAmounts[value] -= (m_GivingAmounts[value] > 0) ? 1 : 0;
			// 	tradeOfferAmounts[value].GetComponent<TextMeshProUGUI>().text = $"{m_GivingAmounts[value]}";
			// });
		}
		
		
		override protected void InitilizeUserInterfaceEventHandler()
		{
			
		}
		
		override protected void UpdateUserInterfaceEventHandler()
		{
			
		}
		
		void ResetCounts()
		{
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