using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Synthicate
{
	public class UiPlayerViewController : MonoBehaviour
	{
		#region Players View
		[SerializeField]
		GameObject playerNameTextObject, playerIcon, playerHackerCounter, playerInfluenceCounter;
		#endregion
		
		
		
		public void SetPlayerName(string playerName)
		{
			playerNameTextObject.GetComponent<TextMeshProUGUI>().text = playerName;
		}
		
		public void SetHackerCount(int count)
		{
			playerHackerCounter.GetComponent<TextMeshProUGUI>().text = $"{count}";
		}
		
		public void SetInfluenceCount(int count)
		{
			playerInfluenceCounter.GetComponent<TextMeshProUGUI>().text = $"{count}";
		}
		
		public void SetPlayerColor(Color color)
		{
			playerIcon.GetComponent<Image>().color = color;
		}
	}
}