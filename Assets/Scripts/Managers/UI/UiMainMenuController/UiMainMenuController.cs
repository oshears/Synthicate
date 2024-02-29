using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Synthicate
{
	public class UiMainMenuController : MonoBehaviour
	{
		#region TitleScreen
		[SerializeField]
		GameObject titleScreen;
		[SerializeField]
		GameObject singlePlayerButton, hostMultiPlayerButton, joinMultiPlayerButton, quitButton; 
		#endregion
		
		#region JoinMultiPlayerScreen
		[SerializeField]
		GameObject joinMultiplayerScreen;
		[SerializeField]
		GameObject multiPlayerConnectButton, multiPlayerCancelButton;
		[SerializeField]
		GameObject playerNameTextInput, ipAddressTextInput, portAddressTextInput;
		#endregion 
		
		#region MultiPlayerLobbyScreen
		[SerializeField]
		GameObject multiplayerLobbyScreen;
		[SerializeField]
		GameObject multiPlayerLeaveLobbyButton, multiPlayerStartGameButton;
		[SerializeField]
		GameObject[] playerDisplays;
		[SerializeField]
		GameObject[] playerNames;
		#endregion 
		
		#region ScriptableObjects
		[SerializeField]
		UiScriptableObject uiScriptableObject;
		#endregion 
		
		void Awake() {
			singlePlayerButton.GetComponent<Button>().onClick.AddListener(uiScriptableObject.OnSinglePlayerButtonEvent);
			// singlePlayerButton.GetComponent<Button>().onClick.AddListener(HandleSinglePlayerButtonEvent);
			// hostMultiPlayerButton.GetComponent<Button>().onClick.AddListener(HandleHostMultiPlayerButtonEvent);
			// joinMultiPlayerButton.GetComponent<Button>().onClick.AddListener(HandleJoinMultiPlayerButtonEvent);
			// quitButton.GetComponent<Button>().onClick.AddListener(HandleQuitPlayerButtonEvent);
		}
		
		// void HandleSinglePlayerButtonEvent()
		// {
		// 	uiScriptableObject.OnSinglePlayerButtonEvent();
		// }
		
		// void HandleJoinMultiPlayerButtonEvent()
		// {
		// 	uiScriptableObject.OnMultiPlayerButtonEvent();
		// }
		
		// void HandleJoinMultiPlayerButtonEvent()
		// {
		// 	uiScriptableObject.OnMultiPlayerButtonEvent();
		// }
		
		
		
		// void HandleQuitPlayerButtonEvent()
		// {
		// 	uiScriptableObject.OnQuitButtonEvent();
		// }
		
	}
}