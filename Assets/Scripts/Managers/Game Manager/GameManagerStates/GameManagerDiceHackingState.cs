using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Synthicate
{
	
	public class GameManagerDiceHackingState : GameManagerAbstractState
	{
		
		[Header("Event Channels")]
		
		[SerializeField]
		GameMenuStateEventChannel m_GameMenuStateEventChannel;
		
		[SerializeField]
		StringEventChannel m_NotificationEventChannel;
		
		[SerializeField]
		StringEventChannel m_LocalNotificationEvent;
		
		[SerializeField]
		TradeInitEventChannel m_InitiateTradeEventChannel;
		
		[SerializeField]
		IntEventChannelSO m_SelectTradePartnerEventChannel;

		public override void Enter()
		{
			
			if (_gameManagerSO.IsClientTurn())
			{
				m_NotificationEventChannel.RaiseEvent($"{_gameManagerSO.clientPlayer.GetName()} is selecting a territory to hack...");
				m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerDiceHackScreen);
			}
			else
			{
				m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerPendingScreen);
			}
			
			_hexManagerSO.BeginHackModeEvent.Invoke();
			
			m_SelectTradePartnerEventChannel.OnEventRaised += SelectTradePartnerEventHandler;
			_hexManagerSO.hackEvent.AddListener(HackEventHandler);
		}
		
		public override void Execute()
		{
			
			
		}

		public override void Exit()
		{
			_hexManagerSO.hackEvent.RemoveListener(HackEventHandler);
			m_SelectTradePartnerEventChannel.OnEventRaised -= SelectTradePartnerEventHandler;
		}
		
		public void HackEventHandler(uint selectedHex)
		{
			// UpdateHexStatesServerRpc(selectedHex);
			m_NotificationEventChannel.RaiseEvent($"{_gameManagerSO.clientPlayer.GetName()} hacked a territory!");
			_userInterfaceSO.OnUpdateUserInterface();
			_hexManagerSO.EndHackModeEvent.Invoke();
			// changeState(_owner.idleState);
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerDiceHackTargetScreen);
			m_InitiateTradeEventChannel.RaiseEvent(TradeInitWindowType.Hacking);
		}
		
		public void SelectTradePartnerEventHandler(int selectedPlayer)
		{
			// Decrement a random resource from the target
			
			// Incrmeent the client's resources
			// _gameManagerSO.clientPlayer.
			for(int i = 0; i < 5; i++)
			{
				_gameManagerSO.clientPlayer.resources[i]++;
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
		

	}
}