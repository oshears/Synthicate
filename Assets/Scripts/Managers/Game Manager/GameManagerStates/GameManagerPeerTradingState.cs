using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerPeerTradingState : GameManagerAbstractState
	{
		

		
		[Header("Event Channels")]
		
		[SerializeField] GameMenuStateEventChannel m_GameMenuStateEventChannel;
		[SerializeField] EventChannelSO m_TradeCanceledEventChannel;
		[SerializeField] EventChannelSO m_UpdateUiEventChannel;
		// [SerializeField] IntEventChannelSO m_SelectTradePartnerEventChannel;
		[SerializeField] EventChannelSO m_TradeExecutedEventChannel;
		[SerializeField] BoolEventChannelSO m_PeerTradeRequestConfirmedEventChannel;
		[SerializeField] BoolEventChannelSO m_ClientTradeRequestConfirmedEventChannel;
		
		
		
		public override void Enter()
		{
			// _menuState = MenuState.Default;
			// _userInterfaceSO.OnUpdateUserInterface();
			
			// TODO: Need to add the logic for the Trade Init Screen and the Trade Requester / Receiver Screen
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.TradeRequesterScreen);
			
			m_TradeCanceledEventChannel.OnEventRaised += TradeCanceledEventHandler;
			// m_SelectTradePartnerEventChannel.OnEventRaised += SelectTradePartnerEventHandler;
			m_TradeExecutedEventChannel.OnEventRaised += TradeExecutedEventHandler;
			m_ClientTradeRequestConfirmedEventChannel.OnEventRaised += ClientTradeRequestConfirmedEventChannel;
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			changeState(_owner.pendingState);
		}
		
		void TradeCanceledEventHandler()
		{
			_owner.tradingState.PeerCancelTradeServerRpc();
			changeState(_owner.pendingSetupState);
		}
		
		
		void TradeExecutedEventHandler()
		{
			changeState(_owner.pendingState);
		}
		
		void ClientTradeRequestConfirmedEventChannel(bool confirmed)
		{
			_owner.tradingState.PeerTradeRequestConfirmedServerRpc(confirmed);
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
			changeState(_owner.pendingState);
		}


	}
}