using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
	[CreateAssetMenu(fileName = "GameNetworkManagerScriptableObject", menuName = "ScriptableObjects/GameNetworkManagerScriptableObject")]
	public class GameNetworkManagerScriptableObject : ScriptableObject
	{
		public string clientPlayerName{get; private set;}
		public bool clientIsHost {get; private set;} = false;
		public void SetClientIsHost (bool isHost) => clientIsHost = isHost;
		public int numConnectedClients {get; private set;} = 0;
		public List<ulong> connectedClientIds {get; private set;}
		public void IncrementNumClients() => numConnectedClients++;
		
		public delegate void NetworkEventHandler();
		public delegate void NetworkConnectionRequestEventHandler(ConnectionRequest request);
		
		public event NetworkEventHandler hostGameEvent;
		public void OnHostGame(string playerName)
		{
			clientPlayerName = playerName;
			clientIsHost = true;
			numConnectedClients = 0;
			hostGameEvent.Invoke(); 
		}
		
		public event NetworkConnectionRequestEventHandler connectionRequestEvent;
		public void OnConnectionRequest(ConnectionRequest request){
			clientPlayerName = request.playerName;
			connectionRequestEvent(request);
		} 
		public event NetworkEventHandler clientConnectedToServer;
		public void OnClientConnectedToServer() => clientConnectedToServer.Invoke();  
		public event NetworkEventHandler serverRecievedNewClientConnection;
		public void OnServerRecievedNewClientConnection(ulong clientId){
			numConnectedClients++;
			connectedClientIds.Add(clientId);
			serverRecievedNewClientConnection.Invoke();  
		}
		public event NetworkEventHandler endNetworkConnectionEvent;
		public void OnEndNetworkConnection() => endNetworkConnectionEvent.Invoke();
		public void SetClientPlayerName(string name)
		{
			clientPlayerName = name;
		}
		
		public delegate void ServerReceivedNewPlayerInfoEventHandler(string playerName, ulong clientId);
		public ServerReceivedNewPlayerInfoEventHandler ServerReceivedNewPlayerInfoEvent;
		public void OnServerReceivedNewPlayerInfo(string playerName, ulong clientId)
		{
			ServerReceivedNewPlayerInfoEvent.Invoke(playerName, clientId);
		}
		
		public delegate void ServerUpdateAllPlayerListsOnClientsEventHandler(List<Player> players);
		public ServerUpdateAllPlayerListsOnClientsEventHandler ServerUpdateAllPlayerListsOnClientsEvent;
		public void OnServerUpdateAllPlayerListsOnClients(List<Player> players) => ServerUpdateAllPlayerListsOnClientsEvent.Invoke(players);
	
		public delegate void ClientUpdateAllPlayerListsEventHandler(string[] playerNames);
		public ClientUpdateAllPlayerListsEventHandler ClientUpdateAllPlayerListsEvent;
		public void OnClientUpdateAllPlayerLists(string[] playerNames) => ClientUpdateAllPlayerListsEvent.Invoke(playerNames);
		
		public NetworkEventHandler ClientReadyEvent;
		public void OnClientReady() => ClientReadyEvent.Invoke();
	
	}
}