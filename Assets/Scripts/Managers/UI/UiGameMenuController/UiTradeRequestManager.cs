using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Synthicate
{
	public class UiTradeRequestManager : UiUpdatableElementMonoBehavior
	{
		#region Trade Window
		[SerializeField]
		GameObject[] initiateTradePlayerButtons, initiateTradePlayerIcons;
		[SerializeField]
		GameObject[] tradeIncrementButtons, tradeDecrementButtons, tradeOfferAmounts, peerTradeAmounts;
		[SerializeField]
		GameObject peerTradeConfirmedIcon;
		[SerializeField]
		GameObject clientTradeConfirmedButton;
		#endregion
		
		
		override protected void Awake() {
			base.Awake();
		}
		
		override protected void InitilizeUserInterfaceEventHandler()
		{
			
		}
		
		override protected void UpdateUserInterfaceEventHandler()
		{
			
		}
		
	}
}