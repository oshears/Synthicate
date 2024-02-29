using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace Synthicate
{
	
	public class GameNetworkManager : NetworkManager
	{
		[SerializeField]
		GameNetworkManagerScriptableObject gameNetworkManagerSO;
		
		UnityTransport transport;
		
		void Awake()
		{
			gameNetworkManagerSO.hostGameEvent += HostGameEventHandler;
		}
		
		void Start()
		{
			transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
		}
		
		void HostGameEventHandler()
		{
			Debug.Log("Network Manager is starting the host!");
			StartHost();
			bool serverStarted = transport.StartServer();
			
			if(!serverStarted) Debug.LogError("ERROR: Could not start the host server!");
		}
		
	}
}