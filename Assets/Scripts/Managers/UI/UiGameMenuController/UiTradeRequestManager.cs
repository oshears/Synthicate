using System.Collections;
using System.Collections.Generic;
using Codice.Client.GameUI.Checkin;
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
		
		[Header("Peer Resources")]
		
		[SerializeField] TextMeshProUGUI[] m_PeerResources;
		
		[Header("Invalid Trade Text")]
		[SerializeField] TextMeshProUGUI m_InvalidTradeText;
		
		[Header("Confirmed Icons")]

		[SerializeField] GameObject m_PeerTradeConfirmedIcon;
		
		[SerializeField] GameObject m_ClientTradeConfirmedIcon;
		
		[Header("Buttons")]
		
		[SerializeField] Button m_ClientTradeConfirmedButton;

		[SerializeField] Button m_CancelTradeButton;
		
		[Header("Event Channels")]
		
		[SerializeField] EventChannelSO m_CancelTradeEventChannel;
		[SerializeField] EventChannelSO m_PeerTradeRequestConfirmedEventChannel;
		[SerializeField] EventChannelSO m_ClientTradeRequestConfirmedEventChannel;
		[SerializeField] EventChannelSO m_TradeExecutedEventChannel;
		[SerializeField] IntArrayEventChannelSO m_ClientTradeAmountsUpdatedEventChannel;
		[SerializeField] IntArrayEventChannelSO m_PeerTradeAmountsUpdatedEventChannel;
		
		
		[Header("Scriptable Object")]
		
		[SerializeField] GameManagerSO m_GameManagerSO;
		
		
		int[] m_GivingAmounts;
		int[] m_ReceivingAmounts;
		
		enum TradeState
		{
			NoneConfirmed,
			ClientConfirmed,
			PeerConfirmed,
			TradeExecuted
		}
		
		TradeState m_TradeState = TradeState.NoneConfirmed;
		
		override protected void Awake() {
			base.Awake();
			
		}
		
		void OnEnable()
		{
			foreach(UserInterfaceIncrementer incr in m_ResourceIncrementers)
			{
				incr.e_AmountChanged += IncrementerAmountChanged;
			}
			
			foreach(TextMeshProUGUI textObject in m_PeerResources) textObject.text = "0";
			
			m_ClientTradeConfirmedButton.onClick.AddListener(TradeConfirmedButtonClick);
			m_CancelTradeButton.onClick.AddListener(TradeCanceledButtonClick);
			m_PeerTradeRequestConfirmedEventChannel.OnEventRaised += PeerTradeRequestConfirmedEventHandler ;
			m_PeerTradeAmountsUpdatedEventChannel.OnEventRaised += PeerTradeAmountsUpdatedEventHandler ;
			
			m_InvalidTradeText.text = "";
			
			m_GivingAmounts = new int[]{0, 0, 0, 0, 0};
			m_ReceivingAmounts = new int[]{0, 0, 0, 0, 0};
			
			ChangeState(TradeState.NoneConfirmed);
		}
		
		void OnDisable()
		{
			m_ClientTradeConfirmedButton.onClick.RemoveListener(TradeConfirmedButtonClick);
			m_CancelTradeButton.onClick.RemoveListener(TradeCanceledButtonClick);
			m_PeerTradeRequestConfirmedEventChannel.OnEventRaised -= PeerTradeRequestConfirmedEventHandler ;
			m_PeerTradeAmountsUpdatedEventChannel.OnEventRaised -= PeerTradeAmountsUpdatedEventHandler ;
		}
		
		
		
		void Start()
		{
			
			
		}
		
		void ChangeState(TradeState m_NewState)
		{
			m_TradeState = m_NewState;
			
			if (m_NewState == TradeState.NoneConfirmed)
			{
				SetClientConfirmedIconVisible(false);
				SetPeerConfirmedIconVisible(false);
			}
			else if (m_NewState == TradeState.ClientConfirmed)
			{
				m_InvalidTradeText.text = "";
				SetClientConfirmedIconVisible(true);
				SetPeerConfirmedIconVisible(false);
			}
			else if (m_NewState == TradeState.PeerConfirmed)
			{
				SetClientConfirmedIconVisible(false);
				SetPeerConfirmedIconVisible(true);
			}
			else if (m_NewState == TradeState.TradeExecuted)
			{
				m_InvalidTradeText.text = "";
				SetClientConfirmedIconVisible(false);
				SetPeerConfirmedIconVisible(false);
			}
		}
		
		void TradeConfirmedButtonClick()
		{
			Debug.Log("Trade Confirm Clicked!");
			
			if (m_GameManagerSO.clientPlayer.HasSufficientResources(m_GivingAmounts))
			{
				m_ClientTradeRequestConfirmedEventChannel.RaiseEvent();
				
				if (m_TradeState == TradeState.PeerConfirmed)
				{
					ExecuteTrade();
				}
				else
				{
					ChangeState(TradeState.ClientConfirmed);
				}
			}
			else
			{
				m_InvalidTradeText.text = "Insufficient Resources!";
			}
			
		}
		
		void TradeCanceledButtonClick()
		{
			ResetCounts();
			m_CancelTradeEventChannel.RaiseEvent();
		}
		
		void ExecuteTrade()
		{
			m_GameManagerSO.clientPlayer.RemoveResources(m_GivingAmounts);
			m_GameManagerSO.clientPlayer.AddResources(m_ReceivingAmounts);
			m_TradeExecutedEventChannel.RaiseEvent();
			ChangeState(TradeState.TradeExecuted);
		}
		
		
		override protected void InitilizeUserInterfaceEventHandler()
		{
			
		}
		
		override protected void UpdateUserInterfaceEventHandler()
		{
			
		}
		
		void IncrementerAmountChanged()
		{
			// Update giving amounts array
			for(int i = 0; i < m_ResourceIncrementers.Length; i++)
			{
				m_GivingAmounts[i] = m_ResourceIncrementers[i].GetAmount();
			}
			
			// Tell the peer that the client amounts have changed and provide the new values
			m_ClientTradeAmountsUpdatedEventChannel.RaiseEvent(m_GivingAmounts);
			
			// Clear the confirmed state (for both peers)
			ChangeState(TradeState.NoneConfirmed);
		}
		
		void ResetWindow()
		{
			ChangeState(TradeState.NoneConfirmed);
			m_InvalidTradeText.text = "";
			ResetCounts();
		}
		
		void ResetCounts()
		{
			m_GivingAmounts = new int[]{0, 0, 0, 0, 0};
			m_ReceivingAmounts = new int[]{0, 0, 0, 0, 0};
			
			for(int i = 0; i < m_ResourceIncrementers.Length; i++)
			{
				m_ResourceIncrementers[i].ResetAmount();
			}
		}
		
		void SetClientConfirmedIconVisible(bool visible)
		{
			m_ClientTradeConfirmedIcon.SetActive(visible);
		}
		
		void SetPeerConfirmedIconVisible(bool visible)
		{
			m_PeerTradeConfirmedIcon.SetActive(visible);
		}
		
		void PeerTradeRequestConfirmedEventHandler()
		{
			if (m_TradeState == TradeState.ClientConfirmed)
			{
				ExecuteTrade();
			}
			else
			{
				ChangeState(TradeState.PeerConfirmed);
			}
		}
		
		void PeerTradeAmountsUpdatedEventHandler(int[] peerAmounts)
		{
			for(int i = 0; i < m_ResourceIncrementers.Length; i++)
			{
				m_ReceivingAmounts[i] = peerAmounts[i];
				m_PeerResources[i].text = $"{peerAmounts[i]}";
			}
			
			ChangeState(TradeState.NoneConfirmed);
		}
	}
}