using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerIdleState : GameManagerAbstractState
	{
		
		public enum MenuState
		{
			Default,
			Expanded,
		}
		
		MenuState m_MenuState;
		
		[Header("Event Channels")]
		
		[SerializeField]
		GameMenuStateEventChannel m_GameMenuStateEventChannel;
		
		[SerializeField]
		EventChannelSO m_CyberActionButtionEventChannel;
		
		[SerializeField]
		EventChannelSO m_TradeButtonEventChannel;
		
		[SerializeField]
		EventChannelSO m_BuildModeButtonEventChannel;
		
		[SerializeField]
		EventChannelSO m_FinishTurnButtonEventChannel;
		
		[SerializeField]
		EventChannelSO m_HackButtonEventChannel;
		
		[SerializeField]
		EventChannelSO m_CancelButtonEventChannel;
		
		[SerializeField]
		DepotSelectedEventChannel m_DepotSelectedEventChannel;
		
		[SerializeField]
		BoolEventChannel m_EnableDepotSelectionEventChannel;
		
		public override void Enter()
		{
			// _menuState = MenuState.Default;
			_userInterfaceSO.OnUpdateUserInterface();
			
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerTurnScreen);
			
			m_CyberActionButtionEventChannel.OnEventRaised += CyberActionButtionEventHandler; 
			m_TradeButtonEventChannel.OnEventRaised += TradeButtonEventHandler; 
			m_BuildModeButtonEventChannel.OnEventRaised += BuildModeButtonEventHandler; 
			m_FinishTurnButtonEventChannel.OnEventRaised += FinishTurnButtonEventHandler; 
			m_HackButtonEventChannel.OnEventRaised += HackButtonEventHandler;
			m_CancelButtonEventChannel.OnEventRaised += CancelButtonEventHandler;
			m_DepotSelectedEventChannel.OnEventRaised += DepotSelectedEventHandler;
			
			m_MenuState = MenuState.Default;
			
			// _depotManagerSO.
			m_EnableDepotSelectionEventChannel.RaiseEvent(true);
		}
		
		public override void Execute()
		{
			
		}

		public override void Exit()
		{
			m_CyberActionButtionEventChannel.OnEventRaised -= CyberActionButtionEventHandler; 
			m_TradeButtonEventChannel.OnEventRaised -= TradeButtonEventHandler; 
			m_BuildModeButtonEventChannel.OnEventRaised -= BuildModeButtonEventHandler; 
			m_FinishTurnButtonEventChannel.OnEventRaised -= FinishTurnButtonEventHandler; 
			m_HackButtonEventChannel.OnEventRaised -= HackButtonEventHandler;
			m_CancelButtonEventChannel.OnEventRaised -= CancelButtonEventHandler;
			m_DepotSelectedEventChannel.OnEventRaised -= DepotSelectedEventHandler;
			
			m_EnableDepotSelectionEventChannel.RaiseEvent(false);
		}
		

		public void CyberActionButtionEventHandler()
		{
			m_MenuState	= MenuState.Expanded;
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.CyberActions);
		}
		public void HackButtonEventHandler()
		{
			changeState(_owner.hackingState);
		}
		public void TradeButtonEventHandler()
		{
			changeState(_owner.tradingState);
		}
		public void BuildModeButtonEventHandler()
		{
			changeState(_owner.buildingState);
		}
		
		public void CancelButtonEventHandler()
		{
			if (m_MenuState == MenuState.Expanded)
			{
				m_MenuState = MenuState.Default;
				m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerTurnScreen);
			}
		}
		
		void DepotSelectedEventHandler(DepotSelection depotSelection)
		{
			Debug.Log("Begining to handle depot selection for:" + depotSelection);
		}
		
		public void FinishTurnButtonEventHandler()
		{
			FinishPlayerTurnServerRpc();
		}
		
		[ServerRpc(RequireOwnership = false)]
		void FinishPlayerTurnServerRpc() => FinishPlayerTurnClientRpc();
		[ClientRpc]
		void FinishPlayerTurnClientRpc()
		{
			// Increment to the next player
			_gameManagerSO.IncrementAndGetNextPlayerIndex(); 
			
			// Change client to the dice state
			changeState(_owner.diceState);
		}

		
		public override void OnGUI()
		{
			if (!IsActiveState()) return;
			
			Rect screenArea = new Rect(0, Screen.height * 0.25f, Screen.width*0.1f, Screen.height*0.1f);
			GUILayout.BeginArea(screenArea);
			GUI.Box(screenArea, "");
			if(GUILayout.Button("Debug: Increment All Resources"))
			{
				_gameManagerSO.OnDebugIncrementAlltResources();
				_userInterfaceSO.OnUpdateUserInterface();
			}
			GUILayout.EndArea();
		}
		 
		

	}
}