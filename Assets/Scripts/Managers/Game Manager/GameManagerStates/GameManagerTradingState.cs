using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerTradingState : GameManagerAbstractState
	{
		

		
		[Header("Event Channels")]
		
		[SerializeField] GameMenuStateEventChannel m_GameMenuStateEventChannel;
		[SerializeField] EventChannelSO m_TradeCanceledEventChannel;
		[SerializeField] EventChannelSO m_CancelButtonEventChannel;
		[SerializeField] EventChannelSO m_UpdateUiEventChannel;
		[SerializeField] IntEventChannelSO m_SelectTradePartnerEventChannel;
		[SerializeField] EventChannelSO m_TradeExecutedEventChannel;
		[SerializeField] EventChannelSO m_PeerTradeRequestConfirmedEventChannel;
		[SerializeField] EventChannelSO m_ClientTradeRequestConfirmedEventChannel;
		[SerializeField] IntArrayEventChannelSO m_ClientTradeAmountsUpdatedEventChannel;
		[SerializeField] IntArrayEventChannelSO m_PeerTradeAmountsUpdatedEventChannel;
		
		
		
		public override void Enter()
		{
			// _menuState = MenuState.Default;
			
			// TODO: Need to add the logic for the Trade Init Screen and the Trade Requester / Receiver Screen
			// m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.TradeRequesterScreen);
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.TradeInitScreen);
			
			m_TradeCanceledEventChannel.OnEventRaised += TradeCanceledEventHandler;
			m_SelectTradePartnerEventChannel.OnEventRaised += SelectTradePartnerEventHandler;
			m_TradeExecutedEventChannel.OnEventRaised += TradeExecutedEventHandler;
			m_ClientTradeRequestConfirmedEventChannel.OnEventRaised += ClientTradeRequestConfirmedEventHandler;
			m_ClientTradeAmountsUpdatedEventChannel.OnEventRaised += ClientTradeAmountsUpdatedEventHandler;
			m_CancelButtonEventChannel.OnEventRaised += CancelButtonEventHandler;
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			m_TradeCanceledEventChannel.OnEventRaised -= TradeCanceledEventHandler;
			m_SelectTradePartnerEventChannel.OnEventRaised -= SelectTradePartnerEventHandler;
			m_TradeExecutedEventChannel.OnEventRaised -= TradeExecutedEventHandler;
			m_ClientTradeRequestConfirmedEventChannel.OnEventRaised -= ClientTradeRequestConfirmedEventHandler;
			m_ClientTradeAmountsUpdatedEventChannel.OnEventRaised -= ClientTradeAmountsUpdatedEventHandler;
		}
		
		void CancelButtonEventHandler()
		{
			changeState(_owner.idleState);
		}
		
		void TradeCanceledEventHandler()
		{
			_owner.m_PeerTradingState.PeerCancelTradeServerRpc();
			changeState(_owner.idleState);
		}

		void SelectTradePartnerEventHandler(int targetClientId)
		{
			_owner.pendingState.InitiateTradeRequestServerRpc(targetClientId);
			
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.TradeRequesterScreen);
		}

		void TradeExecutedEventHandler()
		{
			changeState(_owner.idleState);
		}
		
		void ClientTradeRequestConfirmedEventHandler()
		{
			_owner.m_PeerTradingState.PeerTradeRequestConfirmedServerRpc();
		}
		
		void ClientTradeAmountsUpdatedEventHandler(int[] resources)
		{
			_owner.m_PeerTradingState.UpdatePeerAmountsServerRpc(resources);
		}
		
		[ServerRpc(RequireOwnership = false)]
		public void UpdatePeerAmountsServerRpc(int[] resources) => UpdatePeerAmountsClientRpc(resources);
		
		[ClientRpc]
		public void UpdatePeerAmountsClientRpc(int[] resources)
		{
			if(!IsActiveState()) return;
			m_PeerTradeAmountsUpdatedEventChannel.RaiseEvent(resources);
		}
		
		[ServerRpc(RequireOwnership = false)]
		public void PeerTradeRequestConfirmedServerRpc() => PeerTradeRequestConfirmedClientRpc();
		
		[ClientRpc]
		public void PeerTradeRequestConfirmedClientRpc()
		{
			if(!IsActiveState()) return;
			m_PeerTradeRequestConfirmedEventChannel.RaiseEvent();
		}
		
		[ServerRpc(RequireOwnership = false)]
		public void PeerCancelTradeServerRpc() => PeerCancelTradeClientRpc();
		
		[ClientRpc]
		public void PeerCancelTradeClientRpc()
		{
			if(!IsActiveState()) return;
			changeState(_owner.idleState);
		}

	}
}