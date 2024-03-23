using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using System;

namespace Synthicate
{
	
	public class GameManagerMainMenuState : GameManagerAbstractState
	{
		[SerializeField] GameMenuStateEventChannel m_GameMenuStateEventChannel;
	
		public override void Enter()
		{
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.MainMenu);
			_userInterfaceSO.OnUpdateMainMenuScreen(MainMenu.Screens.TitleScreen);
			
			
			_userInterfaceSO.singlePlayerButtonEvent += SinglePlayerButtonEventHandler;
			_userInterfaceSO.hostMultiplayerButtonEvent += HostMultiplayerButtonEventHandler;
			_userInterfaceSO.joinMultiplayerButtonEvent += JoinMultiplayerButtonEventHandler;
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			_userInterfaceSO.singlePlayerButtonEvent -= SinglePlayerButtonEventHandler;
			_userInterfaceSO.hostMultiplayerButtonEvent -= HostMultiplayerButtonEventHandler;
			_userInterfaceSO.joinMultiplayerButtonEvent -= JoinMultiplayerButtonEventHandler;
		}
		
		void SinglePlayerButtonEventHandler()
		{
			Debug.Log("Starting Singleplayer Game!");
			
			_gameManagerSO.Initialize();
			
			// playerManagers[i] = CreateInstance("PlayerManagerSO") as PlayerManagerSO;
			Player currentPlayer = new Player("Player 1", 0);
			_gameManagerSO.AddPlayer(currentPlayer);
			_gameManagerSO.SetClientPlayer(0);
			
			NetworkManager.Singleton.StartHost();
			
			_gameManagerSO.SetCurrentPlayerTurn(0);
			
			// Go to setup state
			if (_gameManagerSO.m_SkipSetup)
			{
				changeState(_owner.idleState);
			}
			else
			{
				changeState(_owner.setupState);
			}
		}
		
		void HostMultiplayerButtonEventHandler()
		{
			
			_transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
			if (_transport == null) Debug.LogError("Failed to find a network transport!");
			bool hostStarted = NetworkManager.Singleton.StartHost();
			if(hostStarted)
			{
				Debug.Log("Starting Multiplayer Game as Host!");
				_gameManagerSO.Initialize();

				// Create host player
				Player currentPlayer = new Player("Player 1", 0);
				_gameManagerSO.AddPlayer(currentPlayer);
				_gameManagerSO.SetClientPlayer(0);
				
			
				_userInterfaceSO.OnUpdateMainMenuScreen(MainMenu.Screens.HostLobbyScreen);
				changeState(_owner.hostLobbyState); 
			} 
			else
			{
				Debug.LogError("ERROR: There may have been an issue starting the host server!");
			}
			
		}
		
		void JoinMultiplayerButtonEventHandler()
		{
			Debug.Log("Starting Multiplayer Game as Client!");
			_userInterfaceSO.OnUpdateMainMenuScreen(MainMenu.Screens.JoinMultiplayerScreen);
			changeState(_owner.clientLobbyState); 
		}
		
	}
}