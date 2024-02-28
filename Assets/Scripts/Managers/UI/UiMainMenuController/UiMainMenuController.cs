using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Synthicate
{
	public class UiMainMenuController : MonoBehaviour
	{
		[SerializeField]
		GameObject singlePlayerButton, multiPlayerButton, quitButton; 
		
		// [SerializeField]
		// Button testButton;
		[SerializeField]
		UiScriptableObject uiScriptableObject;
		
		void Awake() {
			singlePlayerButton.GetComponent<Button>().onClick.AddListener(HandleSinglePlayerButtonEvent);
			multiPlayerButton.GetComponent<Button>().onClick.AddListener(HandleMultiPlayerButtonEvent);
			quitButton.GetComponent<Button>().onClick.AddListener(HandleQuitPlayerButtonEvent);
		}
		
		void HandleSinglePlayerButtonEvent()
		{
			uiScriptableObject.OnSinglePlayerButtonEvent();
		}
		
		void HandleMultiPlayerButtonEvent()
		{
			uiScriptableObject.OnMultiPlayerButtonEvent();
		}
		
		void HandleQuitPlayerButtonEvent()
		{
			uiScriptableObject.OnQuitButtonEvent();
		}
		
	}
}