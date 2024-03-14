using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;

namespace Synthicate
{
	public class UiNotificationWindow : NetworkBehaviour
	{
		
		static float s_DefaultNotificationTime = 5;
		
		float m_NotificationDuration;
		
		[SerializeField]
		TextMeshProUGUI m_NotificationText;
		
		bool m_NotificationExpired;
		
		public void InitializeNotification(string notificationText)
		{
			m_NotificationText.text = notificationText;
			m_NotificationDuration = 0;
			m_NotificationExpired = false;
		} 
		
		void Update()
		{
			m_NotificationDuration += Time.deltaTime;
			
			if (m_NotificationDuration > s_DefaultNotificationTime)
			{
				m_NotificationExpired = true;
			}
		}
		
		public bool IsExpired() => m_NotificationExpired;
	}
}