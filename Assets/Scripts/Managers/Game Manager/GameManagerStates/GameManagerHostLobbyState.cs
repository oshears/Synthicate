using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using System;

namespace Synthicate
{
	
	public class GameManagerHostLobbyState : GameManagerAbstractState
	{
	
		[SerializeField]
		StringEventChannel m_NotificationEventChannel;
	
		public override void Enter()
		{
			_userInterfaceSO.multiplayerStartGameButtonEvent += LobbyStartGameButtonEventHandler;
			_userInterfaceSO.multiplayerLeaveLobbyButtonEvent += LeaveLobbyEventHandler;
			NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedEventHandler;
			
			_userInterfaceSO.OnSetMainMenuActive(true);
			_userInterfaceSO.OnSetGameMenuActive(false);
			_transport  = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
			
			// TestClientRpc();
			// TestServerRpc();
			
			m_NotificationEventChannel.RaiseEvent($"You hosted a new game!");
		}
		
		// [ServerRpc(RequireOwnership = false)]
		// private void TestServerRpc()
		// {
		// 	Debug.Log("Passed GameManager (ServerRpc)!");
		// }
		
		// [ClientRpc]
		// private void TestClientRpc()
		// {
		// 	Debug.Log("Passed GameManager (ClientRpc)!");
		// }
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			_userInterfaceSO.multiplayerStartGameButtonEvent -= LobbyStartGameButtonEventHandler;
			_userInterfaceSO.multiplayerLeaveLobbyButtonEvent -= LeaveLobbyEventHandler;
			NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnectedEventHandler;
		}
		
		void ClientConnectedEventHandler(ulong clientId)
		{
			Debug.Log($"Recieved a new client connection: {clientId}");
		}
		
		/// <summary>
		/// This method allows the server to process the new client's name and then send out the updates to all other clients.
		/// The server also takes this time to update its UI player displays in the lobby menu.
		/// This is triggered by the RPC event from the client when it sends its name.
		/// <param name="playerName">The player name sent by the client.</param>
		/// <param name="clientId">The ulong client ID.</param>
		/// <returns>void</returns>
		/// </summary>
		[ServerRpc(RequireOwnership = false)]
		public void ReceiveClientPlayerNameServerRpc(string clientPlayerName, ulong clientId)
		{
			// Add the new client player to the game manager scriptable object
			Player newPlayer = new Player(clientPlayerName, _gameManagerSO.playerList.Count);
			newPlayer.SetNetworkClientId(clientId);
			_gameManagerSO.AddPlayer(newPlayer);
			
			// Update the game lobby screen.
			_userInterfaceSO.OnUpdatePlayerDisplays(_gameManagerSO.playerList);
			
			// Send Updates to All Clients
			StringContainer[] playerNames = new StringContainer[_gameManagerSO.playerList.Count];
			for (int i = 0; i < _gameManagerSO.playerList.Count; i++)
			{
				playerNames[i] = new StringContainer(_gameManagerSO.playerList[i].GetName());
			}
			_owner.clientLobbyState.UpdateAllPlayerListsClientRpc(playerNames);
			
			m_NotificationEventChannel.RaiseEvent($"{clientPlayerName} joined the game!");
		} 
		
		
		void LobbyStartGameButtonEventHandler()
		{
			Debug.Log($"Starting Game with {_gameManagerSO.playerList.Count} players!");
			
			_gameManagerSO.SetCurrentPlayerTurn(0);
			
			// Enable the game menu
			_userInterfaceSO.OnSetMainMenuActive(false);
			_userInterfaceSO.OnSetGameMenuActive(true);
			
			_owner.clientLobbyState.StartGameClientRpc();
			
			changeState(_owner.setupState);
		}
		
		void LeaveLobbyEventHandler()
		{
			_userInterfaceSO.OnUpdateMainMenuScreen(MainMenu.Screens.TitleScreen);
			NetworkManager.Singleton.Shutdown();
			changeState(_owner.mainMenuState);
		}
		
	}
}