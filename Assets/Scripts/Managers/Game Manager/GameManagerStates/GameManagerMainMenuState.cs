using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using System;

namespace Synthicate
{
	
	public class GameManagerMainMenuState : GameManagerAbstractState
	{
		bool _waitingForClientReady = false;
		float _waitTimeForClient = 0;
	
		public override void Enter()
		{
			_userInterfaceSO.OnSetMainMenuActive(true);
			_userInterfaceSO.OnSetGameMenuActive(false);
			_userInterfaceSO.OnUpdateMainMenuScreen(MainMenu.Screens.TitleScreen);
			
			
			_userInterfaceSO.singlePlayerButtonEvent += SinglePlayerButtonEventHandler;
			_userInterfaceSO.hostMultiplayerButtonEvent += HostMultiplayerButtonEventHandler;
			_userInterfaceSO.joinMultiplayerButtonEvent += JoinMultiplayerButtonEventHandler;
			
			_waitingForClientReady = false;
			_waitTimeForClient = 0;
			
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			_userInterfaceSO.singlePlayerButtonEvent -= SinglePlayerButtonEventHandler;
			_userInterfaceSO.hostMultiplayerButtonEvent -= HostMultiplayerButtonEventHandler;
			_userInterfaceSO.joinMultiplayerButtonEvent -= JoinMultiplayerButtonEventHandler;
		}
		
		void SinglePlayerButtonEventHandler()
		{
			Debug.Log("Starting Singleplayer Game!");
			
			_gameManagerSO.Initialize();
			
			// playerManagers[i] = CreateInstance("PlayerManagerSO") as PlayerManagerSO;
			Player currentPlayer = new Player("Player 1", 0);
			_gameManagerSO.AddPlayer(currentPlayer);
			_gameManagerSO.SetClientPlayer(0);
			
			NetworkManager.Singleton.StartHost();
			// _gameNetworkManagerSO.OnHostGame("Player 1");


			// if (clientPlayerManager.getId() == currentSetupTurn)
			// 	beginClientSetupStrongholdBuildMode();

			// updateGUIEvent.Invoke();
			// updateMainMenuEvent.Invoke(false);
			
			_userInterfaceSO.OnSetMainMenuActive(false);
			
			
			_gameManagerSO.SetCurrentPlayerTurn(0);
			
			// Enable the game menu
			_userInterfaceSO.OnSetGameMenuActive(true);
			
			// Go to setup state
			changeState(_owner.setupState);
		}
		
		void HostMultiplayerButtonEventHandler()
		{
			
			_transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
			if (_transport == null) Debug.LogError("Failed to find a network transport!");
			bool hostStarted = NetworkManager.Singleton.StartHost();
			if(hostStarted)
			{
				Debug.Log("Starting Multiplayer Game as Host!");
				_gameManagerSO.Initialize();

				// Create host player
				Player currentPlayer = new Player("Player 1", 0);
				_gameManagerSO.AddPlayer(currentPlayer);
				_gameManagerSO.SetClientPlayer(0);
				
			
				_userInterfaceSO.OnUpdateMainMenuScreen(MainMenu.Screens.HostLobbyScreen);
				changeState(_owner.hostLobbyState); 
			} 
			else
			{
				Debug.LogError("ERROR: There may have been an issue starting the host server!");
			}
			
		}
		
		void JoinMultiplayerButtonEventHandler()
		{
			Debug.Log("Starting Multiplayer Game as Client!");
			_userInterfaceSO.OnUpdateMainMenuScreen(MainMenu.Screens.JoinMultiplayerScreen);
			changeState(_owner.clientLobbyState); 
		}
		
	}
}