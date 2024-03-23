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
		[Header("Title Screen Game Objects")]
		[SerializeField] GameObject titleScreen;
		[SerializeField] Button singlePlayerButton, hostMultiPlayerButton, joinMultiPlayerButton, quitButton; 
		
		
		[Header("Join Multiplayer Screen Game Objects")]
		[SerializeField] GameObject joinMultiplayerScreen;
		[SerializeField] Button multiPlayerConnectButton, multiPlayerCancelButton;
		[SerializeField] TMP_InputField playerNameTextInput, ipAddressTextInput, portAddressTextInput;
		
		[Header("Multiplayer Lobby Screen Game Objects")]
		[SerializeField] GameObject multiplayerLobbyScreen;
		[SerializeField] GameObject multiPlayerLeaveLobbyButton, multiPlayerStartGameButton;
		[SerializeField] GameObject[] playerDisplays;
		[SerializeField] // TextMeshProUGUI[] playerNames;
		GameObject[] playerNames;
		
		[Header("Scriptable Objects")]
		[SerializeField] UiScriptableObject uiScriptableObject;
		[SerializeField] GameMenuStateEventChannel m_GameMenuStateEventChannel;
		
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
			multiPlayerStartGameButton.GetComponent<Button>().onClick.AddListener(uiScriptableObject.OnMultiplayerStartGameButton);
			multiPlayerLeaveLobbyButton.GetComponent<Button>().onClick.AddListener(uiScriptableObject.OnMultiplayerLeaveLobbyButton);
			
			// Visibility Events
			uiScriptableObject.UpdateMainMenuScreenEvent += UpdateMainMenuScreenEventHandler;
			uiScriptableObject.updatePlayerDisplaysEvent += UpdatePlayerDisplaysEventHandler;
			
			m_GameMenuStateEventChannel.OnEventRaised += GameMenuStateEventHandler;
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
		
		void UpdateMainMenuScreenEventHandler(MainMenu.Screens screenSelection)
		{
			titleScreen.SetActive(screenSelection == MainMenu.Screens.TitleScreen);
			joinMultiplayerScreen.SetActive(screenSelection == MainMenu.Screens.JoinMultiplayerScreen);
			multiplayerLobbyScreen.SetActive(screenSelection == MainMenu.Screens.HostLobbyScreen || screenSelection == MainMenu.Screens.ClientLobbyScreen);
			
			// Client Lobby Screen
			multiPlayerStartGameButton.SetActive(screenSelection == MainMenu.Screens.HostLobbyScreen);
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
		
		void GameMenuStateEventHandler(GameMenuType gameMenu)
		{
			if (gameMenu == GameMenuType.MainMenu)
			{
				titleScreen.SetActive(true);
				joinMultiplayerScreen.SetActive(false);
				multiplayerLobbyScreen.SetActive(false);
			}
			else
			{
				titleScreen.SetActive(false);
				joinMultiplayerScreen.SetActive(false);
				multiplayerLobbyScreen.SetActive(false);
			}
		}
		
	}
}