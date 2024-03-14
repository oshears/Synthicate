using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Synthicate
{
	
	public class UserInterfaceNetworkDebugger : NetworkBehaviour
	{
		[SerializeField]
		GameObject m_NotificationWindowPrefab;
		
		[SerializeField]
		StringEventChannel m_NotificationEvent;
	
		bool test = true;
		void OnEnable()
		{
			// NewNotification();
			
			m_NotificationEvent.OnEventRaised += NewNotification;
		}
		
		void Start()
		{
			// if(test)
			// {
			// 	NewNotification();
			// 	test = false;
			// }
		}
		
		void Update()
		{
			
		}
		
		void NewNotification(string notificationText = "Hello World")
		{
			// GameObject notifyWindow = Resources.Load<GameObject>("Prefabs/UI/Game Menu/Notifications/Notification Window");
			GameObject newNotification = Instantiate(NetworkManager.GetNetworkPrefabOverride(m_NotificationWindowPrefab), transform);
			// NetworkManager.GetNetworkPrefabOverride
			// GameObject newNotification = Instantiate(m_NotificationWindowPrefab,transform);
			// GameObject newNotification = Instantiate(notifyWindow, transform);
			// newNotification.GetComponent<UiNotificationWindow>().InitializeNotification("Hello World!");
			
			// NetworkManager.Singleton.StartHost();
			NetworkObject netObj = newNotification.GetComponent<NetworkObject>();
			netObj.Spawn();
			
			// test = true;
		}
	}
}