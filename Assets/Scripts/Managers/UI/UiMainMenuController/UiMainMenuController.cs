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
		Button singlePlayerButton, hostMultiPlayerButton, joinMultiPlayerButton, quitButton; 
		#endregion
		
		#region JoinMultiPlayerScreen
		[SerializeField]
		GameObject joinMultiplayerScreen;
		[SerializeField]
		Button multiPlayerConnectButton, multiPlayerCancelButton;
		[SerializeField]
		TMP_InputField playerNameTextInput, ipAddressTextInput, portAddressTextInput;
		#endregion 
		
		#region MultiPlayerLobbyScreen
		[SerializeField]
		GameObject multiplayerLobbyScreen;
		[SerializeField]
		Button multiPlayerLeaveLobbyButton, multiPlayerStartGameButton;
		[SerializeField]
		GameObject[] playerDisplays;
		[SerializeField]
		// TextMeshProUGUI[] playerNames;
		GameObject[] playerNames;
		#endregion 
		
		#region ScriptableObjects
		[SerializeField]
		UiScriptableObject uiScriptableObject;
		#endregion 
		
		void Awake() {
			
			// Title Screen Buttons
			singlePlayerButton.onClick.AddListener(uiScriptableObject.OnSinglePlayerButton);
			hostMultiPlayerButton.onClick.AddListener(uiScriptableObject.OnHostMultiplayerButton);
			joinMultiPlayerButton.onClick.AddListener(uiScriptableObject.OnJoinMultiplayerButton);
			quitButton.onClick.AddListener(uiScriptableObject.OnQuitGameButton);
			
			// Join Game Buttons
			multiPlayerConnectButton.onClick.AddListener(MultiplayerConnectButtonEventHandler);
			multiPlayerCancelButton.onClick.AddListener(uiScriptableObject.OnMultiplayerCancelGameButton);
			
			// Lobby Buttons
			multiPlayerStartGameButton.onClick.AddListener(uiScriptableObject.OnMultiplayerStartGameButton);
			multiPlayerLeaveLobbyButton.onClick.AddListener(uiScriptableObject.OnMultiplayerLeaveLobbyButton);
			
			// Visibility Events
			uiScriptableObject.UpdateMainMenuScreenEvent += UpdateMainMenuScreenEventHandler;
			uiScriptableObject.updatePlayerDisplaysEvent += UpdatePlayerDisplaysEventHandler;
		}
		
		void Start()
		{
			for (int i = 1; i < playerDisplays.Length; i++) playerDisplays[i].SetActive(false);
		}

		
		
		
		void MultiplayerConnectButtonEventHandler()
		{
			ConnectionRequest request;
			// Using this approach because editor directives / pragmas aren't supported in VSCode
			if (ApplicationSettings.IsUnityEditor)
			{
				request = new ConnectionRequest("127.0.0.1", 7777, "Player 2");
			}
			else
			{
				string player = playerNameTextInput.text;
				string ip = ipAddressTextInput.text;
				string port = portAddressTextInput.text;
				ushort portNum = ushort.Parse(port);
				request = new ConnectionRequest(ip, portNum, player);
			}
			
			uiScriptableObject.OnMultiplayerConnectButton(request);
		}
		
		void UpdateMainMenuScreenEventHandler(UserInterface.MainMenuScreens screenSelection)
		{
			titleScreen.SetActive(screenSelection == UserInterface.MainMenuScreens.TitleScreen);
			multiplayerLobbyScreen.SetActive(screenSelection == UserInterface.MainMenuScreens.LobbyScreen);
			joinMultiplayerScreen.SetActive(screenSelection == UserInterface.MainMenuScreens.JoinMultiplayerScreen);
		}
		
		void UpdatePlayerDisplaysEventHandler(List<Player> players)
		{
			Debug.Log($"Updating Player Displays with Num Players: {players.Count}");
			for (int i = 0; i < players.Count; i++)
			{
				playerDisplays[i].SetActive(true);
				// playerNames[i].text = players[i].GetName(); // DEBUG: IDK why this wasn't working
				playerNames[i].GetComponent<TextMeshProUGUI>().text = players[i].GetName();
			} 
		}
		
	}
}