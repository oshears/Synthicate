using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerIdleState : GameManagerAbstractState
	{
		
		// public enum MenuState
		// {
		// 	Default,
		// 	Expanded,
		// 	Build,
		// 	Hack,	
		// }
		
		// MenuState _menuState;
		
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
		
		public override void Enter()
		{
			// _menuState = MenuState.Default;
			Debug.Log("Player Resources:");
			_userInterfaceSO.OnUpdateUserInterface();
			
			m_GameMenuStateEventChannel.RaiseEvent(GameMenu.Screens.PlayerTurnScreen);
		}
		
		public override void Execute()
		{
			m_CyberActionButtionEventChannel.OnEventRaised += CyberActionButtionEventHandler; 
			m_TradeButtonEventChannel.OnEventRaised += TradeButtonEventHandler; 
			m_BuildModeButtonEventChannel.OnEventRaised += BuildModeButtonEventHandler; 
			m_FinishTurnButtonEventChannel.OnEventRaised += FinishTurnButtonEventHandler; 
		}

		public override void Exit()
		{
			m_CyberActionButtionEventChannel.OnEventRaised -= CyberActionButtionEventHandler; 
			m_TradeButtonEventChannel.OnEventRaised -= TradeButtonEventHandler; 
			m_BuildModeButtonEventChannel.OnEventRaised -= BuildModeButtonEventHandler; 
			m_FinishTurnButtonEventChannel.OnEventRaised -= FinishTurnButtonEventHandler; 
		}
		

		public void CyberActionButtionEventHandler()
		{
			
		}
		public void TradeButtonEventHandler()
		{
			changeState(_owner.tradingState);
		}
		public void BuildModeButtonEventHandler()
		{
			changeState(_owner.buildingState);
		}
		
		public void FinishTurnButtonEventHandler()
		{
			IncrementPlayerTurnServerRpc();
			changeState(_owner.diceState);
		}
		
		[ServerRpc(RequireOwnership = false)]
		void IncrementPlayerTurnServerRpc() => IncrementPlayerTurnClientRpc();
		[ClientRpc]
		void IncrementPlayerTurnClientRpc() => _gameManagerSO.IncrementAndGetNextPlayerIndex(); 

		
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