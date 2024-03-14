using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Synthicate
{
	public class UiNotificationAreaManager : UiUpdatableElementMonoBehavior
	{
		[SerializeField]
		GameObject notificationWindow;
		
		[SerializeField]
		GameObject notificationWindowPrefab;
		
		List<GameObject> m_notificationWindows;
		
		[Header("Event Channels")]
		
		[SerializeField]
		StringEventChannel m_NotificationEvent;
		
		override protected void Awake() {
			base.Awake();
			
			m_notificationWindows = new List<GameObject>();
			
			m_NotificationEvent.OnEventRaised += OnNotificationEventHandler;
		}
		
		void OnEnable() {
			notificationWindow.SetActive(false);
		}
		
		override protected void UpdateUserInterfaceEventHandler()
		{
			
		}
		
		override protected void InitilizeUserInterfaceEventHandler()
		{
			
		}
		
		void Update()
		{
			if (m_notificationWindows.Count > 0)
			{
				List<GameObject> notificationsToRemove = new List<GameObject>();
				
				foreach (GameObject gameObject in m_notificationWindows)
				{
					UiNotificationWindow notification = gameObject.GetComponent<UiNotificationWindow>();
					if (notification.IsExpired())
					{
						Destroy(notification);
						notificationsToRemove.Add(gameObject);
					}
				}
				
				foreach (GameObject gameObject in notificationsToRemove)
				{
					m_notificationWindows.Remove(gameObject);
				}		
			}
		}
		
		void OnNotificationEventHandler(string notificationText)
		{
			GameObject newNotification = Instantiate(notificationWindowPrefab, new Vector3(140f, -30f, 0f), Quaternion.Euler(0,0,0), transform);
			m_notificationWindows.Add(newNotification);
		}
		
	}
}