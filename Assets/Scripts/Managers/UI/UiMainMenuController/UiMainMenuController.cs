using System.Collections;
using System.Collections.Generic;
using TMPro;
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
			
			// Title Screen Buttons
			singlePlayerButton.GetComponent<Button>().onClick.AddListener(uiScriptableObject.OnSinglePlayerButton);
			hostMultiPlayerButton.GetComponent<Button>().onClick.AddListener(uiScriptableObject.OnHostMultiplayerButton);
			joinMultiPlayerButton.GetComponent<Button>().onClick.AddListener(uiScriptableObject.OnJoinMultiplayerButton);
			quitButton.GetComponent<Button>().onClick.AddListener(uiScriptableObject.OnQuitGameButton);
			
			// Join Game Buttons
			multiPlayerConnectButton.GetComponent<Button>().onClick.AddListener(MultiplayerConnectButtonEventHandler);
			multiPlayerCancelButton.GetComponent<Button>().onClick.AddListener(uiScriptableObject.OnMultiplayerCancelGameButton);
			
			// Lobby Buttons
			multiPlayerStartGameButton.GetComponent<Button>().onClick.AddListener(uiScriptableObject.OnMultiplayerStartGameButton);
			multiPlayerLeaveLobbyButton.GetComponent<Button>().onClick.AddListener(uiScriptableObject.OnMultiplayerLeaveLobbyButton);
			
			// Visibility Events
			uiScriptableObject.UpdateMainMenuScreenEvent += UpdateMainMenuScreenEventHandler;
		}

		
		
		
		void MultiplayerConnectButtonEventHandler()
		{
			string player = playerNameTextInput.GetComponent<TMP_InputField>().text;
			string ip = ipAddressTextInput.GetComponent<TMP_InputField>().text;
			uint port = uint.Parse(portAddressTextInput.GetComponent<TMP_InputField>().text);
			ConnectionRequest request = new ConnectionRequest(ip, port, player);
			uiScriptableObject.OnMultiplayerConnectButton(request);
		}
		
		void UpdateMainMenuScreenEventHandler(UserInterface.MainMenuScreens screenSelection)
		{
			titleScreen.SetActive(screenSelection == UserInterface.MainMenuScreens.TitleScreen);
			multiplayerLobbyScreen.SetActive(screenSelection == UserInterface.MainMenuScreens.LobbyScreen);
			joinMultiplayerScreen.SetActive(screenSelection == UserInterface.MainMenuScreens.JoinMultiplayerScreen);
		}
		
	}
}