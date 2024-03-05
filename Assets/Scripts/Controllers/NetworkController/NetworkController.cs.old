using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace Synthicate
{
	
	public class NetworkController : NetworkBehaviour
	{
		[SerializeReference]
		GameNetworkManagerScriptableObject gameNetworkManagerSO;
		
		UnityTransport transport;
		
		void Awake()
		{
			gameNetworkManagerSO.hostGameEvent += HostGameEventHandler;
			gameNetworkManagerSO.connectionRequestEvent += ConnectionRequestEventHandler;
			gameNetworkManagerSO.ServerUpdateAllPlayerListsOnClientsEvent += ServerUpdateAllPlayerListsOnClientsEventHandler;
		}
		
		void Start()
		{
			NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedEventHandler;
			transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
			if (transport == null) Debug.LogError("Failed to find a network transport!");
		}
		
		void HostGameEventHandler()
		{
			Debug.Log("Network Manager is starting the host!");
			
			if (transport == null) Debug.LogError("Failed to find a network transport!");
			
			bool hostStarted = NetworkManager.Singleton.StartHost();
			
			if(!hostStarted) Debug.LogError("ERROR: There may have been an issue starting the host server!");
		}
		
		void ConnectionRequestEventHandler(ConnectionRequest request)
		{
			transport.SetConnectionData(
				request.ipAddress,  // The IP address is a string
				request.port // The port number is an unsigned short, I have 7777 assigned for Unity Games
			);
			NetworkManager.Singleton.OnClientDisconnectCallback += failedConnection => {
				Debug.Log("Failed to connect to host at: " + request.ipAddress + ":" + request.port);
				NetworkManager.Singleton.Shutdown();
			};
			NetworkManager.Singleton.OnClientConnectedCallback += succeededConnection => {
				Debug.Log("Successfully connected to host at: " + request.ipAddress + ":" + request.port);
				if (!NetworkManager.Singleton.IsServer) gameNetworkManagerSO.OnClientConnectedToServer();
				// Clients must send their player name to the server
				SendClientPlayerNameServerRpc(gameNetworkManagerSO.clientPlayerName, NetworkManager.Singleton.LocalClientId);
			};
			bool clientStarted = NetworkManager.Singleton.StartClient();
			if (!clientStarted) Debug.LogError("ERROR: There may have been an issue starting the client!");
		}
		
		void ClientConnectedEventHandler(ulong clientId)
		{
			if (NetworkManager.Singleton.IsServer)
			{
				Debug.Log($"Recieved a new client connection: {clientId}");
				gameNetworkManagerSO.OnServerRecievedNewClientConnection(clientId);
			}
		}
		
		[ServerRpc(RequireOwnership = false)]
		void SendClientPlayerNameServerRpc(string clientPlayerName, ulong clientId) => ReceiveClientPlayerNameClientRpc(clientPlayerName, clientId);
		[ClientRpc]
		void ReceiveClientPlayerNameClientRpc(string clientPlayerName, ulong clientId)
		{
			if (NetworkManager.Singleton.IsServer)
			{
				Debug.Log($"Got new client ({clientId}) name: {clientPlayerName}");
				gameNetworkManagerSO.OnServerReceivedNewPlayerInfo(clientPlayerName, clientId);
			}
		}
		
		void ServerUpdateAllPlayerListsOnClientsEventHandler(List<Player> playerList)
		{
			StringContainer[] playerNames = new StringContainer[playerList.Count];
			for (int i = 0; i < playerList.Count; i++)
			{
				playerNames[i] = new StringContainer(playerList[i].GetName());
			}
			
			ClientRpcParams clientRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					// TargetClientIds = new ulong[]{clientId}
					
				}
			};
	
			UpdateAllPlayerListsClientRpc(playerNames);
			// UpdateAllPlayerListsServerRpc(playerNames);
			HelloClientRpc();
			GoodbyeClientRpc();
			// HelloServerRpc();
			// GoodbyeServerRpc();
		}
		
		[ClientRpc]
		void UpdateAllPlayerListsClientRpc(StringContainer[] playerNames)
		{
			if(!NetworkManager.Singleton.IsServer)
			{
				string[] playerNamesArry = new string[playerNames.Length];
				for (int i = 0; i < playerNames.Length; i++)
				{
					playerNamesArry[i] = playerNames[i].text;
				}
				gameNetworkManagerSO.OnClientUpdateAllPlayerLists(playerNamesArry);
			}
		}
		
		[ClientRpc]
		void HelloClientRpc()
		{
			Debug.Log("Recieved Hello Client from Server (1)");
		}
		[ClientRpc]
		void GoodbyeClientRpc()
		{
			Debug.Log("Recieved Goodbye Client from Server (2)");
		}
		[ServerRpc(RequireOwnership = false)]
		void HelloServerRpc()
		{
			Debug.Log("Recieved Hello Client from Server (1)");
		}
		[ServerRpc(RequireOwnership = false)]
		void GoodbyeServerRpc()
		{
			Debug.Log("Recieved Goodbye Client from Server (2)");
		}
	}
}