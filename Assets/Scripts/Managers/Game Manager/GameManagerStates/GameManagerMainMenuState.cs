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
	
		public GameManagerMainMenuState(GameManager owner) : base(owner) 
		{
			
			
			// _gameNetworkManagerSO.clientConnectedToServer += ClientConnectedToServerEventHandler;
			// _gameNetworkManagerSO.serverRecievedNewClientConnection += ServerRecievedNewClientConnectionEventHandler;
			
			// _gameNetworkManagerSO.ServerReceivedNewPlayerInfoEvent += ServerReceivedNewPlayerInfoEventHandler;
			// _gameNetworkManagerSO.ClientUpdateAllPlayerListsEvent += ClientUpdateAllPlayerListsEventHandler;
			
			_waitingForClientReady = false;
			_waitTimeForClient = 0;
		}

		public override void Enter()
		{
			_userInterfaceSO.OnSetMainMenuActive(true);
			_userInterfaceSO.OnSetGameMenuActive(false);
			
			_userInterfaceSO.singlePlayerButtonEvent += SinglePlayerButtonEventHandler;
			_userInterfaceSO.hostMultiplayerButtonEvent += HostMultiplayerButtonEventHandler;
			_userInterfaceSO.joinMultiplayerButtonEvent += JoinMultiplayerButtonEventHandler;
			
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
			_gameManagerSO.SetClientPlayer(currentPlayer);
			
			NetworkManager.Singleton.StartHost();
			// _gameNetworkManagerSO.OnHostGame("Player 1");


			// if (clientPlayerManager.getId() == currentSetupTurn)
			// 	beginClientSetupStrongholdBuildMode();

			// updateGUIEvent.Invoke();
			// updateMainMenuEvent.Invoke(false);
			
			// Go to setup state
			changeState(_owner.setupState);
			
			_userInterfaceSO.OnSetMainMenuActive(false);
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
				_gameManagerSO.SetClientPlayer(currentPlayer);
				
			
				_userInterfaceSO.OnUpdateMainMenuScreen(UserInterface.MainMenuScreens.LobbyScreen);
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
			_userInterfaceSO.OnUpdateMainMenuScreen(UserInterface.MainMenuScreens.JoinMultiplayerScreen);
			changeState(_owner.clientLobbyState); 
		}
		
	}
}