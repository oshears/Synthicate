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
		public bool clientIsHost {get; private set;} = false;
		public void SetClientIsHost(bool isHost) => clientIsHost = isHost;
		
		public delegate void NetworkEventHandler();
		public delegate void NetworkConnectionRequestEventHandler(ConnectionRequest request);
		
		public event NetworkEventHandler hostGameEvent;
		public void OnHostGame()
		{
			clientIsHost = true;
			hostGameEvent.Invoke(); 
		}
		
		public event NetworkConnectionRequestEventHandler connectionRequestEvent;
		public void OnConnectionRequest(ConnectionRequest request) => connectionRequestEvent(request); 
		
		public event NetworkEventHandler endNetworkConnectionEvent;
		public void OnEndNetworkConnection() => endNetworkConnectionEvent.Invoke();

	}
}