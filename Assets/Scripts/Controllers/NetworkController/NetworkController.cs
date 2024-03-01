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
		}
		
		void Start()
		{
			transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
		}
		
		void HostGameEventHandler()
		{
			Debug.Log("Network Manager is starting the host!");
			NetworkManager.Singleton.StartHost();
			bool serverStarted = transport.StartServer();
			
			if(!serverStarted) Debug.LogError("ERROR: There may have been an issue starting the host server!");
		}
		
	}
}