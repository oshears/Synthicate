using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Synthicate
{
	public class UiResourceWindowManager : UiUpdatableElementMonoBehavior
	{
		[SerializeField]
		GameObject powerCounterText;
		
		[SerializeField]
		GameObject peopleCounterText;
		
		[SerializeField]
		GameObject foodCounterText;
		
		[SerializeField]
		GameObject metalCounterText;
		
		[SerializeField]
		GameObject mechCounterText;
		
		[SerializeField]
		GameObject hackerCounterText;
		
		[SerializeField]
		GameObject pointCounterText;
		
		[Header("Event Channels")]
		
		[SerializeField]
		EventChannelSO m_UpdateUiEventChannel;
		
		override protected void Awake() {
			base.Awake();
			m_UpdateUiEventChannel.OnEventRaised += UpdateUserInterfaceEventHandler;
		}
		
		
		
		override protected void InitilizeUserInterfaceEventHandler()
		{
			
		}
		
		override protected void UpdateUserInterfaceEventHandler()
		{
			powerCounterText.GetComponent<TextMeshProUGUI>().text = $"{gameManagerSO.clientPlayer.GetResourceCount(ResourceType.Power)}";
			peopleCounterText.GetComponent<TextMeshProUGUI>().text = $"{gameManagerSO.clientPlayer.GetResourceCount(ResourceType.People)}";
			foodCounterText.GetComponent<TextMeshProUGUI>().text = $"{gameManagerSO.clientPlayer.GetResourceCount(ResourceType.Food)}";
			metalCounterText.GetComponent<TextMeshProUGUI>().text = $"{gameManagerSO.clientPlayer.GetResourceCount(ResourceType.Metal)}";
			mechCounterText.GetComponent<TextMeshProUGUI>().text = $"{gameManagerSO.clientPlayer.GetResourceCount(ResourceType.Mech)}";
			// hackerCounterText.GetComponent<TextMeshProUGUI>().text = gameManagerSO.clientPlayer.GetResourceCount();
			// pointCounterText.GetComponent<TextMeshProUGUI>().text = gameManagerSO.clientPlayer.GetResourceCount();
		}
		
	}
}