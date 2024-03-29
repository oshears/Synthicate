using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
	[CreateAssetMenu(fileName = "UiScriptableObject", menuName = "ScriptableObjects/UiScriptableObject")]
	public class UiScriptableObject : ScriptableObject
	{
		
		[SerializeField]
		public Sprite[] ResourceSprites;
		
		public delegate void UserInterfaceEvent();
		
		#region Title Screen Button Events
		public event UserInterfaceEvent singlePlayerButtonEvent;
		public void OnSinglePlayerButton() => singlePlayerButtonEvent.Invoke();
		public event UserInterfaceEvent hostMultiplayerButtonEvent;
		public void OnHostMultiplayerButton() => hostMultiplayerButtonEvent.Invoke();
		public event UserInterfaceEvent joinMultiplayerButtonEvent;
		public void OnJoinMultiplayerButton() => joinMultiplayerButtonEvent.Invoke();
		public event UserInterfaceEvent quitGameButtonEvent;
		public void OnQuitGameButton() => quitGameButtonEvent.Invoke();
		#endregion
		
		#region Multiplayer Lobby Button Events
		public event UserInterfaceEvent multiplayerStartGameButtonEvent;
		public void OnMultiplayerStartGameButton() => multiplayerStartGameButtonEvent.Invoke();
		public event UserInterfaceEvent multiplayerLeaveLobbyButtonEvent;
		public void OnMultiplayerLeaveLobbyButton() => multiplayerLeaveLobbyButtonEvent.Invoke();
		
		#endregion
		
		#region Join Multiplayer Button Events
		public delegate void MultiplayerConnectButtonEventHandler(ConnectionRequest request);
		public event MultiplayerConnectButtonEventHandler multiplayerConnectButtonEvent;
		public void OnMultiplayerConnectButton(ConnectionRequest request) => multiplayerConnectButtonEvent.Invoke(request);
		public event UserInterfaceEvent multiplayerCancelGameButtonEvent;
		public void OnMultiplayerCancelGameButton() => multiplayerCancelGameButtonEvent.Invoke();
		public delegate void UpdatePlayerDisplaysEventHandler(List<Player> players);
		public event UpdatePlayerDisplaysEventHandler updatePlayerDisplaysEvent;
		public void OnUpdatePlayerDisplays(List<Player> players) => updatePlayerDisplaysEvent.Invoke(players); 
		#endregion
		
		#region Main Menu Events
		public delegate void UpdateMainMenuScreenEventHandler(MainMenu.Screens screenSelection);
		public event UpdateMainMenuScreenEventHandler UpdateMainMenuScreenEvent;
		public void OnUpdateMainMenuScreen(MainMenu.Screens screenSelection) => UpdateMainMenuScreenEvent.Invoke(screenSelection);
		#endregion
		
	}
}