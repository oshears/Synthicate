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
		[SerializeField] EventChannelSO m_UpdateUiEventChannel;
		[SerializeField] IntEventChannelSO m_SelectTradePartnerEventChannel;
		[SerializeField] EventChannelSO m_TradeExecutedEventChannel;
		[SerializeField] BoolEventChannelSO m_PeerTradeRequestConfirmedEventChannel;
		[SerializeField] BoolEventChannelSO m_ClientTradeRequestConfirmedEventChannel;
		
		
		
		public override void Enter()
		{
			// _menuState = MenuState.Default;
			// _userInterfaceSO.OnUpdateUserInterface();
			
			// TODO: Need to add the logic for the Trade Init Screen and the Trade Requester / Receiver Screen
			// m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.TradeRequesterScreen);
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.TradeInitScreen);
			
			m_TradeCanceledEventChannel.OnEventRaised += TradeCanceledEventHandler;
			m_SelectTradePartnerEventChannel.OnEventRaised += SelectTradePartnerEventHandler;
			m_TradeExecutedEventChannel.OnEventRaised += TradeExecutedEventHandler;
			m_ClientTradeRequestConfirmedEventChannel.OnEventRaised += ClientTradeRequestConfirmedEventChannel;
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			m_TradeCanceledEventChannel.OnEventRaised -= TradeCanceledEventHandler;
			m_SelectTradePartnerEventChannel.OnEventRaised -= SelectTradePartnerEventHandler;
			m_TradeExecutedEventChannel.OnEventRaised -= TradeExecutedEventHandler;
			m_ClientTradeRequestConfirmedEventChannel.OnEventRaised -= ClientTradeRequestConfirmedEventChannel;
			
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
		
		void ClientTradeRequestConfirmedEventChannel(bool confirmed)
		{
			_owner.m_PeerTradingState.PeerTradeRequestConfirmedServerRpc(confirmed);
		}
		
		[ServerRpc(RequireOwnership = false)]
		public void PeerTradeRequestConfirmedServerRpc(bool confirmed) => PeerTradeRequestConfirmedClientRpc(confirmed);
		
		[ClientRpc]
		public void PeerTradeRequestConfirmedClientRpc(bool confirmed)
		{
			if(!IsActiveState()) return;
			m_PeerTradeRequestConfirmedEventChannel.RaiseEvent(confirmed);
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