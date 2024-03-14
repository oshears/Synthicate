using System.Collections;
using System.Collections.Generic;
using PlasticPipe;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Synthicate
{
	public class UiTradeRequestManager : UiUpdatableElementMonoBehavior
	{
		[SerializeField]
		GameObject[] initiateTradePlayerButtons, initiateTradePlayerIcons;

		[SerializeField]
		GameObject[] tradeIncrementButtons, tradeDecrementButtons, tradeOfferAmounts, peerTradeAmounts;

		[SerializeField]
		GameObject m_PeerTradeConfirmedIcon;
		
		[SerializeField]
		GameObject m_ClientTradeConfirmedIcon;
		
		[SerializeField]
		GameObject m_ClientTradeConfirmedButton;

		[SerializeField]
		GameObject m_CancelTradeButton;
		
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
			
			
			for(int i = 0; i < tradeIncrementButtons.Length; i++)
			{
				// Increment Buttons
				tradeIncrementButtons[i].GetComponent<Button>().onClick.AddListener(() => {
					m_GivingAmounts[i] += 1;
					tradeOfferAmounts[i].GetComponent<TextMeshProUGUI>().text = $"{m_GivingAmounts[i]}";
				});
				// Decrement Buttons
				tradeIncrementButtons[i].GetComponent<Button>().onClick.AddListener(() => {
					m_GivingAmounts[i] -= (m_GivingAmounts[i] > 0) ? 1 : 0;
					tradeOfferAmounts[i].GetComponent<TextMeshProUGUI>().text = $"{m_GivingAmounts[i]}";
				});
			}
			
			m_CancelTradeButton.GetComponent<Button>().onClick.AddListener(() => {
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
		
		void ResetCounts()
		{
			m_GivingAmounts = new int[]{0, 0, 0, 0, 0};
			m_ReceivingAmounts = new int[]{0, 0, 0, 0, 0};
			
			for(int i = 0; i < tradeIncrementButtons.Length; i++)
			{
				tradeOfferAmounts[i].GetComponent<TextMeshProUGUI>().text = "0";
				peerTradeAmounts[i].GetComponent<TextMeshProUGUI>().text = "0";
			}
		}
		
	}
}