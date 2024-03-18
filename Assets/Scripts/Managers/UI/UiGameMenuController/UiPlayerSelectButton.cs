using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;

namespace Synthicate
{
	public class UiPlayerSelectButton : NetworkBehaviour
	{
		
		[SerializeField]
		Button m_Button;
		
		[SerializeField]
		TextMeshProUGUI m_ButtonText;
		
		[SerializeField]
		Image m_PlayerImage;
		
		
		public void SetPlayerName(string name)
		{
			m_ButtonText.text = name;
		}
		
	}
}