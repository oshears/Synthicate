using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
	public class GameManagerInitState : GameManagerState
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
				currentPlayer.setName("Player 1");
				currentPlayer.setId(0);
				currentPlayer.init();
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