using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using System;

namespace Synthicate
{
	
	public class GameManagerClientLobbyState : GameManagerAbstractState
	{
		bool _waitingForClientReady = false;
		float _waitTimeForClient = 0;
		
		string _playerName = "";
		
		int _clientId = -1;
		
		[SerializeField]
		static float s_ClientConnectTimeDelay = 0.1f;
	
		public override void Enter()
		{
			_userInterfaceSO.multiplayerConnectButtonEvent += MultiplayerConnectButtonEventHandler;
			_userInterfaceSO.multiplayerCancelGameButtonEvent += MultiplayerCancelGameButtonEventHandler;
			_userInterfaceSO.multiplayerLeaveLobbyButtonEvent += LeaveLobbyEventHandler;
			
			_userInterfaceSO.OnSetMainMenuActive(true);
			_userInterfaceSO.OnSetGameMenuActive(false);
			
			_transport  = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
			
			_waitingForClientReady = false;
			_waitTimeForClient = 0;
			_playerName = "";
			_clientId = -1;
		}
		
		public override void Execute()
		{
			if (_waitingForClientReady)
			{
				if(_waitTimeForClient < s_ClientConnectTimeDelay)
				{
					_waitTimeForClient += Time.deltaTime;
				}
				else
				{
					_waitingForClientReady = false;
					// SendClientPlayerNameServerRpc(_playerName, NetworkManager.Singleton.LocalClientId);
					_owner.hostLobbyState.ReceiveClientPlayerNameServerRpc(_playerName, NetworkManager.Singleton.LocalClientId);
				}
			}
			
		}

		public override void Exit()
		{
			_userInterfaceSO.multiplayerConnectButtonEvent -= MultiplayerConnectButtonEventHandler;
			_userInterfaceSO.multiplayerCancelGameButtonEvent -= MultiplayerCancelGameButtonEventHandler;
			_userInterfaceSO.multiplayerLeaveLobbyButtonEvent -= LeaveLobbyEventHandler;
		}
		
	
		void MultiplayerConnectButtonEventHandler(ConnectionRequest request)
		{
			_transport.SetConnectionData(
				request.ipAddress,  // The IP address is a string
				request.port // The port number is an unsigned short, I have 7777 assigned for Unity Games
			);
			NetworkManager.Singleton.OnClientDisconnectCallback += failedConnection => {
				Debug.Log("Failed to connect to host at: " + request.ipAddress + ":" + request.port);
				NetworkManager.Singleton.Shutdown();
				changeState(_owner.mainMenuState);
			};
			NetworkManager.Singleton.OnClientConnectedCallback += succeededConnection => {
				Debug.Log("Successfully connected to host at: " + request.ipAddress + ":" + request.port);
				// gameNetworkManagerSO.OnClientConnectedToServer();
				// Clients must send their player name to the server
				_playerName = request.playerName;
				_waitingForClientReady = true;
				_userInterfaceSO.OnUpdateMainMenuScreen(MainMenu.Screens.ClientLobbyScreen);
			};
			bool _connectedToServer = NetworkManager.Singleton.StartClient();
			if (!_connectedToServer) Debug.LogError("ERROR: There may have been an issue starting the client!");
		}
		
		[ClientRpc]
		public void UpdateAllPlayerListsClientRpc(StringContainer[] playerNames)
		{
			if (!NetworkManager.Singleton.IsServer)
			{
				// Extract strings from the StringContainer
				string[] playerNamesArry = new string[playerNames.Length];
				for (int i = 0; i < playerNames.Length; i++)
				{
					playerNamesArry[i] = playerNames[i].text;
				}

				// Initialize game manager SO using new player list with names list from the server
				_gameManagerSO.InitializeWithPlayerNames(playerNamesArry);
				
				// Update the UI Lobby View
				_userInterfaceSO.OnUpdatePlayerDisplays(_gameManagerSO.playerList);
				
				// Set client id if not already assigned
				if (_clientId == -1)
				{
					_clientId = _gameManagerSO.playerList.Count - 1;
				}

				// Update client player reference
				_gameManagerSO.SetClientPlayer(_clientId);
			}
			 
		}
		
		[ClientRpc]
		public void StartGameClientRpc(bool skipSetup = false)
		{
			if (!NetworkManager.Singleton.IsServer)
			{
				Debug.Log($"Starting Game with {_gameManagerSO.playerList.Count} players!");
				
				// Enable the game menu
				_userInterfaceSO.OnSetMainMenuActive(false);
				_userInterfaceSO.OnSetGameMenuActive(true);

				changeState(skipSetup ? _owner.pendingState : _owner.pendingSetupState);
			}
		}
		
		void MultiplayerCancelGameButtonEventHandler()
		{
			_userInterfaceSO.OnUpdateMainMenuScreen(MainMenu.Screens.TitleScreen);
			if (NetworkManager.Singleton.IsConnectedClient) NetworkManager.Singleton.Shutdown();
		}
		

		void LeaveLobbyEventHandler()
		{
			_userInterfaceSO.OnUpdateMainMenuScreen(MainMenu.Screens.TitleScreen);
			changeState(_owner.mainMenuState);
		}
	}
}