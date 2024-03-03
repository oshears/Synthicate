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
		#region Notification Area
		[SerializeField]
		GameObject notificationArea;
		[SerializeField]
		GameObject notificationWindow;
		#endregion
		
		override protected void Awake() {
			base.Awake();
		}
		
		override protected void UpdateUserInterfaceEventHandler()
		{
			
		}
		
		override protected void InitilizeUserInterfaceEventHandler()
		{
			
		}
		
	}
}