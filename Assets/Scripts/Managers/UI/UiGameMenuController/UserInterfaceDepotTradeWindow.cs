using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;

namespace Synthicate
{
	public class UserInterfaceDepotTradeWindow : MonoBehaviour
	{
		
		[Header("Buttons")]
		
		[SerializeField]
		Button m_ConfirmTradeButton;
		
		[SerializeField]
		Button m_CancelTradeButton;
		
		// [Header("Event Channels")]
		
		// [SerializeField]
		// // EventChannelSO
		
		void OnEnable()
		{
			m_ConfirmTradeButton.onClick.AddListener(ConfirmTradeButtonEventHandler);
			m_CancelTradeButton.onClick.AddListener(CancelTradeButtonEventHandler);
		}
		
		void OnDisable()
		{
			m_ConfirmTradeButton.onClick.RemoveAllListeners();
			m_CancelTradeButton.onClick.RemoveAllListeners();
		}
		
		
		void ConfirmTradeButtonEventHandler()
		{
			
		}
		
		void CancelTradeButtonEventHandler()
		{
			
		}
		
	}
}