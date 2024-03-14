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
		[SerializeField]
		TextMeshProUGUI playerName, playerHackerCounter, playerInfluenceCounter;
		[SerializeField]
		Image playerIcon;
		// GameObject playerNameTextObject, playerIcon, playerHackerCounter, playerInfluenceCounter;
		
		
		
		public void SetPlayerName(string name)
		{
			playerName.text = name;
		}
		
		public void SetHackerCount(int count)
		{
			playerHackerCounter.text = $"{count}";
		}
		
		public void SetInfluenceCount(int count)
		{
			playerInfluenceCounter.text = $"{count}";
		}
		
		public void SetPlayerColor(Color color)
		{
			playerIcon.color = color;
		}
	}
}