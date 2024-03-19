using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerHackingState : GameManagerAbstractState
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
		IntEventChannel m_SelectTradePartnerEventChannel;
		
		[SerializeField]
		EventChannelSO m_CancelButtonEventChannel;
		
		enum HackState
		{
			HexSelect,
			TargetSelect
		}
		HackState m_State;
		
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
			_userInterfaceSO.OnUpdateUserInterface();
			_hexManagerSO.EndHackModeEvent.Invoke();
			// changeState(_owner.idleState);
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerDiceHackTargetScreen);
			m_InitiateTradeEventChannel.RaiseEvent(TradeInitWindowType.Hacking);
			
			m_State = HackState.TargetSelect;
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