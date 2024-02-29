using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace Synthicate
{
	public class GameManagerInitState : GameManagerAbstractState
	{
		public GameManagerInitState(GameManager owner) : base(owner) 
		{
			_userInterfaceSO.singlePlayerButton.AddListener(SinglePlayerButtonEventHandler);
		}

		public override void Enter()
		{
			_userInterfaceSO.OnSetMainMenuActive(true);
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			_userInterfaceSO.OnSetMainMenuActive(false);
		}
		
		public void SinglePlayerButtonEventHandler()
		{
			Debug.Log("Starting Singleplayer Game!");
			
			_gameManagerSO.Initialize();
			_gameManagerSO.SetNumPlayers(1);
			
			for (int i = 0; i < _gameManagerSO.numPlayers; i++)
			{
				// playerManagers[i] = CreateInstance("PlayerManagerSO") as PlayerManagerSO;
				Player currentPlayer = new Player();
				_gameManagerSO.AddPlayer(currentPlayer);
				_gameManagerSO.SetClientPlayer(currentPlayer);
				currentPlayer.SetName("Player 1");
				currentPlayer.SetId(0);
				currentPlayer.Initialize();
				
				NetworkManager.Singleton.StartHost();
			}


			// if (clientPlayerManager.getId() == currentSetupTurn)
			// 	beginClientSetupStrongholdBuildMode();

			// updateGUIEvent.Invoke();
			// updateMainMenuEvent.Invoke(false);
			
			// Go to setup state
			changeState(new GameManagerSetupState(_owner));
		}
		
		public void MultiPlayerButtonEventHandler()
		{
			Debug.Log("Starting Multiplayer Game!");
			
			// uint[] clientIds = new uint[numPlayers];
			// for (int i = 0; i < numPlayers; i++)
			// {
			// 	initClientSOsClientRpc(clientIds,new NetworkStringArray(playerNames.ToArray()));
			// 	clientIds[i] = (uint)NetworkManager.ConnectedClientsIds[i];
			// } 
			
		}

	}
}