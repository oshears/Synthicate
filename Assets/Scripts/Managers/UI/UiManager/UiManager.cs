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
		
		[SerializeField]
		GameObject _gameMenuScreen;
		
		public void Awake()
		{
			_userInterfaceSO.setMainMenuActiveEvent.AddListener(SetMainMenuActiveEventHandler);
			_userInterfaceSO.setGameMenuActiveEvent.AddListener(SetGameMenuActiveEventHandler);
		}
		
		public void Start()
		{
			_gameMenuScreen.SetActive(false);
		}
		
		public void SetMainMenuActiveEventHandler(bool active)
		{
			_titleScreen.SetActive(active);
		}
		
		public void SetGameMenuActiveEventHandler(bool active)
		{
			_gameMenuScreen.SetActive(active);
		}
	}
}