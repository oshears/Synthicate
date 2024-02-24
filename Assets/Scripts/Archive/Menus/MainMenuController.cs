using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;



namespace Synthicate
{
	public class MainMenuController : NetworkBehaviour
	{

		public GameManagerSO gameManagerSO;

		public Texture mainTitle;

		public GUISkin menuSkin;

		public AudioClip selectionAudio;

		AudioSource audioSource;

		NetworkVariable<int> connectedPlayers = new NetworkVariable<int>();

		UnityTransport transport;

		enum MenuStates { Main, JoinGame, ClientInLobby, HostGame, Settings, InGame, GameMenu, Inactive };
		MenuStates state;

		string ipAddress = "";
		string port = "";
		string playerName = "Player";

		//bool failedJoin = false;

		List<string> playerNames = new List<string>();


		// Start is called before the first frame update
		void Start()
		{
			state = MenuStates.Main;


			audioSource = GetComponent<AudioSource>();

			//Debug.Log(gameManagerSO);
			//Debug.Log(gameManagerSO.clientStartGameEvent);
			gameManagerSO.updateMainMenuEvent.AddListener(respondToMainMenuUpdate);

			transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

		}

		// Update is called once per frame
		void Update()
		{

		}

		void respondToMainMenuUpdate(bool enable) => state = enable ? MenuStates.Main : MenuStates.Inactive;

		int getNumConnectedPlayers()
		{
			if (NetworkManager.Singleton.IsServer)
			{
				return NetworkManager.Singleton.ConnectedClientsIds.Count;
			}
			else
			{
				updatePlayerCountServerRpc();
				return connectedPlayers.Value;
			}
		}

		[ServerRpc (RequireOwnership = false)]
		void updatePlayerCountServerRpc()
		{
			connectedPlayers.Value = NetworkManager.Singleton.ConnectedClientsIds.Count;
		}

		[ServerRpc(RequireOwnership = false)]
		void sendNameToServerServerRpc(string playerName)
		{
			playerNames.Add(playerName);
		}

		private void OnGUI()
		{
			// update box sizes
			float screenScaleX = Screen.width * 0.65f;
			float screenScaleY = Screen.height * 0.65f;
			int xStart = (int)(Screen.width - screenScaleX) / 2;
			int yStart = 0;

			Rect screenArea = new Rect(0, 0, Screen.width, Screen.height);
			Rect titleArea = new Rect(xStart, yStart, screenScaleX, screenScaleY);
			Rect menuArea = new Rect(3 * Screen.width / 8, yStart + Screen.height * 0.55f, Screen.width * 0.25f, Screen.height * 0.5f);
			

			//Rect resourceStatusArea = new Rect(7 * Screen.width / 8, 0, Screen.width / 16f, 2 * Screen.height / 16);

			GUI.skin = menuSkin;

			if (state != MenuStates.Inactive)
			{
				// tinted background
				GUI.Box(screenArea, "");

				// title box
				GUILayout.BeginArea(titleArea);
				GUILayout.Box(mainTitle, menuSkin.GetStyle("titleBox"));
				GUILayout.EndArea();
			}
			
			if (state == MenuStates.Main)
			{
				// menu box
				GUILayout.BeginArea(menuArea);
				joinHostedGameButton(GUILayout.Button("Join Game"));
				hostGameButton(GUILayout.Button("Host Game"));
				settingsButton(GUILayout.Button("Settings"));
				exitButton(GUILayout.Button("Exit"));
				GUILayout.EndArea();
			}
			else if (state == MenuStates.JoinGame)
			{
				// menu box
				GUILayout.BeginArea(menuArea);
				GUILayout.Label("IP Address:");
				ipAddress = GUILayout.TextArea(ipAddress);
				GUILayout.Label("Port:");
				port = GUILayout.TextArea(port);
				GUILayout.Label("Your Name:");
				playerName = GUILayout.TextArea(playerName);
				joinGameButton(GUILayout.Button("Join"));
				cancelJoinButton(GUILayout.Button("Cancel"));
				GUILayout.EndArea();

				// it may take the client a moment to connect
				if (NetworkManager.Singleton.IsConnectedClient)
				{
					state = MenuStates.ClientInLobby;

					sendNameToServerServerRpc(playerName);

					Debug.Log(playerName+" Joined Server!");
				}
			}
			else if (state == MenuStates.ClientInLobby)
			{
				GUILayout.BeginArea(menuArea);
				GUILayout.Label("Players in Lobby: " + getNumConnectedPlayers());
				leaveLobbyButton(GUILayout.Button("Leave Lobby"));
				GUILayout.EndArea();
			}
			else if (state == MenuStates.HostGame)
			{
				// menu box
				GUILayout.BeginArea(menuArea);
				GUILayout.Label("Your Name:");
				playerName = GUILayout.TextArea(playerName);
				GUILayout.Label("Players in Lobby: " + getNumConnectedPlayers());
				//GUILayout.Label("Waiting for players...");
				startGameButton(GUILayout.Button("Start Game"));
				cancelHostButton(GUILayout.Button("Cancel"));
				GUILayout.EndArea();

			}
			else if (state == MenuStates.Settings)
			{
				// menu box
				GUILayout.BeginArea(menuArea);
				GUILayout.Button("Visual Effects");
				GUILayout.Button("Sound Effects");
				cancelButton(GUILayout.Button("Cancel"));
				GUILayout.EndArea();
			}

		}

		
		//void openGameMenuButton(bool clicked)
		//{
		//    if (!clicked) return;

