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
		[SerializeField] EventChannelSO m_PeerTradeRequestConfirmedEventChannel;
		[SerializeField] EventChannelSO m_ClientTradeRequestConfirmedEventChannel;
		[SerializeField] IntArrayEventChannelSO m_ClientTradeAmountsUpdatedEventChannel;
		[SerializeField] IntArrayEventChannelSO m_PeerTradeAmountsUpdatedEventChannel;
		
		
		
		public override void Enter()
		{
			// _menuState = MenuState.Default;
			// _userInterfaceSO.OnUpdateUserInterface();
			
			// TODO: Need to add the logic for the Trade Init Screen and the Trade Requester / Receiver Screen
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.TradeRequesterScreen);
			
			m_TradeCanceledEventChannel.OnEventRaised += TradeCanceledEventHandler;
			m_TradeExecutedEventChannel.OnEventRaised += TradeExecutedEventHandler;
			m_ClientTradeRequestConfirmedEventChannel.OnEventRaised += ClientTradeRequestConfirmedEventChannel;
			m_ClientTradeAmountsUpdatedEventChannel.OnEventRaised += ClientTradeAmountsUpdatedEventHandler;
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			m_TradeCanceledEventChannel.OnEventRaised -= TradeCanceledEventHandler;
			m_TradeExecutedEventChannel.OnEventRaised -= TradeExecutedEventHandler;
			m_ClientTradeRequestConfirmedEventChannel.OnEventRaised -= ClientTradeRequestConfirmedEventChannel;
			m_ClientTradeAmountsUpdatedEventChannel.OnEventRaised -= ClientTradeAmountsUpdatedEventHandler;
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
		
		void ClientTradeRequestConfirmedEventChannel()
		{
			_owner.tradingState.PeerTradeRequestConfirmedServerRpc();
		}
		
		void ClientTradeAmountsUpdatedEventHandler(int[] resources)
		{
			_owner.tradingState.UpdatePeerAmountsServerRpc(resources);
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
			changeState(_owner.pendingState);
		}


	}
}