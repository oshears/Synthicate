using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerHackingState : GameManagerAbstractState
	{
		
		[Header("Event Channels")]
		
		[SerializeField] GameMenuStateEventChannel m_GameMenuStateEventChannel;
		
		[SerializeField] StringEventChannel m_NotificationEventChannel;
		
		[SerializeField] StringEventChannel m_LocalNotificationEvent;
		
		[SerializeField] TradeInitEventChannel m_InitiateTradeEventChannel;
		
		[SerializeField] IntEventChannelSO m_SelectTradePartnerEventChannel;
		
		[SerializeField] EventChannelSO m_CancelButtonEventChannel;
		[SerializeField] EventChannelSO m_UpdateUiEventChannel;
		
		
		enum HackState
		{
			HexSelect,
			TargetSelect
		}
		HackState m_State;
		
		int m_HackedPlayer;
		
		public override void Enter()
		{
			
			m_NotificationEventChannel.RaiseEvent($"{_gameManagerSO.clientPlayer.GetName()} is selecting a territory to hack...");
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerHackScreen);
			
			_hexManagerSO.BeginHackModeEvent.Invoke();
			
			m_SelectTradePartnerEventChannel.OnEventRaised += SelectTradePartnerEventHandler;
			_hexManagerSO.hackEvent.AddListener(HackEventHandler);
			
			m_CancelButtonEventChannel.OnEventRaised += CancelButtonEventHandler;
			
			m_State = HackState.HexSelect;
			
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			_hexManagerSO.hackEvent.RemoveListener(HackEventHandler);
			m_SelectTradePartnerEventChannel.OnEventRaised -= SelectTradePartnerEventHandler;
			m_CancelButtonEventChannel.OnEventRaised -= CancelButtonEventHandler;
		}

		public void HackEventHandler(uint selectedHex)
		{
			// UpdateHexStatesServerRpc(selectedHex);
			m_NotificationEventChannel.RaiseEvent($"{_gameManagerSO.clientPlayer.GetName()} hacked a territory!");
			m_UpdateUiEventChannel.RaiseEvent();
			_hexManagerSO.EndHackModeEvent.Invoke();
			// changeState(_owner.idleState);
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerDiceHackTargetScreen);
			m_InitiateTradeEventChannel.RaiseEvent(TradeInitWindowType.Hacking);
			
			m_State = HackState.TargetSelect;
		}
		
		public void SelectTradePartnerEventHandler(int selectedPlayer)
		{
			// Save hacked player id
			m_HackedPlayer = selectedPlayer;
			
			// Decrement a random resource from the target
			_owner.pendingState.TakeRandomResourceFromPeerServerRpc(selectedPlayer);
		}
		
		[ServerRpc(RequireOwnership = false)]
		public void GiveResourceToCurrentPlayerServerRpc(ResourceType resourceType)
		{
			GiveResourceToCurrentPlayerClientRpc(resourceType);
		}
		
		[ClientRpc]
		public void GiveResourceToCurrentPlayerClientRpc(ResourceType resourceType)
		{
			if (_gameManagerSO.IsClientTurn())
			{
				string clientname = _gameManagerSO.GetClientPlayerName();
				string peerName = _gameManagerSO.playerList[m_HackedPlayer].GetName();
				
				if (resourceType != ResourceType.Any && resourceType != ResourceType.None)
				{
					m_NotificationEventChannel.RaiseEvent($"{clientname} took a resource  from {peerName}.");
					_gameManagerSO.clientPlayer.AddResources(resourceType,1);
					
				}
				else
				{
					m_NotificationEventChannel.RaiseEvent($"{clientname} could not take any resources from {peerName}.");
				}
			}
			
			// Change to idle state
			changeState(_owner.idleState);
		}
		
		// [ServerRpc(RequireOwnership = false)]
		// void UpdateHexStatesServerRpc(uint selectedHex) => UpdateHexStatesClientRpc(selectedHex);
		
		// [ClientRpc]
		// void UpdateHexStatesClientRpc(uint selectedHex)
		// {
		// 	// _hexManagerSO.SetHackedHex(selectedHex);
		// }
		
		
		public override void OnGUI()
		{
			if (!IsActiveState()) return;
			
		}
		
		void CancelButtonEventHandler()
		{
			if (m_State == HackState.HexSelect)
			{
				_hexManagerSO.EndHackModeEvent.Invoke();
				changeState(_owner.idleState);
			}
		}
		

	}
}