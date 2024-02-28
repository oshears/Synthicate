using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
	
	public class UiManager : MonoBehaviour
	{
		[SerializeField]
		GameObject _titleScreen;
		
		[SerializeField]
		UiScriptableObject _userInterfaceSO;
		
		public void Awake()
		{
			_userInterfaceSO.setMainMenuActiveEvent.AddListener(SetMainMenuActiveEvent);
		}
		
		public void SetMainMenuActiveEvent(bool active)
		{
			_titleScreen.SetActive(active);
		}
	}
}