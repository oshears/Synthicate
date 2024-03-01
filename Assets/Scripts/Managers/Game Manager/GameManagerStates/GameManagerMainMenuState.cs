using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace Synthicate
{
	public class GameManagerMainMenuState : GameManagerAbstractState
	{
		public GameManagerMainMenuState(GameManager owner) : base(owner) 
		{
			_userInterfaceSO.singlePlayerButtonEvent += SinglePlayerButtonEventHandler;
			_userInterfaceSO.hostMultiplayerButtonEvent += HostMultiplayerButtonEventHandler;
			_userInterfaceSO.joinMultiplayerButtonEvent += JoinMultiplayerButtonEventHandler;
			
			_userInterfaceSO.multiplayerConnectButtonEvent += MultiplayerConnectButtonEventHandler;
			_userInterfaceSO.multiplayerCancelGameButtonEvent += MultiplayerCancelGameButtonEventHandler;
			
			_userInterfaceSO.multiplayerStartGameButtonEvent += LobbyStartGameButtonEventHandler;
			_userInterfaceSO.multiplayerLeaveLobbyButtonEvent += LeaveLobbyEventHandler;
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
			
			for (int i = 0; i < _gameManagerSO.numPlayers; i++)
			{
				// playerManagers[i] = CreateInstance("PlayerManagerSO") as PlayerManagerSO;
				Player currentPlayer = new Player("Player 1", 0);
				_gameManagerSO.AddPlayer(currentPlayer);
				_gameManagerSO.SetClientPlayer(currentPlayer);
				
				// NetworkManager.Singleton.StartHost();
				_gameNetworkManagerSO.OnHostGame();
			}


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
			
			for (int i = 0; i < _gameManagerSO.numPlayers; i++)
			{
				// playerManagers[i] = CreateInstance("PlayerManagerSO") as PlayerManagerSO;
				Player currentPlayer = new Player("Player 1", 0);
				_gameManagerSO.AddPlayer(currentPlayer);
				_gameManagerSO.SetClientPlayer(currentPlayer);
				
				// NetworkManager.Singleton.StartHost();
				_gameNetworkManagerSO.OnHostGame();
			}
			
			_userInterfaceSO.OnUpdatePlayerDisplays(_gameManagerSO.playerList);
			
			// uint[] clientIds = new uint[numPlayers];
			// for (int i = 0; i < numPlayers; i++)
			// {
			// 	initClientSOsClientRpc(clientIds,new NetworkStringArray(playerNames.ToArray()));
			// 	clientIds[i] = (uint)NetworkManager.ConnectedClientsIds[i];
			// } 
			_gameNetworkManagerSO.OnHostGame();
		}
		
		void JoinMultiplayerButtonEventHandler()
		{
			Debug.Log("Starting Multiplayer Game as Client!");
			_userInterfaceSO.OnUpdateMainMenuScreen(UserInterface.MainMenuScreens.JoinMultiplayerScreen);
		}
		
		#region Join Multiplayer Game Event Handlers
		void MultiplayerConnectButtonEventHandler(ConnectionRequest request)
		{
			_userInterfaceSO.OnUpdateMainMenuScreen(UserInterface.MainMenuScreens.LobbyScreen);
		}
		
		void MultiplayerCancelGameButtonEventHandler()
		{
			_userInterfaceSO.OnUpdateMainMenuScreen(UserInterface.MainMenuScreens.TitleScreen);
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
		#endregion

	}
}