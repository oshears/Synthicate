using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerTradingState : GameManagerAbstractState
	{
		

		
		[Header("Event Channels")]
		
		[SerializeField]
		GameMenuStateEventChannel m_GameMenuStateEventChannel;
		
		[SerializeField]
		EventChannelSO m_TradeCanceledEventChannel;
		
		public override void Enter()
		{
			// _menuState = MenuState.Default;
			_userInterfaceSO.OnUpdateUserInterface();
			
			// TODO: Need to add the logic for the Trade Init Screen and the Trade Requester / Receiver Screen
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.TradeRequesterScreen);
			
			m_TradeCanceledEventChannel.OnEventRaised += TradeCanceledEventHandler;
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{

		}
		
		public void TradeCanceledEventHandler()
		{
			changeState(_owner.idleState);
		}



	}
}