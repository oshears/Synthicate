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
			DepotTrade
		}
		
		MenuState m_MenuState;
		
		[Header("Event Channels")]
		
		[SerializeField] GameMenuStateEventChannel m_GameMenuStateEventChannel;
		
		[SerializeField] EventChannelSO m_CyberActionButtionEventChannel;
		
		[SerializeField] EventChannelSO m_TradeButtonEventChannel;
		
		[SerializeField] EventChannelSO m_BuildModeButtonEventChannel;
		
		[SerializeField] EventChannelSO m_FinishTurnButtonEventChannel;
		
		[SerializeField] EventChannelSO m_HackButtonEventChannel;
		
		[SerializeField] EventChannelSO m_CancelButtonEventChannel;
		
		[SerializeField] DepotSelectedEventChannelSO m_DepotSelectedEventChannel;
		
		[SerializeField] BoolEventChannelSO m_EnableDepotSelectionEventChannel;
		
		[SerializeField] EventChannelSO m_TradeExecutedEventChannel;
		
		[SerializeField] EventChannelSO m_CancelTradeEventChannel;
		
		[SerializeField] EventChannelSO m_InitializeUiEventChannel;
		[SerializeField] EventChannelSO m_UpdateUiEventChannel;
		
		public override void Enter()
		{
			// _menuState = MenuState.Default;
			m_InitializeUiEventChannel.RaiseEvent();
			m_UpdateUiEventChannel.RaiseEvent();
			
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerTurnScreen);
			
			m_CyberActionButtionEventChannel.OnEventRaised += CyberActionButtionEventHandler; 
			m_TradeButtonEventChannel.OnEventRaised += TradeButtonEventHandler; 
			m_BuildModeButtonEventChannel.OnEventRaised += BuildModeButtonEventHandler; 
			m_FinishTurnButtonEventChannel.OnEventRaised += FinishTurnButtonEventHandler; 
			m_HackButtonEventChannel.OnEventRaised += HackButtonEventHandler;
			m_CancelButtonEventChannel.OnEventRaised += CancelButtonEventHandler;
			m_DepotSelectedEventChannel.OnEventRaised += DepotSelectedEventHandler;
			m_TradeExecutedEventChannel.OnEventRaised += TradeExecutedEventHandler;
			m_CancelTradeEventChannel.OnEventRaised += CancelTradeEventHandler;
			
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
			m_TradeExecutedEventChannel.OnEventRaised -= TradeExecutedEventHandler;
			
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
		
		public void TradeExecutedEventHandler()
		{
			m_MenuState = MenuState.Default;
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerTurnScreen);
			m_EnableDepotSelectionEventChannel.RaiseEvent(true);
			m_UpdateUiEventChannel.RaiseEvent();
		}
		
		public void CancelButtonEventHandler()
		{
			m_MenuState = MenuState.Default;
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerTurnScreen);
			m_EnableDepotSelectionEventChannel.RaiseEvent(true);
		}
		
		public void CancelTradeEventHandler()
		{
			m_MenuState = MenuState.Default;
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerTurnScreen);
			m_EnableDepotSelectionEventChannel.RaiseEvent(true);
		}
		
		void DepotSelectedEventHandler(DepotSelection depotSelection)
		{
			Debug.Log("Begining to handle depot selection for:" + depotSelection);
			m_MenuState = MenuState.DepotTrade;
			if (depotSelection.Resource == ResourceType.Any)
			{
				m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.AnyDepotTrade);
			}
			else
			{
				m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.DepotTrade);
			}
			m_EnableDepotSelectionEventChannel.RaiseEvent(false);
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
				m_UpdateUiEventChannel.RaiseEvent();
			}
			GUILayout.EndArea();
		}
		 
		

	}
}