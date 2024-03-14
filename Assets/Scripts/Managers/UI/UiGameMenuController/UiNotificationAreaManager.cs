using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Synthicate
{
	public class UiNotificationAreaManager : NetworkedUserInterfaceElement, IUpdatableUserInterfaceElement
	{
		
		
		[Header("Notification Prefabs")]
		
		[SerializeField]
		GameObject notificationWindow;
		
		[SerializeField]
		GameObject notificationWindowPrefab;
		
		List<GameObject> m_notificationWindows;
		
		[Header("Event Channels")]
		
		[SerializeField]
		StringEventChannel m_NotificationEvent;
		
		void Awake() {

			userInterfaceSO.initializeUserInterfaceEvent += InitilizeUserInterfaceEventHandler;
			userInterfaceSO.updateUserInterfaceEvent += UpdateUserInterfaceEventHandler;
			
			m_notificationWindows = new List<GameObject>();
			m_NotificationEvent.OnEventRaised += OnNotificationEventHandler;
		}
		
		void OnEnable() {
			notificationWindow.SetActive(false);
		}
		
		public void UpdateUserInterfaceEventHandler()
		{
			
		}
		
		public void InitilizeUserInterfaceEventHandler()
		{
			
		}
		
		void Update()
		{
			if (m_notificationWindows.Count > 0)
			{
				
				// List<GameObject> notificationsToDisplay = name List<GameObject>();
				List<GameObject> notificationsToRemove = new List<GameObject>();
				
				// Keep track of the number of active notifications
				int activeNotifications = 0;
				
				// Iterate through each notification
				foreach (GameObject gameObject in m_notificationWindows)
				{
					// Destroy expired notifications
					UiNotificationWindow notification = gameObject.GetComponent<UiNotificationWindow>();
					if (notification.IsExpired())
					{
						gameObject.SetActive(false);
						Destroy(notification);
						notificationsToRemove.Add(gameObject);
					}
					
					// Update the positions of active notifications
					if (gameObject.activeSelf)
					{
						gameObject.transform.localPosition = new Vector3(4.33f, 238f + activeNotifications++ * -65f, 0);
					}
				}
				
				// Remove expired notifications
				foreach (GameObject gameObject in notificationsToRemove)
				{
					m_notificationWindows.Remove(gameObject);
				}		
			}
		}
		
		void OnNotificationEventHandler(string notificationText)
		{
			// GameObject newNotification = Instantiate(notificationWindowPrefab, transform);
			// newNotification.GetComponent<NetworkObject>().Spawn();
			GameObject notifyWindow = Resources.Load<GameObject>("Prefabs/UI/Game Menu/Notifications/Notification Window");
			// GameObject newNotification = Instantiate(NetworkManager.GetNetworkPrefabOverride(notifyWindow), transform);
			GameObject newNotification = Instantiate(notifyWindow, transform);
			// GameObject newNotification = Instantiate(NetworkManager.GetNetworkPrefabOverride(notificationWindowPrefab), transform);
			newNotification.GetComponent<NetworkObject>().Spawn();
			newNotification.transform.localPosition = new Vector3(4.33f, 238f + (m_notificationWindows.Count + 1) * -65f, 0);
			newNotification.GetComponent<UiNotificationWindow>().InitializeNotification(notificationText);
			m_notificationWindows.Add(newNotification);
			newNotification.SetActive(true);
		}
		
	}
}