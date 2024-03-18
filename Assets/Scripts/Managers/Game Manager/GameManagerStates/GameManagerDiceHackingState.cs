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
		

		public override void Enter()
		{
			
			if (_gameManagerSO.IsClientTurn())
			{
				m_NotificationEventChannel.RaiseEvent($"{_gameManagerSO.clientPlayer.GetName()} is selecting a territory to hack...");
				m_GameMenuStateEventChannel.RaiseEvent(GameMenu.Screens.PlayerDiceHackScreen);
			}
			else
			{
				m_GameMenuStateEventChannel.RaiseEvent(GameMenu.Screens.PlayerPendingScreen);
			}
			
			_hexManagerSO.BeginHackModeEvent.Invoke();
			
			_hexManagerSO.hackEvent.AddListener(HackEventHandler);
		}
		
		public override void Execute()
		{
			
			
		}

		public override void Exit()
		{
			_hexManagerSO.hackEvent.RemoveListener(HackEventHandler);
		}
		
		public void HackEventHandler(uint selectedHex)
		{
			// UpdateHexStatesServerRpc(selectedHex);
			m_NotificationEventChannel.RaiseEvent($"{_gameManagerSO.clientPlayer.GetName()} hacked a territory!");
			_userInterfaceSO.OnUpdateUserInterface();
			_hexManagerSO.EndHackModeEvent.Invoke();
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