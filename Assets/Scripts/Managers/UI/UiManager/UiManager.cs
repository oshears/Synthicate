using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
	
	public class UiManager : MonoBehaviour
	{
		[SerializeField]
		UiScriptableObject _userInterfaceSO;
		[SerializeField]
		GameObject _mainMenuScreen;
		
		[SerializeField]
		GameObject _gameMenuScreen;
		
		public void Awake()
		{
			_userInterfaceSO.setMainMenuActiveEvent.AddListener(SetMainMenuActiveEventHandler);
			_userInterfaceSO.setGameMenuActiveEvent.AddListener(SetGameMenuActiveEventHandler);
		}
		
		public void Start()
		{
			_mainMenuScreen.SetActive(true);
			_gameMenuScreen.SetActive(false);
		}
		
		public void SetMainMenuActiveEventHandler(bool active)
		{
			_mainMenuScreen.SetActive(active);
		}
		
		public void SetGameMenuActiveEventHandler(bool active)
		{
			_gameMenuScreen.SetActive(active);
		}
	}
}