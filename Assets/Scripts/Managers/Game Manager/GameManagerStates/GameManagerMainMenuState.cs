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
			_userInterfaceSO.singlePlayerButtonEvent += SinglePlayerButtonEventHandler;
			_userInterfaceSO.hostMultiplayerButtonEvent += HostMultiplayerButtonEventHandler;
			_userInterfaceSO.joinMultiplayerButtonEvent += JoinMultiplayerButtonEventHandler;
			
			_userInterfaceSO.multiplayerConnectButtonEvent += MultiplayerConnectButtonEventHandler;
			_userInterfaceSO.multiplayerCancelGameButtonEvent += MultiplayerCancelGameButtonEventHandler;
			
			_userInterfaceSO.multiplayerStartGameButtonEvent += LobbyStartGameButtonEventHandler;
			_userInterfaceSO.multiplayerLeaveLobbyButtonEvent += LeaveLobbyEventHandler;
			
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
			
			
			
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			
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
			changeState(new GameManagerSetupState(_owner));
			
			_userInterfaceSO.OnSetMainMenuActive(false);
		}
		
		void HostMultiplayerButtonEventHandler()
		{
			Debug.Log("Starting Multiplayer Game as Host!");
			_userInterfaceSO.OnUpdateMainMenuScreen(UserInterface.MainMenuScreens.LobbyScreen);
			
			_gameManagerSO.Initialize();
			
			// playerManagers[i] = CreateInstance("PlayerManagerSO") as PlayerManagerSO;
			Player currentPlayer = new Player("Player 1", 0);
			_gameManagerSO.AddPlayer(currentPlayer);
			_gameManagerSO.SetClientPlayer(currentPlayer);
			
			// NetworkManager.Singleton.StartHost();
			// _gameNetworkManagerSO.OnHostGame("Player 1");
			
			
			// uint[] clientIds = new uint[numPlayers];
			// for (int i = 0; i < numPlayers; i++)
			// {
			// 	initClientSOsClientRpc(clientIds,new NetworkStringArray(playerNames.ToArray()));
			// 	clientIds[i] = (uint)NetworkManager.ConnectedClientsIds[i];
			// } 
		}
		
		void JoinMultiplayerButtonEventHandler()
		{
			Debug.Log("Starting Multiplayer Game as Client!");
			_userInterfaceSO.OnUpdateMainMenuScreen(UserInterface.MainMenuScreens.JoinMultiplayerScreen);
		}
		
		#region Join Multiplayer Game Event Handlers
		void MultiplayerConnectButtonEventHandler(ConnectionRequest request)
		{
			// _gameNetworkManagerSO.OnConnectionRequest(request);
			// _userInterfaceSO.OnUpdateMainMenuScreen(UserInterface.MainMenuScreens.LobbyScreen);
		}
		
		void MultiplayerCancelGameButtonEventHandler()
		{
			_userInterfaceSO.OnUpdateMainMenuScreen(UserInterface.MainMenuScreens.TitleScreen);
		}
		void ClientConnectedToServerEventHandler()
		{
			_userInterfaceSO.OnUpdateMainMenuScreen(UserInterface.MainMenuScreens.LobbyScreen);
		}
		void ServerRecievedNewClientConnectionEventHandler()
		{
			// Debug.Log("GameManager has processed a new client connection!");
			// if (_gameNetworkManagerSO.numConnectedClients > 1)
			// {
			// 	_userInterfaceSO.OnUpdatePlayerDisplays(_gameManagerSO.playerList);
			// }
		}
		#endregion
		
		#region Multiplayer Lobby Event Handler
		void LobbyStartGameButtonEventHandler()
		{
			
		}
		void LeaveLobbyEventHandler()
		{
			_userInterfaceSO.OnUpdateMainMenuScreen(UserInterface.MainMenuScreens.TitleScreen);
		}
		
		/// <summary>
		/// This method allows the server to process the new client's name and then send out the updates to all other clients.
		/// The server also takes this time to update its UI player displays in the lobby menu.
		/// This is triggered by the RPC event from the client when it sends its name.
		/// <param name="playerName">The player name sent by the client.</param>
		/// <param name="clientId">The ulong client ID.</param>
		/// <returns>void</returns>
		/// </summary>
		void ServerReceivedNewPlayerInfoEventHandler(string playerName, ulong clientId)
		{
			// Add the new client player to the game manager scriptable object
			Player newPlayer = new Player(playerName, _gameManagerSO.playerList.Count);
			newPlayer.SetNetworkClientId(clientId);
			_gameManagerSO.AddPlayer(newPlayer);
			
			// Update the game lobby screen.
			_userInterfaceSO.OnUpdatePlayerDisplays(_gameManagerSO.playerList);
			
			// Send Updates to All Clients
			// _gameNetworkManagerSO.OnServerUpdateAllPlayerListsOnClients(_gameManagerSO.playerList);
			ServerUpdateAllPlayerListsOnClients();
		}
		async void ServerUpdateAllPlayerListsOnClients()
		{
			// Send Updates to All Clients
			await WaitForSecondsAsync(0.1f);
			// _gameNetworkManagerSO.OnServerUpdateAllPlayerListsOnClients(_gameManagerSO.playerList);
		}
		async Task WaitForSecondsAsync(float delay)
		{
			await Task.Delay(TimeSpan.FromSeconds(delay));
		}
		
		void ClientUpdateAllPlayerListsEventHandler(string[] playerNames)
		{
			// _gameManagerSO.playerList
			List<Player> playerList = new List<Player>();
			for(int i = 0; i < playerNames.Length; i++) playerList.Add(new Player(playerNames[i], i));
			// _userInterfaceSO.OnUpdatePlayerDisplays(_gameManagerSO.playerList);
			_userInterfaceSO.OnUpdatePlayerDisplays(playerList);
		}
		#endregion
		
		// void ConnectionRequestEventHandler(ConnectionRequest request)
		// {
		// 	transport.SetConnectionData(
		// 		request.ipAddress,  // The IP address is a string
		// 		request.port // The port number is an unsigned short, I have 7777 assigned for Unity Games
		// 	);
		// 	NetworkManager.Singleton.OnClientDisconnectCallback += failedConnection => {
		// 		Debug.Log("Failed to connect to host at: " + request.ipAddress + ":" + request.port);
		// 		NetworkManager.Singleton.Shutdown();
		// 	};
		// 	NetworkManager.Singleton.OnClientConnectedCallback += succeededConnection => {
		// 		Debug.Log("Successfully connected to host at: " + request.ipAddress + ":" + request.port);
		// 		if (!NetworkManager.Singleton.IsServer) gameNetworkManagerSO.OnClientConnectedToServer();
		// 		// Clients must send their player name to the server
		// 		SendClientPlayerNameServerRpc(gameNetworkManagerSO.clientPlayerName, NetworkManager.Singleton.LocalClientId);
		// 	};
		// 	bool clientStarted = NetworkManager.Singleton.StartClient();
		// 	if (!clientStarted) Debug.LogError("ERROR: There may have been an issue starting the client!");
		// }
		
		

	}
}