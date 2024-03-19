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
			
			m_MenuState = MenuState.Default;
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
			// if(GUILayout.Button("Hack"))
			// {
				// _gameManagerSO.OnDebugIncrementAlltResources();
				// _userInterfaceSO.OnUpdateUserInterface();
				// _owner.hexManagerSO.hackEvent.Invoke(13);
				// _owner.hexManagerSO.SetHackerCages(13,true);
				// _owner.hexManagerSO.SetHackerParticles(13,true);
			// }
			// GUILayout.EndArea();
			
			// if (_menuState == MenuState.Default)
			// {
			// 	GUI.Box(UserInterface.s_gameMenuArea, "");
			// 	// game menu box
			// 	GUILayout.BeginArea(UserInterface.s_gameMenuArea);
			// 	//GUILayout.Label("Current Turn: Player " + gameManagerSO.getGUITurn());
			// 	if (GUILayout.Button("Menu")){
			// 		_menuState = MenuState.Expanded;
			// 	}
			// 	if(GUILayout.Button("Next Turn"))
			// 	{
			// 		_gameManagerSO.OnStartNextTurn();
			// 		changeState(_owner.diceState);
			// 	}
			// 	GUILayout.EndArea();
			// }
			// else if (_menuState == MenuState.Expanded)
			// {
			// 	GUI.Box(UserInterface.s_expandedGameMenu, "");

			// 	// game menu box
			// 	GUILayout.BeginArea(UserInterface.s_expandedGameMenu);
			// 	//GUILayout.Label("Current Turn: Player " + gameManagerSO.getGUITurn());
			// 	if (GUILayout.Button("Build"))
			// 	{
			// 		// _menuState = MenuState.BuildMode;

			// 		// gameManagerSO.beginClientBuildMode();
			// 		changeState(_owner.buildingState);
			// 	}
			// 	if (GUILayout.Button("Trade"))
			// 	{
			// 		// tradeRequestSelection = 0;
			// 		// tradeRequestSlider = 1f;
			// 		// tradeOfferSelection = 0;
			// 		// tradeOfferSlider = 1f;
			// 		// _menuState = MenuState.TradeRequest;
			// 	}
			// 	if(GUILayout.Button("Event Card"))
			// 	{
			// 		// _menuState = MenuState.Active;
			// 		// gameManagerSO.clientRequestEventCard();
			// 	}
			// 	if (GUILayout.Button("Hack"))
			// 	{
			// 		// _menuState = MenuState.HackMode;
			// 		// gameManagerSO.beginClientHackMode();
			// 		changeState(_owner.hackingState);
			// 	}
			// 	if(GUILayout.Button("Debug: Increment All Resources"))
			// 	{
			// 		_gameManagerSO.OnDebugIncrementAlltResources();
			// 		_userInterfaceSO.OnUpdateUserInterface();
			// 	}
			// 	if (GUILayout.Button("Close"))
			// 	{
			// 		_menuState = MenuState.Default;
			// 	}
			// 	if(GUILayout.Button("Next Turn"))
			// 	{
			// 		_gameManagerSO.OnStartNextTurn();
			// 		changeState(_owner.diceState);
			// 	}
			// 	// if(GUILayout.Button("Next Turn")) gameManagerSO.nextTurn();
			// 	GUILayout.EndArea();
			// }
			
			
			
			// resource box
		}
		 
		

	}
}