		//    state = MenuStates.GameMenu;
		//    playMenuSelectionAudio();

		//}

		//void closeGameMenuButton(bool clicked)
		//{
		//    if (!clicked) return;

		//    state = MenuStates.InGame;
		//    playMenuSelectionAudio();

		//}

		void startGameButton(bool clicked)
		{
			if (!clicked) return;

			state = MenuStates.Inactive;
			playMenuSelectionAudio();

			playerNames.Insert(0, playerName);
			gameManagerSO.hostStartGame((uint) NetworkManager.ConnectedClientsIds.Count,playerNames);
		}

		void joinHostedGameButton(bool clicked)
		{
			if (!clicked) return;

			state = MenuStates.JoinGame;
			playMenuSelectionAudio();
		}
		void joinGameButton(bool clicked)
		{
			if (!clicked) return;

			state = MenuStates.JoinGame;
			playMenuSelectionAudio();

			
			transport.SetConnectionData(
				ipAddress,  // The IP address is a string
				ushort.Parse(port) // The port number is an unsigned short, I have 7777 assigned for Unity Games
			);
			NetworkManager.Singleton.OnClientDisconnectCallback += failedConnection => {
				Debug.Log("Failed to connect to host at: " + ipAddress + ":" + port);
				NetworkManager.Singleton.Shutdown();
			};
			NetworkManager.Singleton.OnClientConnectedCallback += succeededConnection => {
				Debug.Log("Successfully connected to host at: " + ipAddress + ":" + port);
				state = MenuStates.Main;
			};

			NetworkManager.Singleton.StartClient();
		}

		void hostGameButton(bool clicked)
		{
			if (!clicked) return;

			state = MenuStates.HostGame;
			playMenuSelectionAudio();

			// start network host
			NetworkManager.Singleton.StartHost();

		}

		void cancelHostButton(bool clicked)
		{
			if (!clicked) return;

			state = MenuStates.Main;
			playMenuSelectionAudio();

			// disconnect all clients
			//foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
			//    NetworkManager.Singleton.DisconnectClient(clientId);
			//NetworkManager.Singleton.Shutdown(true);
		}

		void cancelJoinButton(bool clicked)
		{
			if (!clicked) return;
			state = MenuStates.Main;
			playMenuSelectionAudio();
			//NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.client)
			//NetworkManager.Singleton.Shutdown(true);
		}

		void leaveLobbyButton(bool clicked)
		{
			if (!clicked) return;
			state = MenuStates.Main;
			playMenuSelectionAudio();
			//NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.client)
			//NetworkManager.Singleton.Shutdown();
		}

		void settingsButton(bool clicked)
		{
			if (!clicked) return;

			state = MenuStates.Settings;
			playMenuSelectionAudio();

		}

		void exitButton(bool clicked)
		{
			if (!clicked) return;



			//state = MenuStates.JoinGame;
			playMenuSelectionAudio();

			Application.Quit();

		}

		void cancelButton(bool clicked)
		{
			if (!clicked) return;
			state = MenuStates.Main;
			playMenuSelectionAudio();
		}

		void playMenuSelectionAudio()
		{
			audioSource.clip = selectionAudio;
			//Debug.Log(audioSource.clip);
			audioSource.Play();
		}
	}
}
